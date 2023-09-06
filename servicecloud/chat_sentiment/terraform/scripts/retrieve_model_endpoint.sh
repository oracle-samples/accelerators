#!/bin/bash
################################################################################################
#  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
#  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
#  http://oss.oracle.com/licenses/upl
#  Copyright (c) 2023, Oracle and/or its affiliates.
################################################################################################
#  Accelerator Package: Incident Text Based Classification
#  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
#  OSvC release: 23C (August 2023) 
#  date: Tue Aug 22 11:57:51 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 9d7dfbde0499f5f0719f0d1a40efe1e7b38f5fb9 $
################################################################################################
#  File: retrieve_model_endpoint.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail

MODEL_ENDPOINT_NAME=$1
if [[ -z "$MODEL_ENDPOINT_NAME" ]]; then
  echo "*** Please set variable MODEL_ENDPOINT_NAME in environment ***"
  exit 1
fi

COMPARTMENT_ID=$2
if [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi

MODEL_LOCATION=$3
if [[ -z "$MODEL_LOCATION" ]]; then
  echo "*** Please set variable MODEL_LOCATION in environment ***"
  exit 1
fi

MODEL_FILE="${MODEL_LOCATION}/model.txt"

while [ ! -f "$MODEL_FILE" ]; do
  echo "Waiting for $MODEL_FILE to be available..."

  echo "*** Getting Endpoint based on display-name as ${MODEL_ENDPOINT_NAME} ***"
  RESULT=$(oci ai language endpoint list -c "${COMPARTMENT_ID}" --all --lifecycle-state "ACTIVE" --display-name "${MODEL_ENDPOINT_NAME}")
  ENDPOINTS=$(echo "${RESULT}" | jq '.data.items')

  echo "${ENDPOINTS}" | jq -c '.[]' | while read ENDPOINT; do
    # do stuff with $i
    ENDPOINT_ID=$(echo "${ENDPOINT}" | jq '."id"')
    DISPLAY_NAME=$(echo "${ENDPOINT}" | jq '."display-name"')
    STATE=$(echo "${ENDPOINT}" | jq '."lifecycle-state"')

    if [[ "$STATE" == *"ACTIVE"* ]]; then
      ENDPOINT_ID=$(echo "$ENDPOINT_ID" | sed -e 's/^"//' -e 's/"$//')
      printf "%s" "${ENDPOINT_ID}" >"${MODEL_FILE}"
      echo "*** MODEL ENDPOINT ${ENDPOINT_ID} ***"
    fi

  done

  sleep 60
done

echo "*** $MODEL_FILE is now available. ***"
