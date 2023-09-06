################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:48 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 39ec69ee289508de84f0559685610f106d2c3431 $
################################################################################################
#  File: mark_completed_chat_to_inactive.py
################################################################################################

import sys
import os
import json
import logging
import requests
import schedule
import time

from os.path import dirname, abspath
from pandas import DataFrame

from common.constants import CX_DOMAIN
from common.utils import get_api_headers

LOGGING_FORMAT = '[%(levelname)s] %(asctime)s %(name)s:%(lineno)d - %(message)s'
base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)
logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))

OAUTH_USER = "OAUTH_USER"
OAUTH_ENTITY = "OAUTH_ENTITY"
OAUTH_PATH = "OAUTH_PATH"
INGESTION_SECRET_ID = "INGESTION_SECRET_ID"
AUTH_TYPE = "AUTH_TYPE"
AUTH_TYPE_VALUE = "OAUTH"
JOB_INTERVAL_IN_MIN = "JOB_INTERVAL_IN_MIN"
JWT_TIME_EXPIRY_IN_MINUTE = "JWT_TIME_EXPIRY_IN_MINUTE"


def get_report_response(column_names, rows, payload):
    """
        This method is used to get the report data using report api
        param column_names: name of report column.
        param rows: value of each rows.
        param payload: this is used to set payload of request
    """
    # Document Link:
    # https://docs.oracle.com/en/cloud/saas/b2c-service/22b/cxsvc/op-services-rest-connect-v1.4-analyticsreports-get.html

    url = f"https://{os.getenv(CX_DOMAIN)}/services/rest/connect/v1.4/analyticsReportResults"

    logging.info(f" Retrieving report from {url}")
    response = post_call_api(payload, url)

    if response.status_code == 200:
        data = response.json()
        if data['count'] != 0:
            rows.extend(data['rows'])
            if len(column_names) == 0:
                column_names.extend(data['columnNames'])
            logger.debug(f"Record Position: 0 and the total rows are : {len(rows)}")
    else:
        logger.error(f"Error fetching report data: and the error status is : {response.status_code}")
        logger.error(f"Error fetching report data: and the error content is : {response.content}")


def post_call_api(payload, url):
    headers = get_api_headers()
    response = requests.request("POST", url, headers=headers, data=payload, verify=True)
    logger.info("Response received from post api call")
    return response


def update_parent(parent_id, completed_date):
    url = f"https://{os.getenv(CX_DOMAIN)}/services/rest/connect/v1.4/AIML.ChatAIResultSummary/{parent_id}"
    payload = json.dumps({
        "IsActive": False,
        "CompletedTime": completed_date
    })
    response = patch_call_api(payload, url)
    if response.status_code == 200:
        logger.info('CHAT_STATUS_UPDATE_JOB success 200 in update parent table call id :' + parent_id)
        return response
    else:
        logger.error('CHAT_STATUS_UPDATE_JOB update parent table call id :' + parent_id + ' error response')


def patch_call_api(payload, url):
    headers = get_api_headers()
    response = requests.request("PATCH", url, headers=headers, data=payload, verify=True)
    return response


def update_inactive_chats():
    try:
        columns_chats_table = []
        rows_chats_table = []
        columns_parent_table = []
        row_parent_table = []

        payload = json.dumps({
            "lookupName": 'CheckForParentRecord',
            "limit": 10000,
            "offset": 0,
            "filters": [{
                "name": "Active",
                "values": ["All"]
            }]
        })

        logger.info('CHAT_STATUS_UPDATE_JOB get active chats from parent table :')
        get_report_response(columns_parent_table, row_parent_table, payload)
        payload = json.dumps({
            "lookupName": 'GetCompletedChats',
            "limit": 10000,
            "offset": 0
        })
        logger.info('CHAT_STATUS_UPDATE_JOB get completed chats from chats table :')
        get_report_response(columns_chats_table, rows_chats_table, payload)
        active_chats = DataFrame(columns=columns_parent_table, data=row_parent_table)

        completed_chats = DataFrame(columns=columns_chats_table, data=rows_chats_table)
        if (len(active_chats) > 0) and (len(completed_chats) > 0):
            unique_active_chats = set(active_chats['ChatId'])
            unique_completed_chats = set(completed_chats['ChatId'])
            list_of_chats_to_update = unique_active_chats.intersection(unique_completed_chats)
            for chat_id in list_of_chats_to_update:
                row_to_update = active_chats.loc[active_chats['ChatId'] == chat_id]['ID']
                row_from_chat = completed_chats.loc[completed_chats['ChatId'] == chat_id]['Date of Chat Session Completion']
                parent_id = row_to_update.values[0]
                completed_date = row_from_chat.values[0]
                logger.info('CHAT_STATUS_UPDATE_JOB before update parent table call id :' + parent_id)
                update_parent(parent_id, completed_date)
    except Exception as error:
        logger.info(f'CHAT_STATUS_UPDATE_JOB Error while job run  {error}')



def check_environment_variables(variables):
    for var in variables:
        value = os.getenv(var)
        if value is None:
            return False
    return True


if __name__ == '__main__':
    logger.info('CHAT_STATUS_UPDATE_JOB inside main')
    env_vars = ['CX_DOMAIN', 'OAUTH_USER', 'OAUTH_ENTITY', 'OAUTH_PATH', 'INGESTION_SECRET_ID', 'AUTH_TYPE',
                'JOB_INTERVAL_IN_MIN']
    schedule.every(int(os.getenv(JOB_INTERVAL_IN_MIN))).minutes.do(update_inactive_chats)
    if check_environment_variables(env_vars):
        while True:
            time.sleep(0.005)
            schedule.run_pending()
            time.sleep(60)
    else:
        logger.error("""CHAT_STATUS_UPDATE_JOB Does not have all the env variable set CX_DOMAIN, OAUTH_USER,
         OAUTH_ENTITY, OAUTH_PATH, INGESTION_SECRET_ID,AUTH_TYPE, JOB_INTERVAL_IN_MIN""")
