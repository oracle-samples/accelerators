################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:25 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 85177f6369cf2e2dd20fad43263893018828f3cd $
################################################################################################
#  File: conda_environment.py
################################################################################################
import os
import logging
import subprocess
import sys
from os.path import dirname, abspath

base_dir = dirname(dirname(dirname(abspath(__file__))))
sys.path.append(base_dir)

from training_scripts.constants import BUCKET_URL, LOGGING_FORMAT, CONDA_ENV_NAME
from training_scripts.utils import set_config
from ai4service_automated_classification.utils.object_storage.bucket_vault import set_up_ocs_connection
from ai4service_automated_classification.utils.object_storage.os import OcsBucket
from ai4service_automated_classification.utils.object_storage.utils import parse_ocs_uri

logging.basicConfig(level=logging.INFO, format=LOGGING_FORMAT)
logger = logging.getLogger(os.path.basename(__file__))


def is_conda_env_exists(ocs_client: OcsBucket):
    return ocs_client.is_directory('conda_environments/')


def create_conda_env():
    logger.info("*** Creating conda enviornment... ***")
    CONDA_RESOURCE_PATH = os.path.join(base_dir, 'environment.yml')
    logger.info(f"*** CONDA ENVIRONMENT FILE : {CONDA_RESOURCE_PATH}**")
    remove_conda_process = subprocess.Popen('rm -rf ./conda', shell=True, stdout=subprocess.PIPE)
    remove_conda_process.wait()
    output = subprocess.check_output(["odsc", "conda", "create",
                                      "--file", CONDA_RESOURCE_PATH, "--name",
                                      CONDA_ENV_NAME, "--slug", f"{CONDA_ENV_NAME}_slug"])
    logger.info(output)
    logger.info(f"*** Conda environment {CONDA_ENV_NAME}_slug created. ***")


def default_slug_path(ocs_client: OcsBucket):
    conda_path = os.path.join(f"{ocs_client.bucket_name}@{ocs_client.namespace}", 'conda_environments')
    conda_architecture_path = os.path.join(conda_path, 'cpu')
    conda_env_path = os.path.join(conda_architecture_path, CONDA_ENV_NAME)
    conda_version_path = os.path.join(conda_env_path, "1.0")
    conda_slug_path = os.path.join(conda_version_path, f"{CONDA_ENV_NAME}_slug")
    return CONDA_ENV_NAME, conda_slug_path


def publish_conda_env(ocs_client: OcsBucket):
    logger.info(f"!!! Pushing the conda environment to the bucket: {ocs_client.bucket_name} !!!")
    conda_init_process = subprocess.Popen(
        f'odsc conda init -b {ocs_client.bucket_name} -n {ocs_client.namespace} --authentication resource_principal',
        shell=True, stderr=subprocess.PIPE)
    conda_init_process.wait()
    if conda_init_process.returncode == 0:
        output = subprocess.check_output(["odsc", "conda", "publish",
                                          "-s", f"{CONDA_ENV_NAME}_slug"])
        logger.info(output)
        logger.info(f"*** Conda enviornment pushed to {ocs_client.bucket_name} ***")
    else:
        logger.error("### Conda publish failed. try to do it manually")
    return default_slug_path(ocs_client)


def get_conda_env():
    data_path = parse_ocs_uri(os.getenv(BUCKET_URL))
    ocs_client = set_up_ocs_connection(data_path.ocs_bucket)
    if not is_conda_env_exists(ocs_client):
        create_conda_env()
        conda_env = publish_conda_env(ocs_client)
    else:
        conda_env = default_slug_path(ocs_client)
    return conda_env


if __name__ == '__main__':
    set_config()
    get_conda_env()
