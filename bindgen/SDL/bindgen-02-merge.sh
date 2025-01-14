#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

if ! [[ -x "$(command -v c2ffi)" ]]; then
  echo "Error: 'c2ffi' is not installed. Please visit https://github.com/bottlenoselabs/c2ffi for instructions to install the tool." >&2
  exit 1
fi

c2ffi merge --inputDirectoryPath "$DIRECTORY/ffi" --outputFilePath "$DIRECTORY/ffi-x/cross-platform.json"
