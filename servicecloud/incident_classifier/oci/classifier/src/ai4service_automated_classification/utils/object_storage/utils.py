################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Tue Jan 31 13:02:52 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: c849028c7e449e67d698af80d61b2ab266428134 $
################################################################################################
#  File: utils.py
################################################################################################
"""This module contains utility functions for ocs."""

import time
from urllib.parse import urlparse
import logging
from abc import ABC, abstractmethod
from datetime import datetime
from typing import Optional, List, Callable, Set, NamedTuple

import pandas as pd

from ai4service_automated_classification.constants import INCIDENT_COLUMN
from training_scripts.constants import TIMESTAMP_CHECK_RANGE

logger = logging.getLogger(__name__)
FILE_INFO_DATETIME_FORMAT = "%d/%m/%y %H:%M:%S"


class FileInfo(NamedTuple):
    """A container for a file name and metadata."""

    name: str
    size: float = 0
    time_created: datetime = datetime(2000, 1, 1)

    def __str__(self) -> str:
        """Format as a string."""
        file_info = {
            "time_created": self.time_created.strftime(FILE_INFO_DATETIME_FORMAT),
            "size": format_bytes_as_human_readable(self.size)
        }
        return f"{self.name}: {file_info}"


def format_bytes_as_human_readable(size: float) -> str:
    """
    Return size in a human-readable format.

    :param size: Size in bytes.
    :return: Size in a human-readable format.
    """
    multiplier = 1024.0
    units = ["bytes", "kB", "MB", "GB", "TB", "PB"]
    for unit in units[:-1]:
        if size < multiplier:
            return f"{size:.1f} {unit}"
        size /= multiplier
    return f"{size:.1f} {units[-1]}"


class DataLoader(ABC):
    """An abstract class outlining the common functionality required by any data loader."""

    def __init__(self, file_info_logging_enabled: bool = True) -> None:
        """Instantiate a DataLoader.

        :param file_info_logging_enabled: Flag for whether to log loaded data file info or not.
        """
        self.file_info_logging_enabled = file_info_logging_enabled

    @abstractmethod
    def _load_file(self, file_path: FileInfo) -> pd.DataFrame:
        """
        Load a file as a Pandas DataFrame.

        :param file_path: Path to the file.
        :return: Pandas DataFrame with the file content.
        """

    @abstractmethod
    def _load_folder(self, location: FileInfo) -> Optional[pd.DataFrame]:
        """
        Load all the files from the folder as a Pandas DataFrame.

        :param location: Path to location to get files from.
        :return: Pandas DataFrame with the content from the files.
        """

    @abstractmethod
    def _is_file(self, location: FileInfo) -> bool:
        """
        Check if the location exists and it's a file.

        :param location: Location to check (e.g. local path, URI).
        :return: True if the location exists and it's a file.
        """

    @abstractmethod
    def _is_dir(self, location: FileInfo) -> bool:
        """
        Check if the location exists and it's a directory.

        :param location: Location to check (e.g. local path, URI).
        :return: True if the location exists and it's a directory.
        """

    @abstractmethod
    def log_file_info(self, location: FileInfo) -> None:
        """
        Log information about loaded files if enable_file_info_logging is set to True.

        :param location: Location to check (e.g. local path, URI).
        """

    def load(self, location: FileInfo) -> Optional[pd.DataFrame]:
        """
        Load data given a file or folder location.

        :param location: A path to a file or folder.
        :return: The loaded data.
        """
        if self._is_file(location):
            data = self._load_file(location)
        elif self._is_dir(location):
            data = self._load_folder(location)
        else:
            raise NotADirectoryError(f"Supplied location '{location}' does not exist!")

        if data is None:
            logger.error("Could not read data!")
        elif data.empty:
            logger.warning("Loaded dataframe is empty!")
        else:
            logger.info(f"The shape of the loaded data: {data.shape}.")
        return data

    @staticmethod
    def _load_df_from_multiple_files(base_location: str,
                                     files_to_load: List[FileInfo],
                                     path_joiner: Callable[[str, FileInfo], FileInfo],
                                     file_loader: Callable[[FileInfo], pd.DataFrame]
                                     ) -> Optional[pd.DataFrame]:
        """
        Load several files into one Pandas DataFrame regardless of the source.

        :param base_location: Folder (any source) where the files are located.
        :param files_to_load: List of files to load (only names without the path).
        :param path_joiner: A function which joins the base location and the filename.
        :param file_loader: A function which takes a full filepath and loads one file as a
        DataFrame.
        :return: One DataFrame which aggregates all the files.
        """
        data_chunks: List[pd.DataFrame] = []
        prev_columns: Set[str] = set()
        for file_name in files_to_load:
            file_path = path_joiner(base_location, file_name)
            file_df = file_loader(file_path)
            cur_columns = set(file_df.columns)
            if cur_columns:
                if not prev_columns:
                    prev_columns = cur_columns
                if cur_columns == prev_columns:
                    data_chunks.append(file_df)
                    prev_columns = cur_columns
                else:
                    raise ValueError(f'Columns for file {file_path} do not match previous ones\n'
                                     f'Expected (from earlier files): {prev_columns}\n'
                                     f'Loaded: {cur_columns}\n'
                                     f'Difference: '
                                     f'{prev_columns.symmetric_difference(cur_columns)}')
            else:
                logger.warning(f"Empty file in location: {file_path}, ignoring.")

        if data_chunks:
            full_data = pd.concat(data_chunks, axis='rows')
            return full_data
        else:
            return None


class OcsPath(NamedTuple):
    """Object to represent an OCS path."""

    ocs_bucket: str
    ocs_path: str


def parse_ocs_uri(uri: str, report_type="") -> OcsPath:
    """
    Parse an OCI URI into an OcsPath.

    @param report_type: type of the report to be fetched. e.g, incident,product,etc.
    @param uri: URI for an oci location.
    @return: OcsPath with the correct bucket and path.
    """
    parsed_url = urlparse(uri)
    scheme = parsed_url.scheme

    if scheme != 'oci' or not uri.endswith('/'):
        raise ValueError(f"Unsupported URI format. Got '{uri}', "
                         f"expected format: 'oci://<bucket>/<path>/'")
    if INCIDENT_COLUMN in report_type:
        report_path = parsed_url.path.lstrip('/')
    else:
        report_path = parsed_url.path.replace('autoclassif/', '')
        report_path = report_path.lstrip('/') + report_type + "/"
    return OcsPath(ocs_bucket=parsed_url.netloc,
                   ocs_path=report_path)


def is_oci_uri(uri: str) -> bool:
    """
    Determine whether a uri is oci or not.

    :param uri:
    :return:
    """
    return uri.startswith("oci:")


def _validate_timestamp(timestamp: Optional[float] = None) -> float:
    """
    Check the timestamp is within range or generate a new one if None.

    :param timestamp: Optional timestamp in float format.
    :return: Passed timestamp in float format. If passed timestamp is none, the current
    timestamp is returned.
    """
    current_ts = time.time()
    if not timestamp:
        return current_ts

    # the timestamp should be within 1 hour before the system time
    if not (current_ts - TIMESTAMP_CHECK_RANGE < timestamp <= current_ts):
        logger.warning(
            f"Passed timestamp is over {TIMESTAMP_CHECK_RANGE / 3600}h "
            f"behind current system time. The model timestamp should be within "
            f"{TIMESTAMP_CHECK_RANGE / 3600}h before the system time. Try creating the timestamp "
            f"once the model is trained and ready to be saved.")

    return timestamp
