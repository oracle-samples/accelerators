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
#  SHA1: $Id: b17ca011ae0d713ce16a8f3cd1f159d5b1a99666 $
################################################################################################
#  File: retrieve_project.sh
################################################################################################

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

PROJECT_FILE_NAME=$4
if [[ -z "$PROJECT_FILE_NAME" ]]; then
  PROJECT_FILE_NAME="project.txt"
fi

PROJECT_FILE="${PROJECT_LOCATION}/${PROJECT_FILE_NAME}"

echo "*** Creating project on display-name as ${PROJECT_NAME} ***"
RESULT=$(oci ai language project create -c "${COMPARTMENT_ID}" --display-name "${PROJECT_NAME}")
PROJECT_ID=$(echo "${RESULT}" | jq '.data."id"')
PROJECT_ID=$(echo "$PROJECT_ID" | sed -e 's/^"//' -e 's/"$//')
printf "%s" "${PROJECT_ID}" > "${PROJECT_FILE}"
echo "*** LANGUAGE PROJECT ID ${PROJECT_ID} ***"
