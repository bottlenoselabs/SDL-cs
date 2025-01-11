#!/bin/bash

if ! [[ -x "$(command -v jq)" ]]; then
  echo "Error: 'jq' is not installed." >&2
  exit 1
fi

PACKAGE_ID=$1
if [[ -z $PACKAGE_ID ]]; then
    echo "Error: NuGet package ID not provided." >&2
    exit 1
fi

PACKAGE_ID_LOWER=$(echo "$PACKAGE_ID" | tr '[:upper:]' '[:lower:]')
API_URL="https://api.nuget.org/v3/registration5-semver1/$PACKAGE_ID_LOWER/index.json"
RESPONSE=$(curl -s "$API_URL")
LATEST_VERSION=$(echo "$RESPONSE" | jq -r '.items[-1].items[-1].catalogEntry.version')
echo "$LATEST_VERSION"
