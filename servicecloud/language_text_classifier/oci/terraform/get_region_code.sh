#!/usr/bin/env bash
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