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
#  date: Mon Jun 26 10:43:28 IST 2023
 
#  revision: rnw-23-02-initial
#  SHA1: $Id: cf365c4e44365d37429996208e8b1c74fc294eff $
################################################################################################
#  File: destroy_jobs.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail

COMPARTMENT_ID=$1
if  [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi

function checkJobCancelled {

    count=0
    max=50
    STATE="CHECK"
    #STATUS ALLOWED: ACCEPTED, CANCELED, CANCELING, DELETED, FAILED, IN_PROGRESS, NEEDS_ATTENTION, SUCCEEDED
    while [[ ${count} -lt ${max} ]]; do
        count=$((count + 1))
        echo "*** Retrieving status try ... ${count} *** "
        STATE=$(oci data-science job-run get --job-run-id "$1" | jq '.data."lifecycle-state"')
        echo "*** STATE: ${STATE}"
        [[ ${STATE} == *"CANCELED"* ]] || [[ ${STATE} == *"DELETED"* ]] && break
        sleep 120
    done
    return $?
}

# Deleting the JOB RUNS

RESULT=$(oci data-science job-run list -c "${COMPARTMENT_ID}")
JOBS=$(echo "${RESULT}" | jq '.data')

echo "${JOBS}" | jq -c '.[]' | while read JOB; do
    # do stuff with $i
  JOB_ID=$(echo "${JOB}" | jq '."id"')
  DISPLAY_NAME=$(echo "${JOB}" | jq '."display-name"')
  STATE=$(echo "${JOB}" | jq '."lifecycle-state"')

  if [[ "$DISPLAY_NAME" == *"build-model"* ]]; then
    echo "*** Skip ${DISPLAY_NAME} ***"
  else
    if [[ "$STATE" != *"DELETED"* ]]; then
      JOB_ID=$(echo $JOB_ID | sed -e 's/^"//' -e 's/"$//')
      echo "${DISPLAY_NAME} - ${STATE}"
      if [[ "$STATE" == *"IN_PROGRESS"* ]] || [[ "$STATE" == *"ACCEPTED"* ]]; then
        RESPONSE=$(oci data-science job-run cancel --job-run-id "${JOB_ID}")
        echo "${RESPONSE}"
        echo "${JOB_ID}"
        checkJobCancelled "${JOB_ID}"
        echo "*** Cancelled ${DISPLAY_NAME} ***"
        RESPONSE=$(oci data-science job-run delete --force --job-run-id "${JOB_ID}")
        echo "${RESPONSE}"
        echo "*** Deleted ${DISPLAY_NAME} ***"
      else
        if [[ "$STATE" == *"CANCELING"* ]]; then
          checkJobCancelled "${JOB_ID}"
        fi
        RESPONSE=$(oci data-science job-run delete --force --job-run-id "${JOB_ID}")
        echo "${RESPONSE}"
        echo "*** Deleted ${DISPLAY_NAME} ***"
      fi
    fi
  fi

done


# Deleting the JOBS

RESULT=$(oci data-science job list -c "${COMPARTMENT_ID}")
JOBS=$(echo "${RESULT}" | jq '.data')

echo "${JOBS}" | jq -c '.[]' | while read JOB; do
    # do stuff with $i
  JOB_ID=$(echo "${JOB}" | jq '."id"')
  DISPLAY_NAME=$(echo "${JOB}" | jq '."display-name"')
  STATE=$(echo "${JOB}" | jq '."lifecycle-state"')

  if [[ "$DISPLAY_NAME" == *"build-model"* ]]; then
    echo "*** Skip ${DISPLAY_NAME} ***"
  else
    if [[ "$STATE" != *"DELETED"* ]]; then
      JOB_ID=$(echo $JOB_ID | sed -e 's/^"//' -e 's/"$//')
      echo "${DISPLAY_NAME} - ${STATE}"
      RESPONSE=$(oci data-science job delete --force --job-id "${JOB_ID}")
      echo "${RESPONSE}"
      echo "*** Deleted ${DISPLAY_NAME} ***"
    fi
  fi

done
