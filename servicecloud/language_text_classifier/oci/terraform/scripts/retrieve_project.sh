#!/bin/bash
# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail


PROJECT_NAME=$1
if [[ -z "$PROJECT_NAME" ]]; then
  echo "*** Please set variable PROJECT_NAME in environment ***"
  exit 1
fi

COMPARTMENT_ID=$2
if [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi

PROJECT_LOCATION=$3
if [[ -z "$PROJECT_LOCATION" ]]; then
  echo "*** Please set variable PROJECT_LOCATION in environment ***"
  exit 1
fi

PROJECT_FILE="${PROJECT_LOCATION}/project.txt"

echo "*** Creating project on display-name as ${PROJECT_NAME} ***"
RESULT=$(oci ai language project create -c "${COMPARTMENT_ID}" --display-name "${PROJECT_NAME}")
PROJECT_ID=$(echo "${RESULT}" | jq '.data."id"')
PROJECT_ID=$(echo "$PROJECT_ID" | sed -e 's/^"//' -e 's/"$//')
printf "%s" "${PROJECT_ID}" > "${PROJECT_FILE}"
echo "*** LANGUAGE PROJECT ID ${PROJECT_ID} ***"
