#!/usr/bin/env bash
################################################################################################
#  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
#  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23A (February 2023) 
#  date: Mon Jun 26 10:43:26 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: ab62ee95d2c254fcadfe07e7212ba07a22b629a3 $
################################################################################################
#  File: run_selenium.sh
################################################################################################
# *** messages for status
# !!! messages for success
# ### messages for failure

set -eo pipefail

if [ -z "$OCI_USER_ID" ]; then
  echo "Please share OCI_USER_ID as env variable"
  exit 1
fi


echo "*** Creating Conda Enviornment with ${OCI_USER_ID} ***"
sudo yum install yum-utils
sudo yum-config-manager --enable public_ol6_addons
sudo yum -y install jq
echo "*** JQ Installed ***"

# Set the path to the OCI directory
OCI_DIR="./oci"
CONDA_ENV_NAME="b2c_env"
CONFIG_FILE=./oci/config
KEY_FILE=./oci/key.pem

mkdir -p $OCI_DIR

echo "[DEFAULT]" > $CONFIG_FILE;

echo "*** Retrieving Private Key ***"
export OCI_CLI_AUTH=resource_principal
OCI_PRIVATE_KEY=$(oci secrets secret-bundle get --secret-id "${OCI_PRIVATE_KEY}" | jq -r '.data."secret-bundle-content".content' | base64 -d)
echo "$OCI_PRIVATE_KEY" > $KEY_FILE
echo "*** Private Key Retrieved ***"

echo "user=${OCI_USER_ID}" >> $CONFIG_FILE
echo "fingerprint=${OCI_FINGERPRINT}" >> $CONFIG_FILE
echo "key_file=${KEY_FILE}" >> $CONFIG_FILE
echo "tenancy=${OCI_TENANCY_ID}" >> $CONFIG_FILE
echo "region=${OCI_REGION}" >> $CONFIG_FILE

# Set the environment variables
export OCI_CLI_PROFILE_FILE=./oci/config

# moving the config to default directory
sudo mkdir -p "${HOME}/.oci"/
sudo cp -r ${OCI_DIR}/* "${HOME}/.oci"/

echo "*** Downloading the enviornment yaml file ***"
curl -o environment.yml "${RAW_GIT_ENV_URL}"

echo "*** CREATING CONDA ENV ***"
odsc conda create --file ./environment.yml --name ${CONDA_ENV_NAME} --slug ${CONDA_ENV_NAME}_slug
sleep 2
echo "*** INIT CONDA ENV ***"
odsc conda init -b "${BUCKET_NAME}" -n "${NAMESPACE}" -a api_key --api_key_profile "DEFAULT" --api_key_config ${CONFIG_FILE}
sleep 2
echo "*** PUBLISH CONDA ENV ***"
odsc conda publish -s ${CONDA_ENV_NAME}_slug
sleep 10
echo "!!! PUBLISHED !!!"

