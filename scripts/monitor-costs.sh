#!/usr/bin/env bash
# Query Azure Cost Management for current month's cost
set -e
subscription="$1"
if [ -z "$subscription" ]; then
  subscription=$(az account show --query id -o tsv 2>/dev/null || echo "")
fi
if [ -z "$subscription" ]; then
  echo "Azure subscription not specified and could not be detected" >&2
  exit 1
fi
start=$(date -I -d "$(date +%Y-%m-01)")
end=$(date -I)
amount=$(az costmanagement query -t ActualCost --timeframe Custom \
  --timeframe-start "$start" --timeframe-end "$end" \
  --subscription "$subscription" \
  --query "properties.rows[0][0]" -o tsv 2>/dev/null || echo "N/A")
echo "Current month cost for $subscription: $amount"

