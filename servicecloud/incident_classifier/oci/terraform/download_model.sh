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
#  date: Tue Jan 31 13:02:56 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 23df1ed3a162596910b00a9358c0ca31a8ec50fb $
################################################################################################
#  File: download_model.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure

set -eo pipefail

COMPARTMENT_ID=$1
MODEL_LOCATION=$2
MODEL_FILE="${MODEL_LOCATION}/templates.pkl"

if [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi

if [[ -z "$MODEL_LOCATION" ]]; then
  echo "*** Please set variable MODEL_LOCATION (location of model file) in environment ***"
  exit 1
fi

TIMESTAMP=$(date -u +%FT%T)
FIVE_MINUTE_FORWARD_DIFFERENCE=300
TEN_MINUTE_BACKWARD_DIFFERENCE=-1200
DIFF_IN_SECONDS=0

function checkModelAvailable() {
  while [[ ! -f "$MODEL_FILE" ]]; do
    if test -f "$MODEL_FILE"; then
      echo "Saved Model File Location: ${MODEL_FILE}"
      break
    fi
    count=$((count + 1))
    echo "*** Retrieving model try ... ${count} *** "
    RESULTS=$(oci data-science model list -c "$COMPARTMENT_ID" --all --lifecycle-state "ACTIVE" | jq -r '.data' || echo "[]")
    TOTAL_MODELS=$(echo "$RESULTS" | jq length)
    echo "*** TOTAL_MODELS: ${TOTAL_MODELS}"
    echo "$RESULTS" | jq -c '.[]' | while read -r MODEL; do
      MODEL_ID=$(echo "$MODEL" | jq -r '.id')
      CREATED_TIME=$(echo "$MODEL" | jq -r '."time-created"')
      START_TIME=$(date -d "${TIMESTAMP}" "+%s")
      END_TIME=$(date -d "${CREATED_TIME}" "+%s")
      DIFF_IN_SECONDS="$((END_TIME - START_TIME))"
      if [[ ${DIFF_IN_SECONDS} -gt ${TEN_MINUTE_BACKWARD_DIFFERENCE} ]]; then
        DIFF_IN_SECONDS=${DIFF_IN_SECONDS#-}
        echo "Diff in seconds: $DIFF_IN_SECONDS"
      else
        echo "Diff in seconds: $DIFF_IN_SECONDS"
      fi
      if [[ ${DIFF_IN_SECONDS} -gt ${FIVE_MINUTE_FORWARD_DIFFERENCE} ]]; then
        echo "*** Model To Download: ${MODEL_ID} ***"
        rm ./artifacts.zip || echo "No Existing Artifact File"
        oci data-science model get-artifact-content --model-id "${MODEL_ID}" --file ./artifacts.zip
        unzip -o artifacts.zip -d "${MODEL_LOCATION}"
        echo "!!! Model Downloaded at ${MODEL_LOCATION} !!!"
        sleep 20
        break
      fi
    done
    sleep 10
  done
  return $?
}

checkModelAvailable
