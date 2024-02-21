
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
#  SHA1: $Id: 42ded88c299b960e5c136fbbd83ee55dad8d910d $
################################################################################################
#  File: storage.py
################################################################################################
####################################
# DataScience related resources
####################################

import os
import logging

from oci.object_storage import ObjectStorageClient
from oci.object_storage.models import CreateBucketDetails

from constants import COMPARTMENT_ID, LOGGING_FORMAT, ENCODING
from utils import get_object_storage_client, get_name

LOGGING_LEVEL = logging.INFO
logging.basicConfig(level=LOGGING_LEVEL, format=LOGGING_FORMAT)
logger = logging.getLogger()


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
    return client.put_object(namespace_name=namespace,
                             bucket_name=bucket_name,
                             object_name=f"{folder_name}",
                             put_object_body=data,
                             content_type=f"text/csv; charset={ENCODING}",
                             content_encoding=ENCODING)


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
