
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2024, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 24B (February 2024) 
#  date: Thu Feb 15 09:26:01 IST 2024
 
#  revision: rnw-24-02-initial
#  SHA1: $Id: 0f0efe641256bc4699b682dd7feddb32021b9610 $
################################################################################################
#  File: utils.py
################################################################################################
####################################
# DataScience related resources
####################################

import csv
import io
import logging
import os
import sys
import time
import re
import oci
import pandas as pd
import schedule

from constants import DEFAULT_ONE_MINUTE_TIMEOUT, LOGGING_FORMAT

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


def get_name(resource, location):
    if resource == "bucket":
        return re.match(r"oci://(.+?)/", location).group(1)
    elif resource == "folder":
        return re.match(r"oci://[^/]+/(.*)", location).group(1)


def get_object_storage_client():
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        logger.info("*** ObjectStorageClient: Using Resource Principal for Authentication ***")
        signer = oci.auth.signers.get_resource_principals_signer()
        return oci.object_storage.ObjectStorageClient(config={}, signer=signer)
    else:
        logger.info("*** ObjectStorageClient: Using local config file for Authentication ***")
        config = oci.config.from_file()
        return oci.object_storage.ObjectStorageClient(config)


def get_language_client():
    if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
        logger.info("*** AIServiceLanguageClient: Using Resource Principal for Authentication ***")
        signer = oci.auth.signers.get_resource_principals_signer()
        return oci.ai_language.AIServiceLanguageClient(config={}, signer=signer)
    else:
        logger.info("*** AIServiceLanguageClient: Using local config file for Authentication ***")
        config = oci.config.from_file()
        return oci.ai_language.AIServiceLanguageClient(config)


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
