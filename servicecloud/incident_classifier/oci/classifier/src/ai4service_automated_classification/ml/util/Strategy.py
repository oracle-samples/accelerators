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
#  SHA1: $Id: 5a0f4a3281eba5f9cdb93be2fb47b9dda64d47e6 $
################################################################################################
#  File: Strategy.py
################################################################################################
"""Strategies to load data from Oracle Cloud Storage."""
import logging
import re
from operator import attrgetter
from typing import Optional, List, Union

from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.strategies import OcsFileBasedStrategy, \
    _extract_relative_path
from ai4service_automated_classification.utils.object_storage.utils import FileInfo

class LastIngestionInFolderStrategy(OcsFileBasedStrategy):
    """Get the last modified file in the provided Oracle Cloud Storage (OCS) location."""

    def get_paths_to_load(self, uri: str, return_full_paths: Optional[bool] = True) -> List[FileInfo]:
        """
        Scan a folder and get the last ingested files in the provided location.

        :param uri: Path to scan.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.
        :return: Single element list of the most recently created OCS objects in the folder,
        containing the file name and metadata.
        """
        if not self.ocs_bucket.is_directory(uri):
            raise NotADirectoryError(f"{uri} is not a directory or does not exist!")
        ocs_files = [FileInfo(*ocs_file) for ocs_file in self.ocs_bucket.get_objects_with_prefix(uri)]
        if not return_full_paths:
            ocs_files = [_extract_relative_path(uri, ocs_file) for ocs_file in ocs_files]

        filtered_ocs_files = [
            ocs_file for ocs_file in ocs_files
            if not self._regex_filter or re.match(self._regex_filter, ocs_file.name)
        ]

        last_ingested_files: List[FileInfo] = []
        if filtered_ocs_files:
            sorted_files = sorted(filtered_ocs_files, key=attrgetter('time_created'))
            last_file = sorted_files[-1]
            try:
                timestamp = last_file.name.split('-')[-2]
            except IndexError as e:
                logging.error(f"Timestamp was not found in the last ingested file name: {last_file.name}")
                raise e
            last_ingested_files = [ocs_file for ocs_file in sorted_files if timestamp in ocs_file.name]
        return last_ingested_files


class IngestionInFolderStrategy(OcsFileBasedStrategy):
    """Get the last n files in the provided Oracle Cloud Storage (OCS) location."""

    def __init__(self, ocs_bucket_or_name: Union[str, OcsBucket],
                 num_files_to_load: int = 1,
                 regex_filter: Optional[str] = None):
        super().__init__(ocs_bucket_or_name, regex_filter=regex_filter)
        self.num_files_to_load = num_files_to_load

    def get_paths_to_load(self, uri: str, return_full_paths: Optional[bool] = True) -> List[FileInfo]:
        """
        Scan a folder and get the last ingested files in the provided location according to num_files_to_load.

        :param uri: Path to scan.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.
        :return: Single element list of the most recently created OCS objects in the folder,
        containing the file name and metadata.
        """
        if not self.ocs_bucket.is_directory(uri):
            raise NotADirectoryError(f"{uri} is not a directory or does not exist!")

        ocs_files = [FileInfo(*ocs_file) for ocs_file in self.ocs_bucket.get_objects_with_prefix(uri)]
        if not return_full_paths:
            ocs_files = [_extract_relative_path(uri, ocs_file) for ocs_file in ocs_files]

        filtered_ocs_files = [
            ocs_file for ocs_file in ocs_files
            if not self._regex_filter or re.match(self._regex_filter, ocs_file.name)
        ]

        sorted_files = sorted(filtered_ocs_files, key=attrgetter('time_created'))
        return sorted_files[:self.num_files_to_load]