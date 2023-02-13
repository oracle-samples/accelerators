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
#  date: Tue Jan 31 12:34:02 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: 053e278427b9a67ff9bb4e1d314944846f20e617 $
################################################################################################
#  File: destroy_models.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail

COMPARTMENT_ID=$1
if [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi

RESULT=$(oci data-science model list -c "${COMPARTMENT_ID}" --all)
MODELS=$(echo "${RESULT}" | jq '.data')

echo "${MODELS}" | jq -c '.[]' | while read MODEL; do
  # do stuff with $i
  MODEL_ID=$(echo "${MODEL}" | jq '."id"')
  DISPLAY_NAME=$(echo "${MODEL}" | jq '."display-name"')
  STATE=$(echo "${MODEL}" | jq '."lifecycle-state"')

  if [[ "$STATE" == *"ACTIVE"* ]]; then
    MODEL_ID=$(echo "$MODEL_ID" | sed -e 's/^"//' -e 's/"$//')
    echo "${DISPLAY_NAME} - ${STATE}"
    RESPONSE=$(oci data-science model delete --force --model-id "${MODEL_ID}")
    echo "${RESPONSE}"
    echo "*** Deleted ${DISPLAY_NAME} ***"
  fi

done
