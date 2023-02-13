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
#  SHA1: $Id: 0f0a7711178fe7ee47f8d5b21550c49b268c70cd $
################################################################################################
#  File: ocs_loaders.py
################################################################################################
"""Loaders for Oracle cloud storage."""

import logging
from abc import abstractmethod
from typing import Optional, List, Any, Union, Callable

import pandas as pd

from ai4service_automated_classification.utils.object_storage.os import DEFAULT_ENCODING
from ai4service_automated_classification.utils.object_storage.strategies import OcsFileBasedStrategy
from ai4service_automated_classification.utils.object_storage.utils import DataLoader, FileInfo, format_bytes_as_human_readable

logger = logging.getLogger(__name__)


class OcsLoader(DataLoader):
    """Data loader from Oracle Cloud Storage (OCS)."""

    def __init__(self, strategy: OcsFileBasedStrategy, file_info_logging_enabled: bool = True):
        """
        Instantiate an OCSLoader with a given strategy.

        :param strategy: Strategy to be used to load the files.
        :param file_info_logging_enabled: Flag for whether to log loaded data file info or not.
        """
        super().__init__(file_info_logging_enabled)
        self.strategy = strategy
        self.ocs_bucket = strategy.ocs_bucket

    def _is_file(self, location: FileInfo) -> bool:
        return self.ocs_bucket.is_file(location.name)

    def _is_dir(self, location: FileInfo) -> bool:
        return self.ocs_bucket.is_directory(location.name)

    @abstractmethod
    def _load_file(self, file_path: FileInfo) -> pd.DataFrame:
        """
        Load a file as a Pandas DataFrame.

        :param file_path: Path to the file.
        :return: Pandas DataFrame with the file content.
        """

    def _load_folder(self, location: FileInfo) -> Optional[pd.DataFrame]:
        if not self._is_dir(location):
            raise NotADirectoryError(f'Location {location} does not exist, '
                                     f'it is not a directory, or it is empty!')
        files_to_load = self.strategy.get_paths_to_load(location.name, return_full_paths=False)

        return self._load_df_from_multiple_files(
            base_location=location.name,
            files_to_load=files_to_load,
            path_joiner=OcsLoader._path_joiner,
            file_loader=self._load_file)

    @staticmethod
    def _path_joiner(base: str, ocs_file: FileInfo) -> FileInfo:
        return FileInfo(
            name=f"{base}{ocs_file.name}",
            time_created=ocs_file.time_created,
            size=ocs_file.size
        )

    def log_file_info(self, file_path: FileInfo) -> None:
        """
        Log information about the file at the given file_path.

        :param file_path: Path to the file.
        """
        msg = f"File info for OCS file with file_path {file_path}"
        logger.info(msg)


class OcsCSVLoader(OcsLoader):
    """CSV loader from Oracle Cloud Storage (OCS)."""

    def __init__(self, strategy: OcsFileBasedStrategy,
                 file_info_logging_enabled: bool = True,
                 selected_columns: Optional[Union[List[str], Callable[[str], bool]]] = None,
                 force_default_encoding: Optional[bool] = False,
                 **pandas_kwargs: Optional[Any]):
        """
        Initialize the class with parameters.

        :param strategy: Strategy followed to load files from Oracle Cloud Storage.
        :param file_info_logging_enabled: Flag for whether to log loaded data file info or not.
        :param selected_columns: Subset of columns to be loaded from csv files.
        :param pandas_kwargs: Pandas parameters for pd.read_csv().
        """
        super().__init__(strategy, file_info_logging_enabled)
        self.selected_columns = selected_columns
        self.encoding = (pandas_kwargs.pop('encoding')
                         if 'encoding' in pandas_kwargs else DEFAULT_ENCODING)
        self.force_default_encoding = force_default_encoding
        self.extra_pandas_args = pandas_kwargs

    def _load_file(self, file_path: FileInfo) -> pd.DataFrame:
        if self.file_info_logging_enabled:
            self.log_file_info(file_path)

        if 'compression' in self.extra_pandas_args:
            df = self.ocs_bucket.read_binary_object_into_dataframe(
                object_name=file_path.name,
                selected_columns=self.selected_columns,
                **self.extra_pandas_args)
        else:
            df = self.ocs_bucket.read_csv_object_into_dataframe(
                object_name=file_path.name,
                selected_columns=self.selected_columns,
                default_encoding=self.encoding,
                force_default_encoding=self.force_default_encoding,
                **self.extra_pandas_args)
        return df


class DynamicSamplingOcsCSVLoader(OcsCSVLoader):
    """
    Class to dynamically sample data as it is loaded.

    This is used to avoid memory problems that occur when we try to load too much data in one go.
    This works by specifying a maximum total size we would like the data to be and then use the
    ratio of that value vs the total size of all files to be loaded to define the sampling ratio.
    When data is compressed, this value is trickier to define, but expect the uncompressed files to
    be between 10x and 20x larger than the compressed ones, so set a limit 10-20x smaller than you
    normally would.
    """

    def __init__(self, strategy: OcsFileBasedStrategy,
                 max_total_size_bytes: int,
                 file_info_logging_enabled: bool = True,
                 selected_columns: Optional[List[str]] = None,
                 random_state: int = 42,
                 **pandas_kwargs: Optional[Any]):
        """
        Instantiate the class.

        :param strategy: File selection strategy.
        :param max_total_size_bytes: Maximum size we would like to load (in bytes).
        :param file_info_logging_enabled: Flag for whether to log loaded data file info or not.
        :param selected_columns: Columns to load from the files.
        :param pandas_kwargs: Extra arguments for pandas.
        """
        super().__init__(strategy, file_info_logging_enabled, selected_columns, **pandas_kwargs)
        self.random_state = random_state
        self.max_total_size_bytes = max_total_size_bytes
        self.sampling_ratio = 1.0
        self.total_rows = 0
        self.files_loaded: List[str] = []

    def _load_file(self, file_path: FileInfo) -> pd.DataFrame:
        raw_df = super()._load_file(file_path)
        self.total_rows += len(raw_df)
        sampled_df = raw_df.sample(frac=self.sampling_ratio,
                                   replace=False,
                                   random_state=self.random_state)
        return sampled_df

    def _load_folder(self, location: FileInfo) -> Optional[pd.DataFrame]:
        if not self._is_dir(location):
            raise NotADirectoryError(f'Location {location} does not exist, '
                                     f'it is not a directory, or it is empty!')
        files_to_load = self.strategy.get_paths_to_load(location.name, return_full_paths=False)
        self.sampling_ratio = self._get_sampling_ratio(files_to_load)
        logger.info(f'Sampling ratio used: {self.sampling_ratio}')
        self.files_loaded = [f'{location.name}{ocs_file.name}' for ocs_file in files_to_load]
        df = self._load_df_from_multiple_files(
            base_location=location.name,
            files_to_load=files_to_load,
            path_joiner=OcsLoader._path_joiner,
            file_loader=self._load_file)
        if df is not None:
            logger.info(f'Loaded {len(df)} rows from a total of {self.total_rows} available.')
        return df

    def _get_sampling_ratio(self, files_to_load: List[FileInfo]) -> float:
        files_info = "\n".join(f"  {file}" for file in files_to_load)
        logger.debug(f"File info:\n{files_info}")
        total_size_bytes = sum(ocs_file.size for ocs_file in files_to_load)
        logger.info(f"Total data to be loaded before sampling:"
                    f" {format_bytes_as_human_readable(total_size_bytes)}")
        if total_size_bytes > self.max_total_size_bytes:
            sampling_ratio = self.max_total_size_bytes / total_size_bytes
        else:
            sampling_ratio = 1.0
        return sampling_ratio
