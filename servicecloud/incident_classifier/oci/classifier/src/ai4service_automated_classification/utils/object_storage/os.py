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
#  SHA1: $Id: ae4b8fa34a0ae453ab35e4f5982b6373f14f68fd $
################################################################################################
#  File: os.py
################################################################################################
"""Classes for accessing OCS bucket data."""
import gzip
import logging
import os
import uuid
from io import BytesIO, StringIO
from types import TracebackType
from typing import Any, List, Optional, Type, Tuple, Union, Callable

import joblib
import oci
import pandas as pd
from oci.config import DEFAULT_PROFILE
from oci.exceptions import ServiceError
from oci.object_storage.models import CreateBucketDetails
from oci.response import Response
from oci.retry import DEFAULT_RETRY_STRATEGY
from oci.base_client import (DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)

from training_scripts.constants import COMPARTMENT_ID
from ai4service_automated_classification.utils.object_storage.utils import FileInfo

logger = logging.getLogger(__name__)
ColumnTypes = Union[List[str], Callable[[str], bool]]
DEFAULT_ENCODING = 'utf-8'
DEFAULT_TIMEOUT = (DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)

# OCI HELPER CONSTANTS
OCI_MODEL_DEPLOYMENT_ENV = 'AIACS_OCI_MODEL_DEPLOYMENT'
USE_MODEL_PREFIX_ENV = 'AIACS_USE_MODEL_PREFIX'
OCI_PREFIX = 'oci_'


class OcsBucket:
    """Implements methods to interact with Oracle Cloud Storage on OCI.

    More info on OCI buckets can be found in
     https://docs.cloud.oracle.com/iaas/Content/Object/Tasks/managingbuckets.htm.
    """

    def __init__(self, config_file: Optional[str] = None,
                 bucket_name: Optional[str] = None,
                 profile_name: Optional[str] = None,
                 region: Optional[str] = None,
                 timeout: Optional[Union[float, Tuple[float, float]]] = DEFAULT_TIMEOUT) -> None:
        """
        Create a class with the given configurations.

        :param config_file: OCI configuration file path. (Defaults to None,
            where it reads the default oci configuration file from the user's home directory)
        :param bucket_name: Name of the bucket being used.
        :param profile_name: Name of the configuration profile to be used (defaults to OCI if
            supplying a config file path).
        :param region: OCI region to use (defaults to the one present in the configuration).
        :param timeout: (optional) The connection and read timeouts for the client.
            The default values are connection timeout 10 seconds and read timeout 60 seconds.
            This keyword argument can be provided as a single float, in which case the value
            provided is used for both the read and connection timeouts, or as a tuple of two
            floats. If a tuple is provided then the first value is used as the connection timeout
            and the second value as the read timeout.
        :type timeout: float or tuple(float, float)
        """
        self.bucket_name = bucket_name
        if OciHelper.is_resource_principals_enabled():
            logger.info("Resource principal is enabled. Hence, using get_resource_principals_signer() to authenticate")
            signer = oci.auth.signers.get_resource_principals_signer()
            self.object_storage = oci.object_storage.ObjectStorageClient(config={},
                                                                         signer=signer,
                                                                         timeout=timeout)
            self.namespace = self.object_storage.get_namespace().data
            if not self.is_exists():
                logger.info(f'Creating bucket named as a {bucket_name}')
                if OciHelper.is_resource_principals_enabled():
                    if COMPARTMENT_ID not in os.environ:
                        raise SystemExit("Please provide the compartment id.")
                # compartment id should have ocid1.compartment not tenancy
                self.create_bucket(compartment_id=signer.compartment_id)
        else:  # keeping old functionality
            if config_file:
                self._oci_config = oci.config.from_file(
                    file_location=config_file,
                    profile_name=profile_name if profile_name else 'oci')
            else:
                self._oci_config = oci.config.from_file(
                    profile_name=profile_name if profile_name else DEFAULT_PROFILE)

            if region:
                self._oci_config['region'] = region

            self.object_storage = oci.object_storage.ObjectStorageClient(
                self._oci_config, timeout=timeout, retry_strategy=DEFAULT_RETRY_STRATEGY)
            self.namespace = self.object_storage.get_namespace().data
            if not self.is_exists():
                logger.info(f'Creating bucket named as a {bucket_name}')
                if OciHelper.is_resource_principals_enabled():
                    if COMPARTMENT_ID not in os.environ:
                        raise SystemExit("Please provide the compartment id.")
                # compartment id should have ocid1.compartment not tenancy
                self.create_bucket(compartment_id=os.getenv(COMPARTMENT_ID))

        self.check_bucket_access(namespace=self.namespace, bucket_name=bucket_name)

    def check_bucket_access(self, namespace: str, bucket_name: Optional[str] = None) -> None:
        """
        Check for bucket access.

        :param namespace: Object storage namespace.
        :param bucket_name: Name of the bucket being used.
        """
        try:
            self.object_storage.get_bucket(namespace, bucket_name)
        except oci.exceptions.ServiceError as exception:
            raise RuntimeError(f"User is unable to access bucket, {exception}")

    def create_bucket(self, compartment_id: Optional[str] = None) -> 'OcsBucket':
        """
        Create a bucket in Oracle Cloud Storage (OCS).

        This function will create a bucket for storing objects in a given compartment
        within an Object Storage namespace.

        A compartment is a collection of related resources that can be accessed
        only by certain authorized groups.

        :param compartment_id: Id of the OCS compartment.
        :return: The class itself.
        """
        request = CreateBucketDetails()
        if not compartment_id:
            compartment_id = self._oci_config["compartment_id"]
        request.compartment_id = compartment_id
        request.name = self.bucket_name
        self.object_storage.create_bucket(self.namespace, request)
        return self

    def get_objects_with_prefix(self, prefix: str, include_dirs: bool = False,
                                limit_per_page: int = 1000) -> List[FileInfo]:
        """
        Get objects given a prefix.

        In order to get all objects recursively, enable `include_dirs`.

        This function uses oci.object_storage.ObjectStorageClient under the hood, which limits
        the response to 1000 objects. If more objects are present in the given OCS path, this
        function will iterate through the paginated response to get all objects.

        :param prefix: String path prefix.
        :param include_dirs: If true, include objects within directories recursively.
        :param limit_per_page: Limit the number of objects loaded per page, between 1 and 1000.
        :return: List of objects from OCS.
        """

        def _filter_objects(object_list: List[FileInfo]) -> List[FileInfo]:
            filtered_object_list = [FileInfo(obj.name, obj.size, obj.time_created)
                                    for obj in object_list
                                    if include_dirs or not obj.name.endswith('/')]
            return filtered_object_list

        fields = 'name,size,timeCreated'
        response = self.object_storage.list_objects(self.namespace,
                                                    self.bucket_name,
                                                    prefix=prefix,
                                                    fields=fields,
                                                    limit=limit_per_page).data
        object_list = response.objects
        all_objects = _filter_objects(object_list)

        while response.next_start_with:
            response = self.object_storage.list_objects(
                self.namespace, self.bucket_name, prefix=prefix, fields=fields,
                start=response.next_start_with, limit=limit_per_page).data

            object_list = response.objects
            next_page_objects = _filter_objects(object_list)
            all_objects.extend(next_page_objects)

        return all_objects

    def is_exists(self) -> bool:
        """
        Check if the object exists and it is a directory.

        From the official `Oracle Cloud documentation: Interacting With Object Storage
        docs.cloud.oracle.com/iaas/Content/StorageGateway/Tasks/interactingwithobjectstorage.htm
        - Bucket exists or not.
        - File length can be 0 (zero) or non-zero, but directory length is always 0 (zero).

        :return: True if the bucket is a exists.
        """
        try:
            response = self.object_storage.get_bucket(self.namespace, self.bucket_name)
            if response.status == 200:
                return True
        except (RuntimeError, ServiceError) as err:
            logger.error(err)
            return False
        return False

    def is_directory(self, location: str) -> bool:
        """
        Check if the object exists and it is a directory.

        From the official `Oracle Cloud documentation: Interacting With Object Storage
        docs.cloud.oracle.com/iaas/Content/StorageGateway/Tasks/interactingwithobjectstorage.htm
        - Directories have a trailing / and files do not.
        - File length can be 0 (zero) or non-zero, but directory length is always 0 (zero).

        :param location: Path of the folder to check.
        :return: True if the object is a directory.
        """
        if location.endswith('/'):
            try:
                objects = self.object_storage.list_objects(self.namespace,
                                                           self.bucket_name,
                                                           prefix=location,
                                                           fields=None).data.objects
                return len(objects) > 0
            except RuntimeError as err:
                logger.error(err)
                return False
        return False

    def is_file(self, location: str) -> bool:
        """
        Check if the object exists and it is a file.

        From the official `Oracle Cloud documentation: Interacting With Object Storage
        docs.cloud.oracle.com/iaas/Content/StorageGateway/Tasks/interactingwithobjectstorage.htm
        - Directories have a trailing / and files do not.
        - File length can be 0 (zero) or non-zero, but directory length is always 0 (zero).

        :param location: Full path of the file to check.
        :return: True if the object is a directory.
        """
        if not location.endswith('/'):
            try:
                self.object_storage.get_object(self.namespace, self.bucket_name, location)
                return True
            except oci.exceptions.ServiceError:
                return False
        return False

    @staticmethod
    def _ensure_encoding(content_data: object,
                         default_encoding: Optional[str] = DEFAULT_ENCODING,
                         force: Optional[bool] = False) -> Optional[str]:
        """
        Set the encoding of the object_storage get_object() response if the value isn't provided.

        :param content_data: The object given by object_storage.get_object(...).data property.
        :param default_encoding: If the encoding isn't set, then default it to this value.
        :param force: If set to True, the default encoding is enforced even if content_data
                      encoding is set.
        :return: Encoding used.
        """
        if force:
            content_data.encoding = default_encoding  # type: ignore
            return default_encoding
        elif content_data.encoding:  # type: ignore
            content_data_encoding_lower = content_data.encoding.lower()  # type: ignore
            if default_encoding and (content_data_encoding_lower != default_encoding.lower()):
                logger.warning(
                    f"Encoding in OCS object header {content_data.encoding} "  # type: ignore
                    f"contradicts requested encoding {default_encoding}.")
            return content_data_encoding_lower  # type: ignore
        elif default_encoding:  # type: ignore
            # In the "text" property of the oci package, it says:
            #  "The encoding of the response content is determined based solely on HTTP
            #  headers, following RFC 2616 to the letter. If you can take advantage of
            #  non-HTTP knowledge to make a better guess at the encoding, you should
            #  set ``r.encoding`` appropriately before accessing this property."
            # If we don't set it, the loading takes forever because the library tries
            # to detect the encoding for the whole data.
            content_data.encoding = default_encoding  # type: ignore
            return default_encoding
        else:
            logging.getLogger(__name__).warning(
                f"Encoding type was not supplied and is also not set in the object. "
                f"This might make reading this file really slow. Please supply "
                f"it or make sure the object you are trying to read contains it in the metadata.")
            return None

    def read_csv_object_into_dataframe(self, object_name: str,
                                       selected_columns: Optional[ColumnTypes] = None,
                                       default_encoding: Optional[str] = DEFAULT_ENCODING,
                                       force_default_encoding: Optional[bool] = False,
                                       **pandas_kwargs: Optional[Any]) -> pd.DataFrame:
        """
        Read CSV directly into a Pandas DataFrame.

        :param object_name: Name of the CSV object.
        :param selected_columns: Subset of columns to be loaded from the CSV object.
        :param default_encoding: Encoding to use for Pandas data frame and for reading from OCS.
            The encoding in Content-type of the object on OCS, if it's provided, must be equal
            to this value. If encoding is None and Content-type is NOT in the header,
            then try to autodetect encoding (in-built feature of .text property -
            caution: it is very slow).
        :param force_default_encoding: If set to True, the default encoding is enforced even if
            content_data encoding is set.
        :param pandas_kwargs: Other Pandas arguments for `pd.read_csv()` apart from "encoding".
        :return: A Pandas DataFrame with data from the CSV object.
        """
        content_data = self.object_storage.get_object(
            self.namespace, self.bucket_name, object_name)
        content_data = content_data.data

        actual_encoding = self._ensure_encoding(
            content_data, default_encoding, force_default_encoding)

        content = content_data.text
        if content:
            df = pd.read_csv(StringIO(content),
                             header=0,
                             usecols=selected_columns,
                             encoding=actual_encoding,
                             **pandas_kwargs)
        else:
            df = pd.DataFrame(columns=selected_columns)
        return df

    def read_binary_object_into_dataframe(self, object_name: str,
                                          selected_columns: Optional[ColumnTypes] = None,
                                          **pandas_kwargs: Optional[Any]) -> pd.DataFrame:
        """
        Read binary object directly into a Pandas DataFrame.

        This can be used for example for compressed files or other binary formats supported by
        pandas. It should be accompanied with the appropriate pandas_kwargs.

        :param object_name: Name of the object.
        :param selected_columns: Subset of columns to be loaded from the object.
        :param pandas_kwargs: Other Pandas arguments for `pd.read_csv()` apart from "encoding".
        :return: A Pandas DataFrame with data from the object.
        """
        content_data = self.object_storage.get_object(self.namespace, self.bucket_name,
                                                      object_name).data

        if content_data.content:
            bytes_stream = BytesIO(content_data.content)
            df = pd.read_csv(bytes_stream, header=0, usecols=selected_columns, **pandas_kwargs)
        else:
            df = pd.DataFrame(columns=selected_columns)
        return df

    def save_object(self, content: Any,
                    object_name: str,
                    content_type: Optional[str] = None,
                    content_encoding: Optional[str] = None) -> Response:
        """
        Save any content into an object in Oracle Cloud Storage (OCS).

        :param content: Content to be saved.
        :param object_name: Name of the object to be saved.
        :param content_type: Content type of the saved object (e.g. "text/csv; charset=utf-8").
            Defaults to 'application/octet-stream' if not set.
        :param content_encoding: Content encoding of the saved object (e.g. "gzip")
        :return: Response from OCS.
        """
        return self.object_storage.put_object(namespace_name=self.namespace,
                                              bucket_name=self.bucket_name,
                                              object_name=object_name,
                                              put_object_body=content,
                                              content_type=content_type,
                                              content_encoding=content_encoding)

    def save_dataframe_as_csv(self,
                              df: pd.DataFrame,
                              object_name: str,
                              save_index: bool = False,
                              **pandas_kwargs: Optional[Any]) -> Response:
        """
        Save Pandas DataFrame as a CSV object in Oracle Cloud Storage (OCS).

        :param df: Pandas DataFrame to be saved.
        :param object_name: Name of the resulting CSV.
        :param save_index: If true, save the index of the Pandas DataFrame in the CSV file.
        :param pandas_kwargs: Pandas arguments for `pd.to_csv()`.
        :return: Response from OCS.
        """
        # This compression workaround is needed because of
        # https://github.com/pandas-dev/pandas/issues/22555

        bytes_io, content_encoding, content_type = self._df_to_bytes_io(
            df, save_index, **pandas_kwargs)

        return self.save_object(bytes_io,
                                object_name,
                                content_type=content_type,
                                content_encoding=content_encoding)

    def _df_to_bytes_io(self,
                        df: pd.DataFrame,
                        save_index: bool,
                        **pandas_kwargs: Optional[Any]) -> Tuple[BytesIO, Optional[str], str]:
        """
        Convert pandas DataFrame to a BytesIO object that can be used as an in memory file.

        :param df: DataFrame to be converted.
        :param save_index: Flag defining whether to save the index of the DataFrame.
        :param pandas_kwargs: Arguments to pe passed to pandas.
        :return: Tuple containing the BytesIO object with the content of the CSV generated from
          the DataFrame and the corresponding content_encoding and content_type values to be used
          when saving in OCI
        """
        compression = pandas_kwargs.get('compression') is not None
        if compression:
            pandas_kwargs.pop('compression')
            logging.getLogger(__name__).info('Using gzip compression (only supported compression '
                                             'type at the moment.)')
        str_buf = StringIO()
        df.to_csv(str_buf, index=save_index, **pandas_kwargs)
        str_buf.seek(0)
        df_csv_str = str_buf.read()
        encoding = str(pandas_kwargs.get('encoding', DEFAULT_ENCODING))
        encoded_df = df_csv_str.encode(encoding)
        bytes_io, content_encoding = self._bytes_to_bytes_io(encoded_df, compression)
        content_type = f"text/csv; charset={encoding}"
        return bytes_io, content_encoding, content_type

    @staticmethod
    def _bytes_to_bytes_io(
            bytes_obj: bytes, compression: bool = False) -> Tuple[BytesIO, Optional[str]]:
        """
        Convert a bytes object to a BytesIO object with optional compression.

        :param bytes_obj: Bytes object to be converted.
        :param compression: Enable gzip compression (defaults to False).
        :return: Tuple with the BytesIO object and the corresponding content encoding.
        """
        if compression:
            bytes_io = BytesIO()
            with gzip.open(bytes_io, 'wb') as f:
                f.write(bytes_obj)
            bytes_io.seek(0)
            # TODO: this is the actually recommended value for a gzipped file, but unfortunately
            #  the python library has no support for reading a gzipped file without automatic
            #  decompression (the REST api does though), so we need to mask this for now.
            # content_encoding = f"gzip"
            content_encoding = None
        else:
            bytes_io = BytesIO(bytes_obj)
            content_encoding = None
        return bytes_io, content_encoding

    def read_object_bytes(self, object_name: str) -> bytes:
        """
        Read a binary file from Oracle Cloud Storage (OCS).

        :param object_name: Name of the object containing the pickle file.
        :return: bytes object containing the content of the file.
        """
        obj = self.object_storage.get_object(self.namespace, self.bucket_name, object_name)
        bytes_stream = BytesIO(obj.data.content)
        object_content = bytes_stream.read()
        return object_content

    def read_joblib_pickle(self, object_name: str) -> Any:
        """
        Read a pickle file from Oracle Cloud Storage (OCS) with joblib.

        :param object_name: Name of the object containing the pickle file.
        :return: Unpickled content of the file.
        """
        obj = self.object_storage.get_object(self.namespace, self.bucket_name, object_name)
        unpickled_data = joblib.load(BytesIO(obj.data.content))
        return unpickled_data

    def read_text_object(self, object_name: str,
                         default_content_encoding: Optional[str] = DEFAULT_ENCODING,
                         force_default_encoding: Optional[bool] = False) -> str:
        """
        Read a text file from Oracle Cloud Storage (OCS).

        :param object_name: Name of the text file.
        :param default_content_encoding: If encoding isn't provided in Content-type of the object,
            set it to this value. If None, then try to autodetect it (caution: it is very slow).
        :param force_default_encoding: If set to True, the default encoding is enforced even if
            content_data encoding is set.
        :return: Text content as string.
        """
        obj = self.object_storage.get_object(self.namespace, self.bucket_name, object_name)
        obj_data = obj.data
        if default_content_encoding:
            self._ensure_encoding(obj_data, default_content_encoding, force=force_default_encoding)
        return obj_data.text

    def delete_bucket(self) -> None:
        """Delete a bucket from Oracle Cloud Storage (OCS)."""
        objects_in_bucket = self.get_objects_with_prefix('', include_dirs=True)
        for bucket_object in objects_in_bucket:
            self.delete_object(bucket_object.name)
        self.object_storage.delete_bucket(self.namespace, self.bucket_name)

    def delete_object(self, object_name: str) -> None:
        """
        Delete an object from Oracle Cloud Storage (OCS).

        :param object_name: Name of the object to be deleted.
        """
        self.object_storage.delete_object(self.namespace, self.bucket_name, object_name)


class TemporaryOcsBucket:
    """Temporary OCS bucket for test fixtures."""

    def __init__(self,
                 config_file: Optional[str] = None,
                 compartment_id: Optional[str] = None,
                 bucket_name_prefix: str = '',
                 timeout: Optional[Union[float, Tuple[float, float]]] = DEFAULT_TIMEOUT):
        """
        Create a temporary OCS bucket.

        :param config_file: OCI configuration file to use.
        :param compartment_id: ID of the compartment that will contain the bucket.
        :param bucket_name_prefix: Prefix to be used in the bucket name generation.
        :param timeout: (optional) The connection and read timeouts for the client.
            The default values are connection timeout 10 seconds and read timeout 60 seconds.
            This keyword argument can be provided as a single float, in which case the value
            provided is used for both the read and connection timeouts, or as a tuple of two
            floats. If a tuple is provided then the first value is used as the connection timeout
            and the second value as the read timeout.
        :type timeout: float or tuple(float, float)
        """
        self.bucket_name_prefix = bucket_name_prefix
        self.compartment_id = compartment_id
        self.config_file = config_file
        random_suffix = uuid.uuid4().hex
        bucket_name = f'{bucket_name_prefix}_{random_suffix}'
        self.bucket = OcsBucket(config_file, bucket_name, timeout=timeout).create_bucket(
            compartment_id)

    def __enter__(self) -> OcsBucket:
        """Return the bucket."""
        return self.bucket

    def __exit__(self,
                 exc_type: Optional[Type[BaseException]],
                 exc_val: Optional[BaseException],
                 exc_tb: Optional[TracebackType]) -> None:
        """Delete the bucket."""
        self.bucket.delete_bucket()


class OciHelper:
    """OCI Helper."""

    @classmethod
    def is_resource_principals_enabled(cls) -> bool:
        """
        Check, if resource principals is enabled.`

        If AIACS_RESOURCE_PRINCIPALS_ENABLED is set and not None, True is returned.
        """
        if os.getenv("OCI_RESOURCE_PRINCIPAL_VERSION") is not None:
            return True
        else:
            return False

    @classmethod
    def use_model_prefix(cls) -> bool:
        """
        Check, if templates prefix should be used.

        If USE_MODEL_PREFIX is set and not None, True is returned.
        """
        model_prefix = os.environ.get(USE_MODEL_PREFIX_ENV)

        if model_prefix is not None and model_prefix.lower() != 'false':
            return True
        else:
            return False

    @classmethod
    def is_oci_model_deployment(cls) -> bool:
        """
        Check, if it is running from OCI Model Deployment.

        If OCI_MODEL_DEPLOYMENT_ENV is set and not None, True is returned.
        """
        if os.environ.get(OCI_MODEL_DEPLOYMENT_ENV) is not None:
            return True
        else:
            return False

    @classmethod
    def get_athena_or_oci_file_name(cls, file_name: str) -> str:
        """
        Prefix the file name with 'oci_' if USE_MODEL_PREFIX is set and not None.

        :param file_name: The name of the file to be prefixed.
        :return: The file name prefixed with 'oci_' if USE_MODEL_PREFIX is set
         and not None, or unmodified otherwise.
        """
        effective_file_name = file_name
        if cls.use_model_prefix():
            effective_file_name = OCI_PREFIX + effective_file_name
        return effective_file_name
