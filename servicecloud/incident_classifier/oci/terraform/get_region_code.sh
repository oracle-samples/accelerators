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
#  SHA1: $Id: 3fe7237c0be31c5e66b7c7cf5dcd8840adadfa5c $
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