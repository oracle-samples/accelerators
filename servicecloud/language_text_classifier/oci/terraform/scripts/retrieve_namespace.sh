#!/bin/bash
# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail

COMPARTMENT_ID=$1
if [[ -z "$COMPARTMENT_ID" ]]; then
  echo "*** Please set variable COMPARTMENT_ID in environment ***"
  exit 1
fi
NAMESPACE=$(oci os ns get -c "${COMPARTMENT_ID}" | jq '.data' | sed 's/"//g')
printf '%s' "${NAMESPACE}" > namespace.txt
