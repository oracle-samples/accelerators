#!/usr/bin/env bash
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
