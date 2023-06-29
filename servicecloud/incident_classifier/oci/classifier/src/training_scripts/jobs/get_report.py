################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:25 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 57ceb28722da4d07a3a4b0ca6ea24da7e64101ad $
################################################################################################
#  File: get_report.py
################################################################################################
import ads
import base64
import json
import jwt
import logging
import os
import requests
import uuid
import sys
import oci

from oci.config import from_file
from pandas import DataFrame
from typing import Optional
from datetime import datetime, timedelta
from os.path import dirname, abspath

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

from ai4service_automated_classification.ml.util.data_util import get_recent_intents_files
from training_scripts.constants import LOGGING_FORMAT, BUCKET_URL, INGESTION_JOB_REPEAT_INTERVAL_IN_HOUR, \
    AIA_INCIDENTS_ANALYTICS_REPORT_PATH, CX_DOMAIN, INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, \
    AIA_INCIDENTS_REPORT_PATH, ROQL_AI_INCIDENT_OFFSET, ROQL_AI_INCIDENT_LIMIT, AIA_REPORT_LIST_QUERY, \
    INCIDENT_REPORT_NAME, AIA_HIERARCHY_DATA_FROM_ROQL, YEAR_MONTH_DATE_FORMAT, INCIDENT_ID, THREAD_ID, \
    OAUTH_USER, OAUTH_ENTITY, OAUTH_PATH, INGESTION_SECRET_ID, AUTH_TYPE, AUTH_TYPE_VALUE, JWT_TIME_EXPIRY_IN_MINUTE, \
    INCIDENT_CLASSIFIER

from ai4service_automated_classification.utils.object_storage.bucket_vault import set_up_ocs_connection
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import parse_ocs_uri, _validate_timestamp
from collections import namedtuple
from urllib.parse import urljoin
from training_scripts.utils import set_config

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def build_data_frame(name, column_names, rows, record_position=0):
    """
        This method is used to get the hierarchy data using report api
        param column_names: name of report column.
        param rows: value of each rows.
        param record_position: this is used to set offset and limit
    """
    # Document Link:
    # https://docs.oracle.com/en/cloud/saas/b2c-service/22b/cxsvc/op-services-rest-connect-v1.4-analyticsreports-get.html
    logging.basicConfig(level="DEBUG", format=LOGGING_FORMAT)

    if not os.getenv(CX_DOMAIN):
        raise RuntimeError("Please configure your domain in environment variable CX_DOMAIN.")

    url = "https://" + os.getenv(CX_DOMAIN) + AIA_INCIDENTS_ANALYTICS_REPORT_PATH

    logging.info(f" Retrieving {name} report from {url}")

    payload = json.dumps({
        "lookupName": name,
        "limit": 100,
        "offset": record_position
    })

    headers = get_api_headers()

    if "dq.lan" in url:
        response = requests.request("POST", url, headers=headers, data=payload, verify=False)
    else:
        response = requests.request("POST", url, headers=headers, data=payload, verify=True)

    if response.status_code == 200:
        data = response.json()
        if data['count'] != 0:
            rows.extend(data['rows'])
            if len(column_names) == 0:
                column_names.extend(data['columnNames'])
            logger.debug(f"Record Position: {record_position} and the total rows are : {len(rows)}")
            record_position = len(rows) + 1
            build_data_frame(name, column_names, rows, record_position)
    else:
        logger.error(f"Error fetching report data: {name} and the error is : {response.json()}")


def build_roql_data_frame(column_names, rows, report_name, report_query, record_position=0):
    """
        This method will use ROQL query to get aia_incidents report data when thread masking is enabled.
        param column_names: name of report column.
        param rows: value of each rows.
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    logging.basicConfig(level="DEBUG", format=LOGGING_FORMAT)
    response = call_roql_api(record_position, report_name, report_query)

    if response.status_code == 200:
        data = response.json()
        if len(data["items"]) != 0 and (data["items"][0]["count"] != 0):
            for item in data["items"]:
                rows.extend(item["rows"])
                if len(column_names) == 0:
                    column_names.extend(item['columnNames'])
                logger.debug(f"Record Position: and the total rows are : {len(rows)}")
            if os.getenv(INCIDENT_REPORT_NAME) == report_name.casefold():
                build_roql_data_frame(column_names, rows, report_name, report_query,
                                      record_position + int(os.getenv(ROQL_AI_INCIDENT_OFFSET)))
    else:
        logger.error(f"Error fetching report data: {report_name} and the error is : {response.json()}")


def call_roql_api(record_position, report_name, report_query):
    """
        this method is used to create the roql final query and call the invoke_roql_api method
        param report_name: current report to be fetched
        param report_query: query for current report
        param record_position: this is used to set offset and limit
    """
    query_value = ""
    if os.getenv(INCIDENT_REPORT_NAME) == report_name.casefold():
        query_value = query_value + report_query + ' LIMIT ' + os.getenv(ROQL_AI_INCIDENT_LIMIT) + ' OFFSET ' + str(
            record_position)
    else:
        query_value = report_query
    url = create_url(os.getenv(CX_DOMAIN), AIA_INCIDENTS_REPORT_PATH,
                     '?query=' + query_value)
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
    if os.getenv(AUTH_TYPE) == AUTH_TYPE_VALUE:
        token = 'Bearer ' + get_token()
    else:
        token = 'Basic ' + get_token()

    headers = {
        'osvc-crest-application-context': f'{INCIDENT_CLASSIFIER}',
        'Authorization': f'{token}',
        'Content-Type': 'application/json'
    }
    return headers


def read_secret_value(secret_client, secret_id):
    response = secret_client.get_secret_bundle(secret_id)
    base64_secret_content = response.data.secret_bundle_content.content
    base64_secret_bytes = base64_secret_content.encode('ascii')
    base64_message_bytes = base64.b64decode(base64_secret_bytes)
    secret_content = base64_message_bytes.decode('ascii')
    return secret_content


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


def get_token():
    vault_client = get_vault_client()
    secret_id = vault_client.get_secret(
        secret_id=os.getenv(INGESTION_SECRET_ID)).data.id
    logger.info("Connection with vault complete and secret id found")
    secret_client = get_secret_client()
    input_key = read_secret_value(secret_client, secret_id)
    logger.info("Connection with secret_client complete and input_key found")
    if os.getenv(AUTH_TYPE) == AUTH_TYPE_VALUE:
        key = "\n".join([l.lstrip() for l in input_key.split("\n")])
        dt = datetime.now() + timedelta(minutes=int(os.getenv(JWT_TIME_EXPIRY_IN_MINUTE)))
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
        logger.info("token created")
        return token
    else:
        return input_key


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


def build_report(report_name, report_query, is_report_present):
    columns = []
    rows = []
    if os.getenv(AIA_HIERARCHY_DATA_FROM_ROQL):
        build_roql_data_frame(columns, rows, report_name, report_query, is_report_present)
    else:
        build_data_frame(report_name, columns, rows)

    return create_hierarchy(columns, rows)


def create_hierarchy(columns, rows):
    """
        This method has custom logic to create hierarchy level 4 and 5 for roql response
        param columns: headers
        param rows: data
    """
    df = DataFrame(columns=columns, data=rows)
    for index, rows in df.iterrows():
        if df.iloc[index, 0] is None:
            df.iloc[index, 4] = df.iloc[index, 2]
        else:
            for item_index, item in df.iloc[index, 3:][df.iloc[index, 3:].isna()].iteritems():
                df.iloc[index][item_index] = df.iloc[index, 0]
                df.iloc[index][df.iloc[index, 3:][df.iloc[index, 3:].isna()].head(1).index] = df.iloc[index, 1]
                break
    return df.iloc[:, 2:]


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
    columns = []
    rows = []

    if os.getenv(INCIDENT_REPORT_NAME) == report_name.casefold():
        if is_report_present:
            date_to_fetch_data = datetime.now() - timedelta(days=1)
            report_query = report_query.replace("END_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                           date_to_fetch_data.max.time())))
            report_query = report_query.replace("START_DATE", str(date_to_fetch_data.combine(date_to_fetch_data,
                                                                                             date_to_fetch_data.min.time())))
            build_roql_data_frame(columns, rows, report_name, report_query)
            logger.info(f"*** get data for date {date_to_fetch_data.date()} and found rows {len(rows)} ***")
            filtered_incidents = find_first_thread(columns, rows)
            if filtered_incidents.size > 0:
                logger.info(
                    f"*** filtered data for date {date_to_fetch_data.date()} and found rows {filtered_incidents.shape[0]} ***")
                store_dataframe(ocs_client, ocs_path, report_name + "_" + date_to_fetch_data.strftime('%Y-%m-%d'),
                                filtered_incidents,
                                num_files_to_keep=int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS)))
        else:
            date_range = int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS))
            for counter in range(date_range):
                query = ""
                rows = []
                date_to_fetch_data = datetime.now() - timedelta(
                    days=(float(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS)) - counter))
                query = query + report_query.replace("END_DATE", str(datetime.combine(date_to_fetch_data.date(),
                                                                                      datetime.max.time())))
                query = query.replace("START_DATE",
                                      str(datetime.combine(date_to_fetch_data.date(), datetime.min.time())))
                build_roql_data_frame(columns, rows, report_name, query)
                logger.info(
                    f"*** {counter} : get data for date {date_to_fetch_data.date()} and found rows {len(rows)} ***")
                filtered_incidents = find_first_thread(columns, rows)
                if filtered_incidents.size > 0:
                    logger.info(
                        f"*** filtered data for date {date_to_fetch_data.date()} and found rows {filtered_incidents.shape[0]} ***")
                    store_dataframe(ocs_client, ocs_path,
                                    report_name + "_" + date_to_fetch_data.date().strftime(YEAR_MONTH_DATE_FORMAT),
                                    filtered_incidents,
                                    num_files_to_keep=int(os.getenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS)))
                    logger.info(
                        f"*** {counter} : get data for date {date_to_fetch_data.date()} and file save complete ***")


def find_first_thread(columns, rows):
    """
        loop through the records and filter out only the incident rows with first thread
        param columns: headers
        param rows: records without having all the incidents
    """
    df = DataFrame(columns=columns, data=rows)
    if df.shape[0] > 0:
        df[THREAD_ID] = df[THREAD_ID].astype('int')
        unique_incidents = set(df[INCIDENT_ID])
        filtered_incidents = DataFrame(columns=columns)
        for incident_id in unique_incidents:
            result_df = df.loc[df[INCIDENT_ID] == incident_id]
            result_df = result_df[result_df[THREAD_ID] == result_df[THREAD_ID].min()]
            filtered_incidents = filtered_incidents.append(result_df)
        return filtered_incidents
    else:
        return df


def prepare_csv(json_report, output_path):
    df = DataFrame(json_report)
    return df.to_csv(output_path, )


def parse_env():
    """Parse env to dictionary.

    :return: A named tuple with the different job arguments.
    """

    bucket_storage_url = os.getenv(BUCKET_URL)
    if bucket_storage_url is None:
        raise SystemExit(f"Bucket location required for storing the data. Please set {BUCKET_URL} in env")
    data = {
        "bucket_url": bucket_storage_url
    }
    return namedtuple('Arguments', data.keys())(**data)


def fetch_report():
    logger.info(f"*** Scheduled the job at {datetime.now()} ***")
    args = parse_env()
    url = create_url(os.getenv(CX_DOMAIN), AIA_INCIDENTS_REPORT_PATH,
                     '?query=' + AIA_REPORT_LIST_QUERY)
    # get the list of reports that needs to be ingested
    report_list = invoke_roql_api(url)
    if report_list.status_code == 200:
        data = report_list.json()
        if len(data["items"]) != 0 and (data["items"][0]["count"] != 0):
            report_list_data = json.loads(data["items"][0]["rows"][0][0])
            for report_data in report_list_data:
                report_name = report_data["reportName"]
                data_path = parse_ocs_uri(args.bucket_url, report_name)
                logger.info(f'Setting up OCI Connection: {data_path}...')
                ocs_client = set_up_ocs_connection(data_path.ocs_bucket)
                is_report_present = ocs_client.is_directory(data_path.ocs_path)
                logger.info("Connection with OCI Established")
                if os.getenv(INCIDENT_REPORT_NAME) == report_name.casefold():
                    build_and_store_report(report_name, report_data["reportQuery"], is_report_present,
                                           ocs_client, data_path.ocs_path)
                else:
                    incident_data = build_report(report_name, report_data["reportQuery"],
                                                 is_report_present)
                    logger.info(f"*** {report_name} : data is fetched and have {incident_data.shape[0]} rows ***")
                    store_dataframe(ocs_client, data_path.ocs_path, report_data["reportName"] + "_" +
                                    datetime.now().strftime('%Y-%m-%d'), incident_data, num_files_to_keep=0)
                logger.info(f"*** Completed Scheduled job at {datetime.now()} ***")


def store_dataframe(ocs_client: OcsBucket,
                    ocs_path: str,
                    report_data: str,
                    dataframe: DataFrame,
                    num_files_to_keep: Optional[int] = 0,
                    timestamp: Optional[float] = None) -> None:
    """
    Store the dataframe as a CSV file in ocs bucket on given location path.

    @param ocs_client: connected instance of OCI bucket.
    @param ocs_path: location where CSV file would be stored
    @param report_data: type of the report to be fetched. e.g, incident,product,etc.
    @param dataframe: dataframe containing the intents.
    @param num_files_to_keep: to keep first n files and delete others.
    @param timestamp: to save the CSV file with the given timestamp.
                      (Optional) By default, it will use current.
    """
    try:
        recent_files = get_recent_intents_files(ocs_client, ocs_path)
        # keep last 30 files and delete others
        unwanted_files = recent_files[num_files_to_keep:]
        logger.info(f"deleting {len(unwanted_files)} files")
        for file in unwanted_files:
            ocs_client.delete_object(os.path.join(ocs_path, file.name))
        timestamp = _validate_timestamp(timestamp)
        object_name = os.path.join(ocs_path, f"{report_data}-{round(timestamp)}.csv")
        ocs_client.save_dataframe_as_csv(dataframe, object_name)
        logger.info(f"Uploaded {object_name}")
    except Exception as err:
        logger.error(err)


if __name__ == '__main__':
    set_config()
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        ads.set_auth(auth='resource_principal')
    fetch_report()
