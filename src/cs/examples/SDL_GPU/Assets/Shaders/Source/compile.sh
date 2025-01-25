#!/bin/bash
# https://google.github.io/styleguide/shellguide.html
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

# Get the .NET runtime identifier (RID) for the current operating system
# See: https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
function get_dotnet_rid() {
    local UNAME_OS_STRING="$(uname -a)"
    case "${UNAME_OS_STRING}" in
        *Microsoft*)    local RID_OS="win";;
        *microsoft*)    local RID_OS="win";;
        Linux*)         local RID_OS="linux";;
        Darwin*)        local RID_OS="osx";;
        CYGWIN*)        local RID_OS="linux";;
        MINGW*)         local RID_OS="win";;
        *Msys)          local RID_OS="win";;
        *)              local RID_OS="UNKNOWN:${UNAME_OS_STRING}"
    esac
    local UNAME_ARCH_STRING="$(uname -m)"
    case "${UNAME_ARCH_STRING}" in
        x86_64)         local RID_ARCH="x64";;
        arm64)          local RID_ARCH="arm64";;
        *)              local RID_ARCH="UNKNOWN:${UNAME_ARCH_STRING}"
    esac
    echo "$RID_OS-$RID_ARCH"
    return 0
}

RID=`get_dotnet_rid`

if ! [[ -x "$(command -v shadercross)" ]]; then
    FILE_PATH_SHADERCROSS="$DIRECTORY/../../../../../../../lib/$RID/shadercross"
    if ! [[ -x "$FILE_PATH_SHADERCROSS" ]]; then
        echo "Error: 'shadercross' is not installed" >&2
        exit 1
    else
        export PATH="$PATH:$(dirname "$FILE_PATH_SHADERCROSS")"
    fi
fi

DIRECTORY_OUTPUT="$DIRECTORY/../Compiled"
DIRECTORY_OUTPUT_SPIRV="$DIRECTORY_OUTPUT/SPIRV"
DIRECTORY_OUTPUT_MSL="$DIRECTORY_OUTPUT/MSL"
DIRECTORY_OUTPUT_DXIL="$DIRECTORY_OUTPUT/DXIL"
mkdir -p "$DIRECTORY_OUTPUT_SPIRV"
mkdir -p "$DIRECTORY_OUTPUT_MSL"
mkdir -p "$DIRECTORY_OUTPUT_DXIL"

find "$DIRECTORY" -type f -name "*.vert.hlsl" | while read -r FILE_PATH; do
    FILE_NAME=$(basename $"$FILE_PATH" .hlsl)
    shadercross $FILE_PATH --stage vertex --source HLSL --dest SPIRV --output "$DIRECTORY_OUTPUT_SPIRV/$FILE_NAME.spv"
    shadercross $FILE_PATH --stage vertex --source HLSL --dest MSL --output "$DIRECTORY_OUTPUT_MSL/$FILE_NAME.msl"
    shadercross $FILE_PATH --stage vertex --source HLSL --dest DXIL --output "$DIRECTORY_OUTPUT_DXIL/$FILE_NAME.dxil"
done

find "$DIRECTORY" -type f -name "*.frag.hlsl" | while read -r FILE_PATH; do
    FILE_NAME=$(basename "$FILE_PATH" .hlsl)
    shadercross $FILE_PATH --stage fragment --source HLSL --dest SPIRV --output "$DIRECTORY_OUTPUT_SPIRV/$FILE_NAME.spv"
    shadercross $FILE_PATH --stage fragment --source HLSL --dest MSL --output "$DIRECTORY_OUTPUT_MSL/$FILE_NAME.msl"
    shadercross $FILE_PATH --stage fragment --source HLSL --dest DXIL --output "$DIRECTORY_OUTPUT_DXIL/$FILE_NAME.dxil"
done

find "$DIRECTORY" -type f -name "*.comp.hlsl" | while read -r FILE_PATH; do
    FILE_NAME=$(basename "$FILE_PATH" .hlsl)
    shadercross $FILE_PATH --stage compute --source HLSL --dest SPIRV --output "$DIRECTORY_OUTPUT_SPIRV/$FILE_NAME.spv"
    shadercross $FILE_PATH --stage compute --source HLSL --dest MSL --output "$DIRECTORY_OUTPUT_MSL/$FILE_NAME.msl"
    shadercross $FILE_PATH --stage compute --source HLSL --dest DXIL --output "$DIRECTORY_OUTPUT_DXIL/$FILE_NAME.dxil"
done
