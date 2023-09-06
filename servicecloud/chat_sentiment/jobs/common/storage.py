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
#  SHA1: $Id: 7b20f2f18dcc9bffacf0154daf538e576324b22a $
################################################################################################
#  File: storage.py
################################################################################################

import os
import logging
from typing import Optional

import oci
import re
from oci.object_storage import ObjectStorageClient
from oci.object_storage.models import CreateBucketDetails

from common.constants import COMPARTMENT_ID, LOGGING_FORMAT

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


def store_dataframe(ocs_client,
                    location: str,
                    filename: str,
                    data,
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
        store_or_update_csv(object_name, data, ocs_client)
        logger.info(f"Uploaded {filename}")
    except Exception as err:
        logger.error(err)


def get_csv_file_names(bucket_name: str, prefix="", client: ObjectStorageClient = None):
    if client is None:
        client = get_object_storage_client()
    limit_per_page = 1000

    def _filter_objects(obj_list):
        return [obj.name for obj in obj_list if obj.name.endswith(".csv")]

    namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
    fields = 'name,size,timeCreated'

    if not is_bucket_exists(bucket_name, client):
        client.create_bucket(namespace_name=namespace,
                             create_bucket_details=CreateBucketDetails(name=bucket_name,
                                                                       compartment_id=os.getenv(COMPARTMENT_ID)))

    response = client.list_objects(namespace,
                                   bucket_name,
                                   prefix=prefix,
                                   fields=fields,
                                   limit=limit_per_page).data
    object_list = response.objects
    all_objects = _filter_objects(object_list)
    while response.next_start_with:
        response = client.list_objects(namespace,
                                       bucket_name,
                                       prefix=prefix,
                                       fields=fields,
                                       start=response.next_start_with,
                                       limit=limit_per_page).data
        object_list = response.objects
        next_page_objects = _filter_objects(object_list)
        all_objects.extend(next_page_objects)
    return namespace, all_objects


def is_bucket_exists(bucket_name, client: ObjectStorageClient = None):
    if client is None:
        client = get_object_storage_client()
    namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
    try:
        buckets = client.list_buckets(namespace_name=namespace, compartment_id=os.getenv(COMPARTMENT_ID)).data
        found = False
        for bucket in buckets:
            if bucket.name == bucket_name:
                found = True
                break
    except Exception as err:
        logger.info(err)
        found = False
    return found


def get_bucket_details(bucket_location: str):
    bucket_name = get_name("bucket", bucket_location)
    folder_name = get_name("folder", bucket_location)
    return bucket_name, folder_name


def get_object_list(bucket_location: str):
    client = get_object_storage_client()
    bucket_name, folder_name = get_bucket_details(bucket_location)
    if not is_bucket_exists(bucket_name, client):
        logger.info(f"*** BUCKET: {bucket_location} not found. ***")
    namespace, objects = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=client)
    if len(objects) == 0:
        raise FileNotFoundError("!!! Not enough data to train on !!!")
    return bucket_name, namespace, objects


def delete_folder(bucket_location: str, client: ObjectStorageClient = None):
    if client is None:
        client = get_object_storage_client()
    bucket_name, folder_name = get_bucket_details(bucket_location)
    if is_bucket_exists(bucket_name, client):
        logger.info(f"*** BUCKET: {bucket_location} found. ***")
        namespace, objects = get_csv_file_names(bucket_name=bucket_name, prefix=folder_name, client=client)
        logger.info(f"*** Deleting {len(objects)} global incidents files ***")
        for object_name in objects:
            client.delete_object(namespace, bucket_name, object_name)


def is_directory(bucket_location: str, client: ObjectStorageClient = None) -> bool:
    namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
    bucket_name, folder_name = get_bucket_details(bucket_location)
    if folder_name.endswith('/'):
        try:
            objects = client.list_objects(namespace, bucket_name, prefix=folder_name, fields=None).data.objects
            return len(objects) > 0
        except Exception as err:
            logger.error(err)
            return False
    return False


def store_or_update_csv(bucket_location: str, data, client: ObjectStorageClient = None):
    if client is None:
        client = get_object_storage_client()
    bucket_name, folder_name = get_bucket_details(bucket_location)
    namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
    if not is_bucket_exists(bucket_name, client):
        logger.info(f"*** BUCKET: {bucket_location} not found. ***")
        logger.info(f"*** Creating Bucket: {bucket_location}")
        client.create_bucket(namespace_name=namespace,
                             create_bucket_details=CreateBucketDetails(name=bucket_name,
                                                                       compartment_id=os.getenv(COMPARTMENT_ID)))
    encoding = "utf-8"
    return client.put_object(namespace_name=namespace,
                             bucket_name=bucket_name,
                             object_name=f"{folder_name}",
                             put_object_body=data,
                             content_type=f"text/csv; charset={encoding}",
                             content_encoding=encoding)


def store_csv(bucket_location: str, filename: str, data, client: ObjectStorageClient = None):
    if client is None:
        client = get_object_storage_client()
    bucket_name, folder_name = get_bucket_details(bucket_location)
    namespace = client.get_namespace(compartment_id=os.getenv(COMPARTMENT_ID)).data
    if not is_bucket_exists(bucket_name, client):
        logger.info(f"*** BUCKET: {bucket_location} not found. ***")
        logger.info(f"*** Creating Bucket: {bucket_location}")
        client.create_bucket(namespace_name=namespace,
                             create_bucket_details=CreateBucketDetails(name=bucket_name,
                                                                       compartment_id=os.getenv(COMPARTMENT_ID)))
    encoding = "utf-8"
    return client.put_object(namespace_name=namespace,
                             bucket_name=bucket_name,
                             object_name=filename,
                             put_object_body=data,
                             content_type=f"text/csv; charset={encoding}",
                             content_encoding=encoding)
