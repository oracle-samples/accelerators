################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:19 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ca57f488cd6af799855de2e6f1726f4392871bde $
################################################################################################
#  File: ocs_util.py
################################################################################################
import os
import logging
from typing import List

from dateutil.tz import tzutc
import datetime
from datetime import timedelta
from pandas import DataFrame

from ai4service_automated_classification.ml.util.Strategy import  LastIngestionInFolderStrategy
from ai4service_automated_classification.utils.object_storage.bucket_vault import set_up_ocs_connection
from ai4service_automated_classification.utils.object_storage.ocs_loaders import OcsCSVLoader
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import OcsPath, FileInfo

logger = logging.getLogger(os.path.basename(__file__))

CSV_PATTERN = ".*\\.csv"


def retrieve_hierarchy_data(path: OcsPath) -> DataFrame:
    """
    Receive OcsPath and return hierarchy data from Object Storage.

    @type OcsPath
    @param path: DSDK OcsPath
    @rtype DataFrame
    @return: The hierarchy report data from Object Storage
    """
    hierarchy_bucket = set_up_ocs_connection(path.ocs_bucket)
    hierarchy_path_info = FileInfo(path.ocs_path)
    hierarchy_data: DataFrame = OcsCSVLoader(
        LastIngestionInFolderStrategy(hierarchy_bucket, regex_filter=CSV_PATTERN)
    ).load(hierarchy_path_info)
    return hierarchy_data


def retrieve_files(path: str,
                   bucket_vault: OcsBucket,
                   current_time: datetime.datetime,
                   interval: timedelta = timedelta(days=30, hours=0, minutes=0)) -> List[FileInfo]:
    """
    Retrieve files from OCS Path.

    :param path: OCS Path to folder
    :param bucket_vault: OCI Bucket
    :param current_time: Date of retrieval
    :param interval: Timedelta in days, hours, minutes
    :return: List[FileInfo]
    """
    unfiltered_files = bucket_vault.get_objects_with_prefix(path)
    files = []
    current_time = current_time.replace(tzinfo=tzutc())
    for file in unfiltered_files:
        created_time = file.time_created.replace(tzinfo=tzutc())
        if (current_time - created_time) >= interval:
            files.append(file)
    return files


def delete_files(path: OcsPath,
                 interval: timedelta = timedelta(days=30, hours=0, minutes=0)) -> None:
    """
    Delete files under folder older than interval.

    :param path: OCS Path to folder
    :param bucket_vault: OCI Bucket
    :param interval: Timedelta in days, hours, minutes
    :return: None
    """
    current_time = datetime.datetime.now(tz=tzutc())
    bucket_vault = set_up_ocs_connection(path.ocs_bucket)
    logger.info(f"Deleting files from '{path}' older than {interval}...")
    files = retrieve_files(path.ocs_path, bucket_vault, current_time, interval)
    logger.info(f"There are {len(files)} files in '{path}'.")
    for file in files:
        logger.debug(f"Deleting file '{file.name}'...")
        try:
            bucket_vault.delete_object(file.name)
        except Exception as e:
            logger.warning(f"Deleting '{file.name}' could not succeed. " + str(e))

    logger.info(f"Done deleting files from '{path}'.")
