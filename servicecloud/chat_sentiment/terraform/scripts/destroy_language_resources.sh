#!/usr/bin/env bash
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
#  SHA1: $Id: 6c7d1f1ee31cd967fcd3057a0af2430ec3e89150 $
################################################################################################
#  File: destroy_language_resources.sh
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

echo "*** Delete All Active Language Endpoints ***"
RESULT=$(oci ai language endpoint list -c "${COMPARTMENT_ID}" --all)
ENDPOINTS=$(echo "${RESULT}" | jq '.data.items')

echo "${ENDPOINTS}" | jq -c '.[]' | while read ENDPOINT; do
  # do stuff with $i
  ENDPOINT_ID=$(echo "${ENDPOINT}" | jq '."id"')
  DISPLAY_NAME=$(echo "${ENDPOINT}" | jq '."display-name"')
  STATE=$(echo "${ENDPOINT}" | jq '."lifecycle-state"')

  if [[ "$STATE" != *"DELETED"* ]]; then
    ENDPOINT_ID=$(echo "$ENDPOINT_ID" | sed -e 's/^"//' -e 's/"$//')
    echo "${DISPLAY_NAME} - ${STATE}"
    RESPONSE=$(oci ai language endpoint delete --force --endpoint-id "${ENDPOINT_ID}")
    echo "${RESPONSE}"
    echo "*** Deleted ${DISPLAY_NAME} ***"
  fi

done


echo "*** Delete All Active Language Models ***"
RESULT=$(oci ai language model list -c "${COMPARTMENT_ID}" --all)
MODELS=$(echo "${RESULT}" | jq '.data.items')

echo "${MODELS}" | jq -c '.[]' | while read MODEL; do
  # do stuff with $i
  MODEL_ID=$(echo "${MODEL}" | jq '."id"')
  DISPLAY_NAME=$(echo "${MODEL}" | jq '."display-name"')
  STATE=$(echo "${MODEL}" | jq '."lifecycle-state"')

  if [[ "$STATE" != *"DELETED"* ]]; then
    MODEL_ID=$(echo "$MODEL_ID" | sed -e 's/^"//' -e 's/"$//')
    echo "${DISPLAY_NAME} - ${STATE}"
    RESPONSE=$(oci ai language model delete --force --model-id "${MODEL_ID}")
    echo "${RESPONSE}"
    echo "*** Deleted ${DISPLAY_NAME} ***"
  fi

done



echo "*** Delete All Active Language Projects ***"
RESULT=$(oci ai language project list -c "${COMPARTMENT_ID}" --all)
PROJECTS=$(echo "${RESULT}" | jq '.data.items')

echo "${PROJECTS}" | jq -c '.[]' | while read PROJECT; do
  # do stuff with $i
  PROJECT_ID=$(echo "${PROJECT}" | jq '."id"')
  DISPLAY_NAME=$(echo "${PROJECT}" | jq '."display-name"')
  STATE=$(echo "${PROJECT}" | jq '."lifecycle-state"')

  if [[ "$STATE" != *"DELETED"* ]]; then
    PROJECT_ID=$(echo "$PROJECT_ID" | sed -e 's/^"//' -e 's/"$//')
    echo "${DISPLAY_NAME} - ${STATE}"
    RESPONSE=$(oci ai language project delete --force --project-id "${PROJECT_ID}")
    echo "${RESPONSE}"
    echo "*** Deleted ${DISPLAY_NAME} ***"
  fi

done


