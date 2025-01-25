#!/bin/bash
# https://google.github.io/styleguide/shellguide.html

if ! [[ -x "$(command -v jq)" ]]; then
    echo "Error: 'jq' is not installed" >&2
    exit 1
fi

if ! [[ -x "$(command -v gh)" ]]; then
    echo "Error: 'gh' (GitHub CLI) is not installed" >&2
    exit 1
fi

gh auth status &>/dev/null
if [[ $? -ne 0 ]]; then
    echo "Error: Not signed into GitHub" >&2
    exit 1
fi

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

function validate_rid {
    if [[ ! -z $DESIRED_RID ]]; then
        if [[ $DESIRED_RID == 'win-x64' || $DESIRED_RID == 'win-arm64' ]]; then
            if [[ $RID == 'win-x64' || $RID == 'win-arm64' ]]; then
                RID="$DESIRED_RID"
            else
                echo "RID '$DESIRED_RID' is not known or can not be be used for Windows. Please use one of the following for Windows: 'win-x64', 'win-arm64'" >&2
                exit 1
            fi
        elif [[ $DESIRED_RID == 'osx-x64' || $DESIRED_RID == 'osx-arm64' ]]; then
            if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
                RID="$DESIRED_RID"
            else
                echo "RID '$DESIRED_RID' is not known or can not be be used for macOS. Please use one of the following for macOS: 'osx-x64', 'osx-arm64'" >&2
                exit 1
            fi
        elif [[ $DESIRED_RID == 'linux-x64' || $DESIRED_RID == 'linux-arm64' ]]; then
            if [[ $RID == 'linux-x64' || $RID == 'linux-arm64' ]]; then
                RID="$DESIRED_RID"
            else
                echo "RID '$DESIRED_RID' is not known or can not be be used for Linux. Please use one of the following for Linux: 'linux-x64', 'linux-arm64'" >&2
                exit 1
            fi
        else
            echo "RID '$DESIRED_RID' is not known. Please use one of the following for the appropriate operating system: 'win-x64', 'win-arm64', 'osx-x64', 'osx-arm64', 'linux-x64', 'linux-arm64'" >&2
            exit 1
        fi
    fi
    echo "RID: $RID"
}

function initialize() {
    DIRECTORY_CURRENT=`pwd`
    # Get the directory where this script exists so that this script can be run from anywhere (e.g. doesn't have to be executed in the same directory)
    DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

    # Get the name of this script file without extension
    FILE_NAME_WITHOUT_EXTENSION=`basename ${BASH_SOURCE[0]} ".sh"`

    # Directory used to create this script's files/folders
    DIRECTORY_CONTAINER="$DIRECTORY/.$FILE_NAME_WITHOUT_EXTENSION"
    mkdir -p "$DIRECTORY_CONTAINER"

    # Directory to place CMake build files per project
    DIRECTORY_CONTAINER_ARCHIVES="$DIRECTORY_CONTAINER/archives"
    mkdir -p "$DIRECTORY_CONTAINER_ARCHIVES"
}

gh api \
  -H "Accept: application/vnd.github+json" \
  -H "X-GitHub-Api-Version: 2022-11-28" \
  /repos/libsdl-org/SDL_shadercross/actions/artifacts/2483730884

# TEMP_FILE_PATH_JSON_ARTIFACTS=$(mktemp)
# curl --output "$TEMP_FILE_PATH_JSON_ARTIFACTS" -L -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2022-11-28" https://api.github.com/repos/libsdl-org/SDL_shadercross/actions/artifacts
# ARCHIVE_URL_MACOS=`jq -r '.artifacts
# | map(select(.name == "SDL3_shadercross-macos-arm64" and .expired == false))
# | sort_by(.created_at)
# | reverse
# | .[0]
# | "https://github.com/libsdl-org/SDL_shadercross/actions/runs/\(.workflow_run.id)/artifacts/\(.id)"' "$TEMP_FILE_PATH_JSON_ARTIFACTS"`

# echo $ARCHIVE_URL_MACOS

# TEMP_FILE_PATH_ARCHIVE_MACOS=$(mktemp)
# curl -L "$TEMP_FILE_PATH_ARCHIVE_MACOS"

# Clean up
# rm "$TEMP_FILE_PATH_JSON_ARTIFACTS"
# rm "$TEMP_FILE_PATH_ARCHIVE_MACOS"

function download {
    echo "Downloading shadercross archive ($RID)..."

    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        FILE_PATH_ARCHIVE="$DIRECTORY_CONTAINER_ARCHIVES/SDL3_shadercross-3.0.0-darwin-arm64-x64.tar.gz"

        if [[ ! -f "$FILE_PATH_ARCHIVE" ]]; then
            cd "$DIRECTORY_CONTAINER_ARCHIVES"
            gh run download 12958770530 -n "SDL3_shadercross-macos-arm64" --repo "libsdl-org/SDL_shadercross"
            cd "$DIRECTORY_CURRENT"

            if [[ ! -f "$FILE_PATH_ARCHIVE" ]]; then
                echo "Downloading shadercross archive ($RID)... failed" >&2
            exit 1
            else
                echo "Downloading shadercross archive ($RID)... done"
            fi
        else
            echo "Downloading shadercross archive ($RID)... skipped"
        fi
    fi
}

function unzip {
    echo "Unzipping archive..."

    if [[ ! -f "$FILE_PATH_ARCHIVE" ]]; then
        echo "Unzipping archive... failed" >&2
        exit 1
    fi

    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        DIRECTORY_PATH_ARCHIVE_UNZIPPED="$DIRECTORY_CONTAINER/osx"
    else
        DIRECTORY_PATH_ARCHIVE_UNZIPPED="$DIRECTORY_CONTAINER/$RID"
    fi

    mkdir -p "$DIRECTORY_PATH_ARCHIVE_UNZIPPED"
    tar -xzf "$FILE_PATH_ARCHIVE" -C "$DIRECTORY_PATH_ARCHIVE_UNZIPPED" --strip-components=1

    echo "Unzipping archive... done"
}

function copy_files() {
    echo "Copying files..."

    local DIRECTORY_COPY_SOURCE_BIN="$DIRECTORY_PATH_ARCHIVE_UNZIPPED/bin"
    local DIRECTORY_COPY_SOURCE_LIB="$DIRECTORY_PATH_ARCHIVE_UNZIPPED/lib"

    local DIRECTORY_COPY_DESTINATION="$DIRECTORY/../lib/$RID"
    mkdir -p "$DIRECTORY_COPY_DESTINATION"

    cd "$DIRECTORY_COPY_SOURCE_LIB"
    if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
        cp "$DIRECTORY_COPY_SOURCE_BIN/shadercross" "$DIRECTORY_COPY_DESTINATION/shadercross"
        install_name_tool -rpath @executable_path/../lib @executable_path "$DIRECTORY_COPY_DESTINATION/shadercross"

        install_name_tool -change @rpath/libSDL3_shadercross.0.dylib @rpath/libSDL3_shadercross.dylib $DIRECTORY_COPY_DESTINATION/shadercross
        install_name_tool -change @rpath/libSDL3.0.dylib @rpath/libSDL3.dylib $DIRECTORY_COPY_DESTINATION/shadercross

        cp libdxcompiler.dylib "$DIRECTORY_COPY_DESTINATION/libdxcompiler.dylib"

        cp libspirv-cross-c-shared.*.*.*.dylib "$DIRECTORY_COPY_DESTINATION/libspirv-cross-c-shared.dylib"
        install_name_tool -id @rpath/libspirv-cross-c-shared.dylib $DIRECTORY_COPY_DESTINATION/libspirv-cross-c-shared.dylib

        cp libSDL3_shadercross.*.*.*.dylib "$DIRECTORY_COPY_DESTINATION/libSDL3_shadercross.dylib"
        install_name_tool -id @rpath/libSDL3_shadercross.dylib $DIRECTORY_COPY_DESTINATION/libSDL3_shadercross.dylib
        install_name_tool -change @rpath/libSDL3.0.dylib @rpath/libSDL3.dylib $DIRECTORY_COPY_DESTINATION/libSDL3_shadercross.dylib
        install_name_tool -change @rpath/libspirv-cross-c-shared.0.dylib @rpath/libspirv-cross-c-shared.dylib $DIRECTORY_COPY_DESTINATION/libSDL3_shadercross.dylib
    fi
    cd "$DIRECTORY_CURRENT"

    echo "Copying files... done"
}

DESIRED_RID=$1
RID=`get_dotnet_rid`
validate_rid
initialize
download
unzip
copy_files


