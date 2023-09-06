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
#  date: Tue Aug 22 11:57:50 IST 2023
 
#  revision: RNW-23C
#  SHA1: $Id: 9158b8f36e968cb90e4f2d0fc6950989e1905d29 $
################################################################################################
#  File: get_region_code.sh
################################################################################################

# *** messages for status
# !!! messages for success
# ### messages for failure'

rm "$2" 2> /dev/null
oci iam region list | jq -r '.data' | jq -c '.[]' | while read -r region; do
    REGION_NAME=$(echo "$region"| jq -r '.name')
    if [[ "$REGION_NAME" == "$1" ]]; then
          echo "$region" | jq -r '.key' | awk '{print tolower($0)}' | tr -d '"' | tr -d '\n'   >> "$2"
    fi
done