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
#  SHA1: $Id: b4402b41b4a825def87a12be852fe31ab56deb2f $
################################################################################################
#  File: bucket_vault.py
################################################################################################
"""Classes for accessing OCS bucket data."""

import logging
import uuid
from pathlib import Path
from types import TracebackType
from typing import Optional, Type, Union, Tuple


# Default timeout value(second)
from ai4service_automated_classification.utils.object_storage.os import OciHelper, OcsBucket

DEFAULT_CONNECTION_TIMEOUT = 10.0
DEFAULT_READ_TIMEOUT = 60.0
DEFAULT_TIMEOUT = (DEFAULT_CONNECTION_TIMEOUT, DEFAULT_READ_TIMEOUT)


class TemporaryOcsBucketVault:
    """Temporary OCS bucket (with OCS credentials taken from Vault) for test fixtures."""

    def __init__(self,
                 vault_server_address: str,
                 vault_role: str,
                 compartment_id: Optional[str] = None,
                 bucket_name_prefix: str = '',
                 auth_token: Optional[str] = None,
                 timeout: Optional[Union[float, Tuple[float, float]]] = DEFAULT_TIMEOUT):
        """
        Create a temporary OCS bucket given credentials stored in vault.

        :param vault_server_address: Address for vault server.
        :param vault_role: Role to be used when authenticating with vault.
        :param compartment_id: OCS compartment id.
        :param bucket_name_prefix: Prefix to bse used for the bucket name.
        :param auth_token: Vault authorization token.
        :param timeout: The connection and read timeouts for the client.
        :type timeout: float or tuple(float, float)
        """
        self.bucket_name_prefix = bucket_name_prefix
        self.compartment_id = compartment_id
        random_suffix = uuid.uuid4().hex
        bucket_name = f'{bucket_name_prefix}_{random_suffix}'
        # TODO create bucket from vault
        # self.bucket = OcsBucketVault(
        #     auth_token=auth_token,
        #     vault_server_address=vault_server_address,
        #     vault_role=vault_role,
        #     bucket_name=bucket_name, timeout=timeout).create_bucket(compartment_id)

    def __enter__(self) -> OcsBucket:
        """Return the bucket."""
        return self.bucket

    def __exit__(self,
                 exc_type: Optional[Type[BaseException]],
                 exc_val: Optional[BaseException],
                 exc_tb: Optional[TracebackType]) -> None:
        """Delete the bucket."""
        self.bucket.delete_bucket()


def set_up_ocs_connection(ocs_bucket: str,
                          timeout: Optional[
                              Union[float, Tuple[float, float]]] = DEFAULT_TIMEOUT) -> OcsBucket:
    """
    Set up an OCS connection using Vault if available.

    :param ocs_bucket: Name or OCS bucket object.
    :param timeout: The connection and read timeouts for the client.
    :type timeout: float or tuple(float, float)
    :return: The name of the OCS bucket or an OcsBucketVault object.
    """
    errors = 0
    if not (Path.home() / '.oci/config').is_file():
        logging.getLogger(__name__).error(f"{Path.home()}/.oci/config does not exist!")
        errors += 1
    if not (Path.home() / '.oci/oci_api_key.pem').is_file():
        logging.getLogger(__name__).error(
            f'{Path.home()}/.oci/oci_api_key.pem does not exist!')
        errors += 1

    # ODSC-17596
    if errors and not OciHelper.is_resource_principals_enabled():
        raise FileNotFoundError("Required OCI configuration files not found!")

    return OcsBucket(bucket_name=ocs_bucket, timeout=timeout)
