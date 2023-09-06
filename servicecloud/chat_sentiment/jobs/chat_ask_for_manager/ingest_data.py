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
#  SHA1: $Id: d1a586b324f898b77714edba4db491a3629318cb $
################################################################################################
#  File: ingest_data.py
################################################################################################

import base64
from os.path import dirname, abspath

import logging
import os

import pandas as pd

from oci.object_storage.models import CreateBucketDetails
from pandas import DataFrame
from datetime import datetime, timedelta

from typing import Optional
from tornado import concurrent
from common.constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, CX_DOMAIN, COMPARTMENT_ID, \
    DATE_FORMAT, INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, MANAGER_ASK_FEEDBACK_QUERY
from common.utils import data_to_csv, invoke_roql_api
from common.storage import get_object_storage_client, store_or_update_csv, get_csv_file_names, is_directory, \
    get_bucket_details, is_bucket_exists

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

DEFAULT_ENCODING = 'utf-8'


def build_roql_data_frame(ocs_client, ocs_path, report_name, day, report_query, record_position=0):
    """
        This method will use ROQL query to get aia_incidents report data when thread masking is enabled.
        param column_names: name of report column.
        param rows: value of each rows.
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    column_names = []
    rows = []
    is_loop = True
    while is_loop:
        response = call_roql_api(record_position, report_name, report_query)
        if response.status_code == 200:
            data = response.json()
            if len(data["items"]) != 0 and (data["items"][0]["count"] != 0):
                for item in data["items"]:
                    rows.extend(item["rows"])
                    if len(column_names) == 0:
                        column_names.extend(item['columnNames'])
                    logger.debug(f"Record Position: and the total rows are : {len(rows)}")

                record_position = record_position + 1000
            else:
                is_loop = False
        else:
            logger.error(f"Error fetching report data: aia_incidents and the error is : {response.json()}")
            is_loop = False

    logger.info(
        f"***  : get data for date {day} and found rows {len(rows)} ***")

    filtered_incidents = DataFrame(columns=column_names, data=rows)
    if filtered_incidents.size > 0:
        logger.info(f"*** filtered data for date {day} and found rows {filtered_incidents.shape[0]} ***")
        store_dataframe(ocs_client, ocs_path, report_name + "_" + day.strftime(DATE_FORMAT),
                        filtered_incidents,
                        num_files_to_keep=int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, 365)))


def call_roql_api(record_position, report_name, report_query):
    """
        this method is used to create the roql final query and call the invoke_roql_api method
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    logger.info(f"*** Getting Data For {report_name} ***")
    query_value = report_query + ' LIMIT 1000 OFFSET ' + str(record_position)
    url = f'https://{os.getenv(CX_DOMAIN)}/services/rest/connect/v1.4/queryResults/?query={query_value}'
    return invoke_roql_api(url)


def build_and_store_report(report_name, report_query, is_report_present, ocs_client, ocs_path):
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
    if is_report_present:
        futures = []
        tasks = []
        executor = concurrent.futures.ThreadPoolExecutor(max_workers=max(os.cpu_count() - 1, 1))
        date_to_fetch_data = datetime.now() - timedelta(days=1)
        report_query = report_query.replace("END_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                       date_to_fetch_data.max.time())))
        report_query = report_query.replace("START_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                         date_to_fetch_data.min.time())))
        tasks.append((ocs_client, ocs_path, report_name, date_to_fetch_data.date(), report_query))

        for filter_task_args in tasks:
            future = executor.submit(build_roql_data_frame, *filter_task_args)
            futures.append(future)

        concurrent.futures.wait(futures)

        for future in concurrent.futures.as_completed(futures):
            try:
                future.result()
            except Exception as exc:
                logger.error('generated an exception: %s' % exc)

        executor.shutdown()
    else:
        file_name = "ai_ml_TicketData_train_enhanced.csv"
        csv_file_path = os.path.join(dirname(abspath(__file__)), file_name)
        initial_file = pd.read_csv(csv_file_path)
        store_dataframe(ocs_client, ocs_path, report_name + "_" + datetime.now().strftime(DATE_FORMAT), initial_file)

    logger.info("*** All Ingestion Threads Complete ***")


def find_first_thread(columns, rows):
    """
        loop through the records and filter out only the incident rows with first thread
        param columns: headers
        param rows: records without having all the incidents
    """
    df = DataFrame(columns=columns, data=rows)
    if "Thread Id" in columns:
        if df.shape[0] > 0:
            df["Thread Id"] = df["Thread Id"].astype('int')
            unique_incidents = set(df["Incident ID"])
            filtered_incidents = DataFrame(columns=columns)
            for incident_id in unique_incidents:
                result_df = df.loc[df["Incident ID"] == incident_id]
                result_df = result_df[result_df["Thread Id"] == result_df["Thread Id"].min()]
                filtered_incidents = pd.concat([filtered_incidents, result_df])
            return filtered_incidents
        else:
            return df
    else:
        return df


def fetch_report():
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    ocs_client = get_object_storage_client()
    logger.info("Connection with OCI Established")
    bucket_location = os.getenv(MAIN_SOURCE_BUCKET_LOCATION)
    bucket_name, folder_name = get_bucket_details(bucket_location)
    if not is_bucket_exists(bucket_name, ocs_client):
        namespace = ocs_client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
        ocs_client.create_bucket(namespace_name=namespace,
                                 create_bucket_details=CreateBucketDetails(name=bucket_name,
                                                                           compartment_id=os.getenv(COMPARTMENT_ID)))
    report_name = "aia_supervisor_ask"
    is_report_present = is_directory(bucket_location, ocs_client)
    logger.info(f"is_report_present: {is_report_present}")
    logger.info(f"*** Doing Ingestion For  {report_name} ***")
    build_and_store_report(report_name, MANAGER_ASK_FEEDBACK_QUERY,
                           is_report_present, ocs_client, bucket_location)
    logger.info(f"*** Completed Ingestion job at {datetime.now()} ***")


def store_dataframe(ocs_client,
                    location: str,
                    filename: str,
                    df: DataFrame,
                    num_files_to_keep: Optional[int] = 0) -> None:
    try:
        bucket_name, folder_name = get_bucket_details(location)
        namespace, recent_files = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=ocs_client)
        # keep last 30 files and delete others
        unwanted_files = recent_files[num_files_to_keep:]
        logger.info(f"deleting {len(unwanted_files)} files")
        for file in unwanted_files:
            ocs_client.delete_object(namespace, bucket_name, file)

        object_name = os.path.join(location, f"{filename}.csv")
        data = data_to_csv(df)
        store_or_update_csv(object_name, data, ocs_client)
        logger.info(f"Uploaded {filename}")
    except Exception as err:
        logger.error(err)


if __name__ == '__main__':
    fetch_report()
