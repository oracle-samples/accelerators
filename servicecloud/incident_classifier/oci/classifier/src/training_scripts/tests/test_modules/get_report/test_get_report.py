################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:26 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 0588e377b1b3e9ff243fa205bbdd6611831ffc20 $Id$
################################################################################################
#  File: test_get_report.py
################################################################################################
import logging

import requests
import oci
import jwt
import pytest
from datetime import datetime,timedelta
from pandas import DataFrame
from collections import namedtuple
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from training_scripts.constants import AIA_HIERARCHY_DATA_FROM_ROQL, INCIDENT_REPORT_NAME, ROQL_AI_INCIDENT_OFFSET, \
    ROQL_AI_INCIDENT_LIMIT, CX_DOMAIN, AUTH_TYPE, AUTH_TYPE_VALUE, INGESTION_SECRET_ID, JWT_TIME_EXPIRY_IN_MINUTE, \
    INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, INCIDENT_ID, THREAD_ID, BUCKET_URL
from training_scripts.jobs import get_report
from training_scripts.jobs.get_report import build_report, build_roql_data_frame, call_roql_api, invoke_roql_api, \
    read_secret_value, get_vault_client, get_secret_client, is_resource_principal_enabled, get_token, create_url, \
    build_and_store_report, find_first_thread, parse_env

URL_WITH_QUERY = "https://hostname.com/services/rest/connect/v1.3/queryResults?query="
DEFAULT_CONNECTION_TIMEOUT = 10.0
DEFAULT_READ_TIMEOUT = 60.0
DEFAULT_TIMEOUT = (DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)


class Content:
    content = "dGV4dHRvZW5jb2Rl"


class SecretBundleContent:
    id = 'test_id'
    secret_bundle_content = Content()


class OcsPath:
    ocs_bucket = "oci://<bucket>/"
    ocs_path = "/"


class OcsBucket:
    def is_directory(self, location: str):
        return lambda location: True

    def delete_object(self, object_name: str) -> None:
        return None

    def save_dataframe_as_csv(self, dataframe, object_name):
        return None


class Response:
    status_code = 200
    data = SecretBundleContent()

    def json(self):
        return {"items": [{"tableName": "Table0", "count": 1137,
                           "columnNames": ["Incident ID", "Reference #", "Subject", "Text", "Product ID",
                                           "Initial Product", "Category ID", "Initial Category", "Disposition ID",
                                           "Initial Disposition", "closedTime"], "rows": [
                ["1", "090724-000000", "Question goes here",
                 "Hi, my name is Dusty. How may I help you?\ ('Concluded by Agent').",
                 "1", "0", "8", "0", None, "0", None]]}]}


class ResponseReport:
    status_code = 200

    def json(self):
        return {"items": [{"tableName": "configurations", "count": 1, "rows": [[
            "[{ \"reportName\": \"aia_incidents\", \"reportQuery\": \"\"}, { \"reportName\": \"aia_disposition_hierarchy\", \"reportQuery\": \"\"}, { \"reportName\": \"aia_product_hierarchy\", \"reportQuery\": \"\"}, { \"reportName\": \"aia_category_hierarchy\", \"reportQuery\": \"\"}]"]]}]}


class VaultsClient(object):
    client = 'vault'

    def get_secret(self, secret_id):
        return SecretsClient()


class SecretsClient(object):
    client = 'secrets'
    data = SecretBundleContent()

    def get_secret_bundle(self, secret_id, **kwargs):
        return Response()


class FileInfo:
    name = "name"


def get_secret_bundle_mock(secret_id):
    return {"data": {"secret_bundle_content": {"content": "content"}}}


def build_roql_data_frame_mock(column_names, rows, report_name, report_query, record_position):
    return None


def build_roql_data_frame_mock_four_param(column_names, rows, report_name, report_query):
    return None


def find_first_thread_mock(columns, rows):
    return DataFrame(['tom', 'tom', 'tom'], [['tom', 'tom', 'tom'], ['tom', 'tom', 'tom'], ['tom', 'tom', 'tom']])


def invoke_roql_api_mock(url):
    return Response()


def invoke_roql_api_report_mock(url):
    return ResponseReport()


def build_data_frame_mock(name, column_names, rows, record_position=0):
    return None


def call_roql_api_mock(record_position, report_name, report_query):
    return Response()


def request_mock(method, url, headers, verify):
    return Response()


def set_up_ocs_connection_mock(ocs_bucket):
    return OcsBucket()


def get_recent_intents_files_mock(ocs_client, ocs_path):
    return [FileInfo()]


def test_build_report_from_roql(monkeypatch):
    monkeypatch.setenv(AIA_HIERARCHY_DATA_FROM_ROQL, AIA_HIERARCHY_DATA_FROM_ROQL)
    monkeypatch.setattr(get_report, "build_roql_data_frame", build_roql_data_frame_mock)
    monkeypatch.setattr(get_report, "build_data_frame", build_data_frame_mock)
    build_report("reportname", "query", True)


def test_build_report_not_from_roql(monkeypatch):
    monkeypatch.setenv(AIA_HIERARCHY_DATA_FROM_ROQL, "not roql")
    monkeypatch.setattr(get_report, "build_roql_data_frame", build_roql_data_frame_mock)
    monkeypatch.setattr(get_report, "build_data_frame", build_data_frame_mock)
    build_report("reportname", "query", True)


def test_build_roql_data_frame(monkeypatch, caplog):
    data = [['tom', 'tom', 'tom'], ['tom', 'tom', 'tom'], ['tom', 'tom', 'tom']]
    monkeypatch.setenv(INCIDENT_REPORT_NAME, INCIDENT_REPORT_NAME)
    monkeypatch.setenv(ROQL_AI_INCIDENT_OFFSET, 10)
    monkeypatch.setattr(get_report, "call_roql_api", call_roql_api_mock)
    monkeypatch.setattr(Response, "status_code", 200)
    monkeypatch.setattr(get_report, "build_roql_data_frame", build_roql_data_frame_mock)
    with caplog.at_level(logging.DEBUG):
        build_roql_data_frame(['Col1', 'Col2', 'Col3'], data, INCIDENT_REPORT_NAME, "query", 0)
        assert 'Record Position: and the total rows are : 4' in caplog.messages



def test_build_roql_data_frame_fail(monkeypatch, caplog):
    monkeypatch.setattr(get_report, "call_roql_api", call_roql_api_mock)
    monkeypatch.setattr(Response, "status_code", 400)
    with caplog.at_level(logging.DEBUG):
        build_roql_data_frame(None, None, INCIDENT_REPORT_NAME, "query", 0)
        assert "Error fetching report data: incident_report_name and the error is : "+str(Response().json()) in caplog.messages


def test_call_roql_api(monkeypatch):
    monkeypatch.setenv(INCIDENT_REPORT_NAME, INCIDENT_REPORT_NAME)
    monkeypatch.setenv(ROQL_AI_INCIDENT_LIMIT, 1000)
    monkeypatch.setattr(get_report, "create_url", lambda domain, path, query: URL_WITH_QUERY)
    monkeypatch.setattr(get_report, "invoke_roql_api", invoke_roql_api_mock)
    response_value = call_roql_api(10, INCIDENT_REPORT_NAME, "report_query")
    assert response_value.status_code == 200


def test_call_roql_api_other_report(monkeypatch):
    monkeypatch.setenv(INCIDENT_REPORT_NAME, "other")
    monkeypatch.setenv(CX_DOMAIN, CX_DOMAIN)
    monkeypatch.setattr(get_report, "create_url", lambda domain, path, query: URL_WITH_QUERY)
    monkeypatch.setattr(get_report, "invoke_roql_api", invoke_roql_api_mock)
    monkeypatch.setattr(requests, "request", request_mock)
    response_value = call_roql_api(10, INCIDENT_REPORT_NAME, "report_query")
    assert response_value.status_code == 200


def test_invoke_roql_api_oauth(monkeypatch):
    monkeypatch.setenv(AUTH_TYPE, AUTH_TYPE_VALUE)
    monkeypatch.setattr(get_report, "get_token", lambda: "sssklhjdh")
    monkeypatch.setattr(requests, "request", request_mock)
    response_value = invoke_roql_api(URL_WITH_QUERY)
    assert response_value.status_code == 200


def test_invoke_roql_api_basic(monkeypatch):
    monkeypatch.setenv(AUTH_TYPE, "basic")
    monkeypatch.setattr(get_report, "get_token", lambda: "sssklhjdh")
    monkeypatch.setattr(requests, "request", request_mock)
    response_value = invoke_roql_api(URL_WITH_QUERY)
    assert response_value.status_code == 200


def test_read_secret_value(monkeypatch):
    secrete_content = read_secret_value(SecretsClient(), "secret_id")
    assert secrete_content == 'texttoencode'


def test_get_vault_client(monkeypatch):
    monkeypatch.setattr(oci.auth.signers, 'get_resource_principals_signer', lambda: None)
    monkeypatch.setattr(oci.vault, 'VaultsClient', lambda config, signer: VaultsClient())
    monkeypatch.setattr(get_report, 'is_resource_principal_enabled', lambda: True)
    vaults_client = get_vault_client()
    assert vaults_client.client == 'vault'


def test_get_vault_client_config(monkeypatch):
    monkeypatch.setattr(oci.vault, 'VaultsClient', lambda config: VaultsClient())
    monkeypatch.setattr(get_report, 'is_resource_principal_enabled', lambda: False)
    vaults_client = get_vault_client()
    assert vaults_client.client == 'vault'


def test_get_secret_client(monkeypatch):
    monkeypatch.setattr(oci.auth.signers, 'get_resource_principals_signer', lambda: None)
    monkeypatch.setattr(oci.secrets, 'SecretsClient', lambda config, signer: SecretsClient())
    monkeypatch.setattr(get_report, 'is_resource_principal_enabled', lambda: True)
    secrets_client = get_secret_client()
    assert secrets_client.client == 'secrets'


def test_get_secret_client_config(monkeypatch):
    monkeypatch.setattr(oci.secrets, 'SecretsClient', lambda config: SecretsClient())
    monkeypatch.setattr(get_report, 'is_resource_principal_enabled', lambda: False)
    secrets_client = get_secret_client()
    assert secrets_client.client == 'secrets'


def test_is_resource_principal_enabled(monkeypatch):
    monkeypatch.setenv('OCI_RESOURCE_PRINCIPAL_VERSION', '2.2')
    result = is_resource_principal_enabled()
    assert result == True


def test_is_resource_principal_enabled(monkeypatch):
    result = is_resource_principal_enabled()
    assert result == False


def test_get_token_basic(monkeypatch):
    monkeypatch.setenv(INGESTION_SECRET_ID, 'INGESTION_SECRET_ID')
    vault_client = VaultsClient()
    monkeypatch.setattr(get_report, 'get_vault_client', lambda: vault_client)
    monkeypatch.setattr(vault_client, 'get_secret', vault_client.get_secret)
    monkeypatch.setattr(get_report, 'get_secret_client', lambda: SecretsClient())
    monkeypatch.setattr(get_report, 'read_secret_value', lambda secret_client, secret_id: "basis auth key")
    response = get_token()
    assert response == 'basis auth key'


def test_get_token_jwt(monkeypatch):
    monkeypatch.setenv(INGESTION_SECRET_ID, 'INGESTION_SECRET_ID')
    monkeypatch.setenv(JWT_TIME_EXPIRY_IN_MINUTE, 2)
    monkeypatch.setenv(AUTH_TYPE, 'OAUTH')
    vault_client = VaultsClient()
    monkeypatch.setattr(get_report, 'get_vault_client', lambda: vault_client)
    monkeypatch.setattr(vault_client, 'get_secret', vault_client.get_secret)
    monkeypatch.setattr(get_report, 'get_secret_client', lambda: SecretsClient())
    monkeypatch.setattr(get_report, 'read_secret_value', lambda secret_client, secret_id: "input key")
    monkeypatch.setattr(jwt, 'encode', lambda payload, key, algorithm, headers: "jwt token value")
    response = get_token()
    assert response == 'jwt token value'


def test_create_url(monkeypatch):
    url = create_url('cx.rightnow.com', '/', None)
    assert url == 'https://cx.rightnow.com/'


def test_build_report_aia_incidents(monkeypatch, caplog):
    monkeypatch.setenv(AIA_HIERARCHY_DATA_FROM_ROQL, AIA_HIERARCHY_DATA_FROM_ROQL)
    monkeypatch.setenv(INCIDENT_REPORT_NAME, INCIDENT_REPORT_NAME)
    monkeypatch.setenv(ROQL_AI_INCIDENT_OFFSET, 10)
    monkeypatch.setattr(get_report, "call_roql_api", call_roql_api_mock)
    monkeypatch.setattr(Response, "status_code", 200)
    with caplog.at_level(logging.DEBUG):
        build_report('aia_incidents', 'query', True)
        assert "Record Position: and the total rows are : 1" in caplog.messages



def test_build_and_store_report(monkeypatch, caplog):
    monkeypatch.setenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, '1')
    monkeypatch.setattr(get_report, "call_roql_api", call_roql_api_mock)
    monkeypatch.setenv(ROQL_AI_INCIDENT_OFFSET, 10)
    monkeypatch.setattr(Response, "status_code", 200)
    monkeypatch.setattr(get_report, "build_roql_data_frame", build_roql_data_frame_mock_four_param)
    monkeypatch.setattr(get_report, "find_first_thread", find_first_thread_mock)
    monkeypatch.setenv(INCIDENT_REPORT_NAME, INCIDENT_REPORT_NAME)
    monkeypatch.setattr(get_report, 'store_dataframe',
                        lambda ocs_client, ocs_path, report_data, dataframe, num_files_to_keep: None)
    with caplog.at_level(logging.DEBUG):
        build_and_store_report(INCIDENT_REPORT_NAME, 'query', True, None, None)
        date_to_fetch_data = datetime.now() - timedelta(days=1)
        assert '*** filtered data for date '+str(date_to_fetch_data.date())+' and found rows 3 ***' in caplog.messages


def test_build_and_store_report_bulk(monkeypatch, caplog):
    monkeypatch.setenv(INGESTION_JOB_INITIAL_DATA_FETCH_DAYS, '1')
    monkeypatch.setattr(get_report, "call_roql_api", call_roql_api_mock)
    monkeypatch.setenv(ROQL_AI_INCIDENT_OFFSET, 10)
    monkeypatch.setattr(Response, "status_code", 200)
    monkeypatch.setattr(get_report, "build_roql_data_frame", build_roql_data_frame_mock_four_param)
    monkeypatch.setattr(get_report, "find_first_thread", find_first_thread_mock)
    monkeypatch.setenv(INCIDENT_REPORT_NAME, INCIDENT_REPORT_NAME)
    monkeypatch.setattr(get_report, 'store_dataframe',
                        lambda ocs_client, ocs_path, report_data, dataframe, num_files_to_keep: None)
    with caplog.at_level(logging.DEBUG):
        date_to_fetch_data = datetime.now() - timedelta(days=1)
        build_and_store_report(INCIDENT_REPORT_NAME, 'query', False, None, None)
        assert '*** 0 : get data for date '+str(date_to_fetch_data.date())+' and file save complete ***' in caplog.messages



def test_find_first_thread(monkeypatch):
    columns = [INCIDENT_ID, THREAD_ID, 'SUBJECT']
    rows = [[1, 5, 'second'], [1, 3, 'first']]
    result = find_first_thread(columns, rows)
    assert result['SUBJECT'].item() == 'first'


def test_find_first_thread_empty_row(monkeypatch):
    columns = [INCIDENT_ID, THREAD_ID, 'SUBJECT']
    rows = []
    result = find_first_thread(columns, rows)
    assert result['SUBJECT'].empty


def test_fetch_report(monkeypatch, caplog):
    monkeypatch.setenv(CX_DOMAIN, 'cx.rightnow.com')
    data = {
        "bucket_url": "oci://<bucket>/<path>/"
    }
    monkeypatch.setenv(INCIDENT_REPORT_NAME, 'aia_incidents')
    monkeypatch.setattr(get_report, 'parse_env', lambda: namedtuple('Arguments', data.keys())(**data))
    monkeypatch.setattr(get_report, "invoke_roql_api", invoke_roql_api_report_mock)
    monkeypatch.setattr('training_scripts.jobs.get_report.parse_ocs_uri', lambda uri, report_type: OcsPath())
    ocs_client_mock = OcsBucket()
    monkeypatch.setattr('training_scripts.jobs.get_report.set_up_ocs_connection', lambda ocs_bucket: ocs_client_mock)
    monkeypatch.setattr(get_report, 'build_report',
                        lambda report_name, report_query, is_report_present: DataFrame(['header', 'header', 'header'],
                                                                                       [['row', 'row', 'row'], ]))
    monkeypatch.setattr(get_report, 'store_dataframe',
                        lambda ocs_client, ocs_path, report_data, dataframe, num_files_to_keep: None)
    monkeypatch.setattr(ocs_client_mock, "is_directory", ocs_client_mock.is_directory("oci://<bucket>/<path>/"))
    monkeypatch.setattr(get_report, 'build_and_store_report',
                        lambda report_name, report_query, is_report_present, ocs_client, ocs_path: None)
    with caplog.at_level(logging.DEBUG):
        get_report.fetch_report()
        assert "Connection with OCI Established" in caplog.messages

def test_store_dataframe(monkeypatch, caplog):
    monkeypatch.setattr('training_scripts.jobs.get_report.get_recent_intents_files', get_recent_intents_files_mock)
    with caplog.at_level(logging.DEBUG):
        get_report.store_dataframe(OcsBucket(), '/', "reportName_" +
                                   datetime.now().strftime('%Y-%m-%d'), None, num_files_to_keep=0)
        assert 'deleting 1 files' in caplog.messages


def test_store_dataframe_error(monkeypatch,caplog):
    with caplog.at_level(logging.DEBUG):

        get_report.store_dataframe(OcsBucket(), '/', "reportName_" +
                               datetime.now().strftime('%Y-%m-%d'), None, num_files_to_keep=0)
        assert '\'OcsBucket\' object has no attribute \'get_objects_with_prefix\'' in caplog.messages


def test_parse_env(monkeypatch):
    monkeypatch.setenv(BUCKET_URL, "oci://<bucket>/")
    result = parse_env()
    assert result is not None


def test_parse_env_error(monkeypatch):
    with pytest.raises(SystemExit) as pytest_wrapped_e:
        parse_env()
        assert pytest_wrapped_e.type == SystemExit
        assert pytest_wrapped_e.value.code == 42
