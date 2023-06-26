################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:24 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 047397870a04ccc9ad71521b7f2819af98413d46 $
################################################################################################
#  File: strategies.py
################################################################################################
"""Strategies to load data from Oracle Cloud Storage."""
import logging
import re
from abc import ABC, abstractmethod
from operator import attrgetter
from typing import List, Union, Optional, Set

from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import FileInfo

logger = logging.getLogger(__name__)


def _extract_relative_path(uri: str, ocs_file: FileInfo) -> FileInfo:
    return FileInfo(name=ocs_file.name.replace(uri, '', 1),
                    time_created=ocs_file.time_created,
                    size=ocs_file.size)


class OcsFileBasedStrategy(ABC):
    """Abstract class for strategy to load files from Oracle Cloud Storage (OCS)."""

    def __init__(self,
                 ocs_bucket_or_name: Union[str, OcsBucket],
                 regex_filter: Optional[str] = None):
        """
        Initialise the OCS bucket.

        :param ocs_bucket_or_name: Either OCS bucket or its name.
        :param regex_filter: optional regex pattern used to select files within the location.
        """
        self._regex_filter = regex_filter
        if isinstance(ocs_bucket_or_name, str):
            ocs_bucket_name = ocs_bucket_or_name
            self.ocs_bucket = OcsBucket(bucket_name=ocs_bucket_name)
        elif isinstance(ocs_bucket_or_name, OcsBucket):
            self.ocs_bucket = ocs_bucket_or_name
        else:
            raise TypeError("'ocs_bucket_or_name' should either be a bucket name "
                            "or an OcsBucket object")

    @abstractmethod
    def get_paths_to_load(self, uri: str, return_full_paths: Optional[bool]) -> List[FileInfo]:
        """
        Get files inside specific location in an OCS location according to some rule.

        :param uri: OCS folder to scan for objects.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.

        :return: List of OCS objects, containing the file names and metadata.
        """


class AllObjectsInUriStrategy(OcsFileBasedStrategy):
    """Strategy to load all object in a particular uri address in Oracle Cloud Storage (OCS)."""

    def get_paths_to_load(
            self, uri: str, return_full_paths: Optional[bool] = True) -> List[FileInfo]:
        """
        Get all files (objects) inside specific location in a bucket recursively.

        :param uri: OCS folder to scan for objects.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.

        :return: List of OCS objects in the folder, containing the file names and metadata.
        """
        if not self.ocs_bucket.is_directory(uri):
            raise NotADirectoryError(f"{uri} is not a directory or does not exist!")
        ocs_files = [
            FileInfo(*ocs_file) for ocs_file in self.ocs_bucket.get_objects_with_prefix(uri)]
        if not return_full_paths:
            ocs_files = [_extract_relative_path(uri, ocs_file) for ocs_file in ocs_files]

        filtered_ocs_files = [
            ocs_file for ocs_file in ocs_files
            if not self._regex_filter or re.match(self._regex_filter, ocs_file.name)
        ]
        return filtered_ocs_files


class LastModifiedFileInFolderStrategy(OcsFileBasedStrategy):
    """Get the last modified file in the provided Oracle Cloud Storage (OCS) location."""

    def get_paths_to_load(
            self, uri: str, return_full_paths: Optional[bool] = True) -> List[FileInfo]:
        """
        Scan a folder and get the last modified file in the provided location.

        :param uri: Path to scan.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.
        :return: Single element list of the most recently created OCS objects in the folder,
        containing the file name and metadata.
        """
        if not self.ocs_bucket.is_directory(uri):
            raise NotADirectoryError(f"{uri} is not a directory or does not exist!")
        ocs_files = [
            FileInfo(*ocs_file) for ocs_file in self.ocs_bucket.get_objects_with_prefix(uri)]
        if not return_full_paths:
            ocs_files = [_extract_relative_path(uri, ocs_file) for ocs_file in ocs_files]

        filtered_ocs_files = [
            ocs_file for ocs_file in ocs_files
            if not self._regex_filter or re.match(self._regex_filter, ocs_file.name)
        ]

        if not filtered_ocs_files:
            return []
        else:
            return sorted(filtered_ocs_files, key=attrgetter('time_created'))[-1:]


class AllObjectsInUriPartitionsStrategy(OcsFileBasedStrategy):
    """
    Strategy to load all objects in a particular uri address in Oracle Cloud Storage (OCS).

    The URI has to contain partitions and these partitions can be filtered with minimum and
    maximum values.
    """

    def __init__(self, ocs_bucket_or_name: Union[str, OcsBucket],
                 regex_filter: Optional[str] = None,
                 min_partition: Optional[str] = None,
                 max_partition: Optional[str] = None,
                 partition_prefix: str = 'partition=',
                 check_success_files: bool = False):
        """
        Instantiate a strategy using provided minimum and maximum partitions.

        Partitions are folders with names in the format <partition_prefix><partition_value>. The
        files to load are expected to be directly inside this folder and not in any other
        sub-folder.

        :param ocs_bucket_or_name: Either OCS bucket or its name.
        :param regex_filter: optional regex pattern used to select files within the location.
        :param min_partition: Minimum partition value to allow for selection.
        :param max_partition: Maximum partition value to allow for selection.
        :param partition_prefix: Partition prefix (defaults to 'partition=').
        :param check_success_files: Check that only folders that contain a _SUCCESS file can
            be a prefix to valid file paths.
        """
        super().__init__(ocs_bucket_or_name, regex_filter=regex_filter)
        self.check_success_files = check_success_files
        self.partition_prefix = partition_prefix
        self.min_partition = min_partition
        self.max_partition = max_partition

    def _get_partition_value(self, full_path: str) -> str:
        path_components = full_path.split('/')
        if len(path_components) < 2:
            return ''
        partition_candidate = path_components[-2]
        if partition_candidate.startswith(self.partition_prefix):
            return partition_candidate.replace(self.partition_prefix, '', 1)
        else:
            return ''

    def _is_partition_valid_within_range(self, partition_name: str) -> bool:
        if not partition_name:
            return False
        if self.max_partition and (partition_name > self.max_partition):
            return False
        if self.min_partition and (partition_name < self.min_partition):
            return False
        return True

    def _has_valid_prefix(self, ocs_file_name: str, valid_prefixes: Set[str]) -> bool:
        if not self.check_success_files:
            return True
        ocs_file_prefix = ocs_file_name.rsplit('/', 1)[0]
        return any(ocs_file_prefix.startswith(valid_prefix)
                   for valid_prefix in valid_prefixes)

    def _get_valid_prefixes(self, ocs_files: List[FileInfo]) -> Set[str]:
        if not self.check_success_files:
            return set()
        valid_prefixes = {ocs_file.name.rsplit('/', 1)[0]
                          for ocs_file in ocs_files
                          if ocs_file.name.endswith('_SUCCESS')}
        logger.info(f'Valid prefixes (ones with a success file): {valid_prefixes}')
        return valid_prefixes

    def _has_valid_partition(self, file_name: str) -> bool:
        return self._is_partition_valid_within_range(self._get_partition_value(file_name))

    def get_paths_to_load(self,
                          uri: str,
                          return_full_paths: Optional[bool] = True) -> List[FileInfo]:
        """
        Get all files (objects) inside specific location in a bucket recursively.

        :param uri: OCS folder to scan for objects.
        :param return_full_paths: If True, then full paths are returned, otherwise relative paths.

        :return: List of  file names in the folder.
        """
        if not self.ocs_bucket.is_directory(uri):
            raise NotADirectoryError(f"{uri} is not a directory or does not exist!")
        ocs_files = self.ocs_bucket.get_objects_with_prefix(uri)
        valid_prefixes = self._get_valid_prefixes(ocs_files)

        ocs_files_names = [
            ocs_file.name if return_full_paths
            else ocs_file.name.replace(uri, '', 1)
            for ocs_file in ocs_files
            if self._has_valid_partition(ocs_file.name) and (
                self._has_valid_prefix(ocs_file.name, valid_prefixes))]

        filtered_paths = [FileInfo(file_path) for file_path in ocs_files_names
                          if not self._regex_filter or re.match(self._regex_filter, file_path)]
        return filtered_paths
