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
#  SHA1: $Id: 16a41e19da0489444fe93d6c7de8825f21315e0d $
################################################################################################
#  File: retrieve_namespace.sh
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
NAMESPACE=$(oci os ns get -c "${COMPARTMENT_ID}" | jq '.data' | sed 's/"//g')
printf '%s' "${NAMESPACE}" > namespace.txt
