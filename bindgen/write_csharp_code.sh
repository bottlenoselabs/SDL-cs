#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

dotnet build "$DIRECTORY/../src/cs/production/SDL.Bindgen/SDL.Bindgen.csproj" -p:OutputPath="$DIRECTORY/plugins/SDL.Bindgen"
c2cs cs