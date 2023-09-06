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
#  SHA1: $Id: 202e0e76bfcff016bfe6cb9bd7d0bd55f94b1dcd $
################################################################################################
#  File: utils.py
################################################################################################
import base64
import csv
import io
import logging
import os
import sys
import time
import uuid
from functools import lru_cache
from urllib.parse import urljoin

import jwt
import oci
import pandas as pd
import requests
from oci.config import from_file
from oci.object_storage import ObjectStorageClient
from datetime import datetime, timedelta
from common.constants import LOGGING_FORMAT, DEFAULT_ONE_MINUTE_TIMEOUT, LANGUAGE_SERVICE_MAX_CHAT_TEXT_LIMIT, \
    TEXT_COLUMN, DEFAULT_ENCODING, OAUTH_PATH, OAUTH_ENTITY, OAUTH_USER, JWT_TIME_EXPIRY_IN_MINUTE, AUTH_TYPE_VALUE, \
    AUTH_TYPE, INGESTION_SECRET_ID
from common.storage import delete_folder, store_or_update_csv

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


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
    return os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None


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


def get_language_client():
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        logger.info("*** AIServiceLanguageClient: Using Resource Principal for Authentication ***")
        signer = oci.auth.signers.get_resource_principals_signer()
        return oci.ai_language.AIServiceLanguageClient(config={}, signer=signer)
    else:
        logger.info("*** AIServiceLanguageClient: Using local config file for Authentication ***")
        config = oci.config.from_file()
        return oci.ai_language.AIServiceLanguageClient(config)


def data_to_csv(df: pd.DataFrame):
    output = io.StringIO()
    writer = csv.DictWriter(output, fieldnames=df.columns)
    writer.writeheader()

    rows = []
    for index, row in df.iterrows():
        row_dict = {}
        for column, value in row.items():
            if type(value) == str:
                if value is not None:
                    row_dict[column] = value.encode('latin-1', errors='replace').decode("utf-8", errors="ignore")
                else:
                    row_dict[column] = value
            else:
                row_dict[column] = value

        rows.append(row_dict)

    writer.writerows(rows)
    csv_string = output.getvalue()
    output.close()
    return csv_string


def wait_for_resource_status(client,
                             resource_id,
                             resource_type,
                             status,
                             opc_request_id="rnow-accelerator",
                             start_time=time.time(), timeout=DEFAULT_ONE_MINUTE_TIMEOUT):
    if client is None:
        client = get_language_client()

    get_func = getattr(client, f"get_{resource_type}")
    response = get_func(resource_id, opc_request_id=opc_request_id)

    diff = time.time() - start_time
    while diff < timeout:
        if response.data.lifecycle_state == status:
            logger.info(f"*** GOT STATUS TO BE {status} After {diff} seconds ***")
            return
        else:
            logger.info(
                f"*** CURRENT STATUS:{response.data.lifecycle_state} "
                f"| WAITING FOR: {status} "
                f"| TIMEOUT: {round(diff, 2)}/{timeout} seconds ***")
            recursion_limit = sys.getrecursionlimit()
            sleep_seconds = 1 if (timeout / recursion_limit) < 1 else int(timeout / recursion_limit)
            time.sleep(sleep_seconds)
            return wait_for_resource_status(client, resource_id, resource_type,
                                            status, opc_request_id, start_time,
                                            timeout)
    raise TimeoutError("Resource status did not change to {} after {} seconds".format(status, timeout))


def upload_rectified_data(df_first_200k: pd.DataFrame, location: str, client: ObjectStorageClient):
    if len(df_first_200k) > 0:
        df_first_200k[TEXT_COLUMN] = df_first_200k[TEXT_COLUMN].astype(str)
        df_first_200k[TEXT_COLUMN] = df_first_200k[TEXT_COLUMN].apply(
            lambda text: text[:LANGUAGE_SERVICE_MAX_CHAT_TEXT_LIMIT])
        data = df_first_200k.to_csv(encoding=DEFAULT_ENCODING, index=False).encode(DEFAULT_ENCODING)
        delete_folder(location, client)
        rectified_data_path = os.path.join(location, f"data_{datetime.now().strftime('%Y-%m-%d')}.csv")
        store_or_update_csv(rectified_data_path, data, client)
        logger.info(f"*** Rectified Job Complete and Stored data in {rectified_data_path} ***")
    else:
        logger.info(f"*** Rectified Job Complete Due To No Data Rectified ***")
