#!/usr/bin/env bash
# Validate Bicep templates use free-tier SKUs
set -e
errors=0

grep -q "name: 'Free'" infra/modules/swa.bicep || { echo 'Static Web Apps not Free plan'; errors=1; }
grep -q "enableFreeTier: true" infra/modules/cosmos.bicep || { echo 'Cosmos DB free tier not enabled'; errors=1; }
grep -q "name: 'Y1'" infra/modules/function.bicep || { echo 'Function plan not Y1'; errors=1; }
grep -q "Standard_LRS" infra/modules/blob.bicep || { echo 'Storage account not Standard_LRS'; errors=1; }

if [ "$errors" -eq 0 ]; then
  echo "Free tier configuration check passed."
else
  echo "Free tier configuration issues detected." >&2
  exit 1
fi
