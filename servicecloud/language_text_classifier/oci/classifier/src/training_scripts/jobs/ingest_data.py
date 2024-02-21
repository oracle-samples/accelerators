
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:00 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: e3151ca14971b31c256b46c4b393934faf6c8c21 $
################################################################################################
#  File: ingest_data.py
################################################################################################
####################################
# DataScience related resources
####################################

import base64
import json
from functools import lru_cache
from typing import Optional

import jwt
import logging
import os

import pandas as pd
import requests
import uuid
import sys
import oci

from oci.config import from_file
from oci.object_storage.models import CreateBucketDetails
from pandas import DataFrame
from datetime import datetime, timedelta
from os.path import dirname, abspath
from urllib.parse import urljoin

from tornado import concurrent

from constants import LOGGING_FORMAT, MAIN_SOURCE_BUCKET_LOCATION, CX_DOMAIN, AIA_INCIDENTS_REPORT_PATH, \
    AIA_REPORT_LIST_QUERY, INGESTION_SECRET_ID, AUTH_TYPE_VALUE, AUTH_TYPE, OAUTH_USER, \
    OAUTH_ENTITY, OAUTH_PATH, JWT_TIME_EXPIRY_IN_MINUTE, INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, COMPARTMENT_ID, ENCODING
from storage import get_object_storage_client, store_or_update_csv, get_csv_file_names, is_directory, \
    get_bucket_details, is_bucket_exists

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

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
        response = call_roql_api(record_position, "aia_incidents", report_query)
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

    filtered_incidents = filter_and_store_incidents(column_names, rows)
    if filtered_incidents.size > 0:
        logger.info(f"*** filtered data for date {day} and found rows {filtered_incidents.shape[0]} ***")
        store_dataframe(ocs_client, ocs_path, report_name + "_" + day.strftime('%Y-%m-%d'),
                        filtered_incidents,
                        num_files_to_keep=int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS)))


def call_roql_api(record_position, report_name, report_query):
    """
        this method is used to create the roql final query and call the invoke_roql_api method
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    query_value = report_query + ' LIMIT 1000 OFFSET ' + str(record_position)
    url = f'https://{os.getenv(CX_DOMAIN)}/services/rest/connect/v1.3/queryResults/?query={query_value}'
    return invoke_roql_api(url)


def invoke_roql_api(url):
    """
        This method has the rest api call to get roql data
        param url : url with roql query
    """
    headers = get_api_headers()
    response = requests.request("GET", url, headers=headers, verify=True)
    return response


def get_api_headers():
    token = get_token()

    headers = {
        'osvc-crest-application-context': f'{"Language Text Classification"}',
        'Authorization': f'{token}',
        'Content-Type': 'application/json'
    }
    return headers


@lru_cache(maxsize=10)
def read_secret_value(secret_client, secret_id):
    response = secret_client.get_secret_bundle(secret_id)
    base64_secret_content = response.data.secret_bundle_content.content
    base64_secret_bytes = base64_secret_content.encode('ascii')
    base64_message_bytes = base64.b64decode(base64_secret_bytes)
    secret_content = base64_message_bytes.decode('ascii')
    return secret_content


@lru_cache(maxsize=10)
def read_secret_original_content(secret_client, secret_id):
    response = secret_client.get_secret_bundle(secret_id)
    return response.data.secret_bundle_content.content


def get_vault_client():
    if is_resource_principal_enabled():
        signer = oci.auth.signers.get_resource_principals_signer()
        return oci.vault.VaultsClient({}, signer=signer)
    else:
        return oci.vault.VaultsClient(from_file())


def get_secret_client():
    if is_resource_principal_enabled():
        signer = oci.auth.signers.get_resource_principals_signer()
        return oci.secrets.SecretsClient({}, signer=signer)
    else:
        return oci.secrets.SecretsClient(from_file())


def is_resource_principal_enabled():
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        return True
    else:
        return False


@lru_cache(maxsize=1)
def get_secret():
    vault_client = get_vault_client()
    secret_id = vault_client.get_secret(
        secret_id=os.getenv(INGESTION_SECRET_ID)).data.id
    secret_client = get_secret_client()
    return secret_client, secret_id


def get_token():
    secret_client, secret_id = get_secret()
    if os.getenv(AUTH_TYPE) == AUTH_TYPE_VALUE:
        input_key = read_secret_value(secret_client, secret_id)
        key = "\n".join([l.lstrip() for l in input_key.split("\n")])
        dt = datetime.now() + timedelta(minutes=int(os.getenv(JWT_TIME_EXPIRY_IN_MINUTE, 2)))
        token = jwt.encode({
            "sub": os.getenv(OAUTH_USER),
            "iss": os.getenv(OAUTH_ENTITY),
            "exp": dt,
            "jti": str(uuid.uuid1()),
            "aud": os.getenv(OAUTH_PATH)
        }, key, "RS256", {
            "alg": "RS256",
            "type": "jwt"
        })
        logger.info("*** token retrieved ***")
        return 'Bearer ' + token
    else:
        input_key = read_secret_original_content(secret_client, secret_id)
        return 'Basic ' + input_key


def create_url(domain, path, query_params):
    """
        This method will create url for roql api to fetch report details
        param domain: base domain of cx.
        param path: uri path for the api.
        param query_params: query params for the api.
    """
    base = 'https://' + domain
    base = urljoin(base, path)
    url_query = urljoin(base, query_params)
    return url_query


def filter_and_store_incidents(columns, rows):
    filtered_incidents = find_first_thread(columns, rows)
    return filtered_incidents


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
    futures = []
    tasks = []
    executor = concurrent.futures.ThreadPoolExecutor(max_workers=max(os.cpu_count() - 1, 1))

    if is_report_present:
        date_to_fetch_data = datetime.now() - timedelta(days=1)
        report_query = report_query.replace("END_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                       date_to_fetch_data.max.time())))
        report_query = report_query.replace("START_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                         date_to_fetch_data.min.time())))
        tasks.append((ocs_client, ocs_path, report_name, date_to_fetch_data.date(), report_query))
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
            tasks.append((ocs_client, ocs_path, report_name, date_to_fetch_data.date(), query))

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

    configuration_url = create_url(os.getenv(CX_DOMAIN), AIA_INCIDENTS_REPORT_PATH,
                                   '?query=' + AIA_REPORT_LIST_QUERY)
    logger.info(f"configuration_url: {configuration_url}")

    # get the list of reports that needs to be ingested
    report_response = invoke_roql_api(configuration_url)
    logger.info(f"report_response status_code: {report_response.status_code}")

    if report_response.status_code == 200:
        data = report_response.json()
        if len(data["items"]) != 0 and (data["items"][0]["count"] != 0):
            report_list = json.loads(data["items"][0]["rows"][0][0])
            for report_data in report_list:
                report_name = report_data["reportName"]
                is_report_present = is_directory(bucket_location, ocs_client)
                logger.info(f"is_report_present: {is_report_present}")
                if is_report_present:
                    logger.info(f"*** Getting Feedback For  {report_name} ***")
                    build_and_store_report(report_name, report_data["feedbackQuery"],
                                           is_report_present, ocs_client, bucket_location)
                else:
                    logger.info(f"*** Doing Ingestion For  {report_name} ***")
                    build_and_store_report(report_name, report_data["reportQuery"],
                                           is_report_present, ocs_client, bucket_location)

            logger.info(f"*** Completed Ingestion job at {datetime.now()} ***")
    else:
        logger.info(f"!!! Configuration Error Code: {report_response.status_code}!!!")
        raise ValueError(f"!!! Configuration Error Code: {report_response.status_code}!!!")


def store_dataframe(ocs_client,
                    location: str,
                    filename: str,
                    df: DataFrame,
                    num_files_to_keep: Optional[int] = 0) -> None:
    try:
        if len(df) > 0:
            bucket_name, folder_name = get_bucket_details(location)
            namespace, recent_files = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=ocs_client)
            # keep last 30 files and delete others
            unwanted_files = recent_files[num_files_to_keep:]
            logger.info(f"deleting {len(unwanted_files)} files")
            for file in unwanted_files:
                ocs_client.delete_object(namespace, bucket_name, file)

            object_name = os.path.join(location, f"{filename}.csv")

            data = df.to_csv(encoding=ENCODING, index=False).encode(ENCODING)
            store_or_update_csv(object_name, data, ocs_client)
            logger.info(f"Uploaded {filename}")
        else:
            logger.info(f"Nothing to upload for {filename}")

    except Exception as err:
        logger.error(err)


if __name__ == '__main__':
    fetch_report()
