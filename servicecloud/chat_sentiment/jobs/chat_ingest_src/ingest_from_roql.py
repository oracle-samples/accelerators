################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:47 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: f1bfa3b270efd947e0b646dcd0d3b5cf7b1193a3 $
################################################################################################
#  File: ingest_from_roql.py
################################################################################################

import logging
import os
import sys
import pandas as pd

from io import BytesIO
from datetime import datetime, timedelta
from os.path import dirname, abspath

from tornado import concurrent

from common.constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, CX_DOMAIN, \
    INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, CHAT_ID_PLACEHOLDER
from common.storage import get_object_storage_client, is_directory, get_bucket_details, get_csv_file_names
from common.utils import invoke_roql_api

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

DEFAULT_ENCODING = 'utf-8'


def build_roql_data_frame(report_name, day, report_query, record_position=0):
    """
        This method will use ROQL query to get aia_incidents report data when thread masking is enabled.
        param column_names: name of report column.
        param rows: value of each rows.
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    columns = []
    rows = []
    is_loop = True
    while is_loop:
        response = call_roql_api(record_position, report_name, report_query)
        if response.status_code == 200:
            data = response.json()
            if len(data["items"]) != 0 and (data["items"][0]["count"] != 0):
                for item in data["items"]:
                    rows.extend(item["rows"])
                    if len(columns) == 0:
                        columns.extend(item['columnNames'])
                    logger.debug(f"Record Position: and the total rows are : {len(rows)}")

                record_position = record_position + 1000
            else:
                is_loop = False
        else:
            logger.error(f"Error fetching report data: aia_incidents and the error is : {response.json()}")
            is_loop = False

    logger.info(
        f"***  : get data for date {day} and found rows {len(rows)} ***")

    return columns, rows


def call_roql_api(record_position, report_name, report_query):
    """
        this method is used to create the roql final query and call the invoke_roql_api method
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    logger.info(f"*** Ingesting {report_name} ***")
    query_value = report_query + ' LIMIT 1000 OFFSET ' + str(record_position)
    url = f'https://{os.getenv(CX_DOMAIN)}/services/rest/connect/v1.3/queryResults/?query={query_value}'
    return invoke_roql_api(url)


def build_chat_data(report_name, report_query, is_report_present):
    """
        This method check if an incident report is present or not in oci bucket
        if report is present it will fetch data for current day
        else it will fetch data for INGESTION_JOB_INITIAL_DATA_FETCH_DAYS number of days
        param is_report_present: if bucket has a file or not.
        param ocs_client: ocs client.
        param report_name: current report to be fetched
        param report_query: query for current report
        param ocs_path: path of bucket
    """
    futures = []
    tasks = []
    executor = concurrent.futures.ThreadPoolExecutor(max_workers=max(os.cpu_count() - 1, 1))

    if is_report_present:
        date_to_fetch_data = datetime.now() - timedelta(days=1)
        report_query = report_query.replace("END_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                       date_to_fetch_data.max.time())))
        report_query = report_query.replace("START_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                         date_to_fetch_data.min.time())))
        tasks.append((report_name, date_to_fetch_data.date(), report_query))
    else:
        date_range = int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, 365))
        logger.info(f"*** Fetching data of last {date_range} days")
        for counter in range(date_range):
            date_to_fetch_data = datetime.now() - timedelta(
                days=(float(date_range) - counter))
            query = report_query.replace("END_DATE", str(datetime.combine(date_to_fetch_data.date(),
                                                                          datetime.max.time())))
            query = query.replace("START_DATE",
                                  str(datetime.combine(date_to_fetch_data.date(), datetime.min.time())))
            tasks.append((report_name, date_to_fetch_data.date(), query))

    for filter_task_args in tasks:
        future = executor.submit(build_roql_data_frame, *filter_task_args)
        futures.append(future)

    concurrent.futures.wait(futures)

    columns, rows = [], []
    for future in concurrent.futures.as_completed(futures):
        try:
            n_columns, n_rows = future.result()
            columns.extend(n_columns)
            rows.extend(n_rows)
        except Exception as exc:
            logger.error('generated an exception: %s' % exc)

    executor.shutdown()
    logger.info("*** All Ingestion Threads Complete ***")
    return columns, rows


def fetch_report(report_name, query):
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    ocs_client = get_object_storage_client()
    logger.info("Connection with OCI Established")
    bucket_location = os.getenv(MAIN_SOURCE_BUCKET_LOCATION)
    is_report_present = is_directory(bucket_location, ocs_client)
    logger.info(f"is_report_present: {is_report_present}")
    if is_report_present:
        logger.info(f"*** Getting Feedback For  {report_name} ***")
        bucket_name, folder_name = get_bucket_details(bucket_location)
        namespace, recent_files = get_csv_file_names(bucket_name=bucket_name,
                                                     prefix=f"{folder_name}feedback_ingest_",
                                                     client=ocs_client)
        sorted_file_paths = sorted(recent_files, key=lambda x: x.split('_')[-1], reverse=True)
        if len(sorted_file_paths) > 0:
            content_data = ocs_client.get_object(namespace, bucket_name, sorted_file_paths[0])
            if content_data.data:
                bytes_stream = BytesIO(content_data.data.content)
                df = pd.read_csv(bytes_stream,
                                 header=0,
                                 encoding="utf-8")
                max_chat_id = df["Chat ID"].unique().max()
                query = query.replace(CHAT_ID_PLACEHOLDER, str(max_chat_id))
        else:
            query = query.replace(f" and id > {CHAT_ID_PLACEHOLDER}", "")

        columns, rows = build_chat_data(report_name, query, is_report_present)
    else:
        logger.info(f"*** Doing Ingestion For  {report_name} ***")
        query = query.replace(f" and id > {CHAT_ID_PLACEHOLDER}", "")
        columns, rows = build_chat_data(report_name, query, is_report_present)

    logger.info(f"*** Completed Ingestion job at {datetime.now()} ***")
    return columns, rows
