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
#  SHA1: $Id: c4a82c487076c61d6a83d7d6bf2b7ab74e8e61ec $
################################################################################################
#  File: destroy_logs.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure
set -eo pipefail

LOG_GROUP_ID=$1
if  [[ -z "$LOG_GROUP_ID" ]]; then
  echo "*** Please set variable LOG_GROUP_ID in environment ***"
  exit 1
fi

echo "*** Start deleting the logs ***"
oci logging log list --log-group-id "${LOG_GROUP_ID}" | jq -r '.data' | jq -c '.[]' | while read -r log; do
    LOG_ID=$(echo "$log"| jq -r '.id')
    echo "$LOG_ID"
    oci logging log delete --force --log-group-id "${LOG_GROUP_ID}" --log-id "${LOG_ID}"
done

echo "*** All Logs Deleted ***"
