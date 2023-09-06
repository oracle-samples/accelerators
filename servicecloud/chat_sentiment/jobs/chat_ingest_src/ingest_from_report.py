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
#  SHA1: $Id: f27061c79d9983e2b572cc0e1d4d3410c5fd751c $
################################################################################################
#  File: ingest_from_report.py
################################################################################################

import sys
import logging
import json
import requests
import os
import concurrent.futures
from os.path import dirname, abspath
from datetime import datetime, timedelta

from common.constants import INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, LOGGING_FORMAT, CX_DOMAIN, \
    MAX_WORKER_FOR_REPORT_CALL, DATE_TIME_FORMAT
from common.utils import get_api_headers, create_url

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)
logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def build_data_frame(report_name, column_names, rows, start_date, end_date, record_position=0):
    """
        This method is used to get the hierarchy data using report api
        param column_names: name of report column.
        param rows: value of each rows.
        param record_position: this is used to set offset and limit
    """
    # Document Link:
    # https://docs.oracle.com/en/cloud/saas/b2c-service/22b/cxsvc/op-services-rest-connect-v1.4-analyticsreports-get.html

    is_loop = True
    while is_loop:

        url = create_url(os.getenv(CX_DOMAIN), '/services/rest/connect/v1.4/analyticsReportResults', '')
        logging.info(f" Retrieving  report from {url}")

        payload = json.dumps({
            "lookupName": report_name,
            "limit": 10000,
            "offset": record_position,
            "filters": [
                {
                    "name": "date",
                    "values": [
                        start_date,
                        end_date
                    ]
                }
            ]
        })
        response = post_call_api(payload, url)

        if response.status_code == 200:
            data = response.json()
            if data['count'] != 0:
                rows.extend(data['rows'])
                if len(column_names) == 0:
                    column_names.extend(data['columnNames'])
                logger.debug(f"Record Position: {record_position} and the total rows are : {len(rows)}")

                record_position = len(rows) + 1
            else:
                is_loop = False
        else:
            logger.error(f"Error fetching report data:  and the error is : {response.json()}")
            is_loop = False

    return rows, column_names


def post_call_api(payload, url):
    headers = get_api_headers()
    response = requests.request("POST", url, headers=headers, data=payload, verify=True)
    logger.info("Response received from post api call")
    return response


def populate_rows_from_report(report_name):
    columns = []
    rows = []
    futures = []
    tasks = []

    executor = concurrent.futures.ThreadPoolExecutor(max_workers=MAX_WORKER_FOR_REPORT_CALL)
    date_range = int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS))
    for counter in range(date_range):
        date_to_fetch_data = datetime.now() - timedelta(
            days=(date_range - counter))
        end_date = datetime.combine(date_to_fetch_data.date(), datetime.max.time()).strftime(DATE_TIME_FORMAT)
        start_date = datetime.combine(date_to_fetch_data.date(), datetime.min.time()).strftime(DATE_TIME_FORMAT)
        tasks.append((report_name, [], [], start_date, end_date))

    for task_args in tasks:
        future = executor.submit(build_data_frame, *task_args)
        futures.append(future)

    concurrent.futures.wait(futures)

    for future in concurrent.futures.as_completed(futures):
        try:
            result_row, result_column = future.result()
            if len(columns) == 0:
                columns = result_column
            rows.extend(result_row)
        except Exception as exc:
            logger.error('generated an exception: %s' % exc)

    executor.shutdown()
    return columns, rows
