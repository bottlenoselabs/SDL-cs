#!/bin/bash

# For Linux, please see https://wiki.libsdl.org/SDL3/README/linux for instructions to pre-install development packages.

# Get the directory where this script exists so that this script can be run from anywhere (e.g. doesn't have to be executed in the same directory)
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

# Set variables unique to this script: SDL3. See https://wiki.libsdl.org/SDL3/Installation for details.
INPUT_DIRECTORY="$DIRECTORY/SDL"
OUTPUT_DIRECTORY="$DIRECTORY/../lib"
BUILD_FLAGS="-DSDL_TEST_LIBRARY=OFF"

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

# Script parameters
BUILD_TYPE=$1
DESIRED_RID=$2

# Check and validate script parameter: 1
if [[ ! -z $BUILD_TYPE ]]; then
    if [[ $BUILD_TYPE == 'debug' ]]; then
        CMAKE_BUILD_TYPE="Debug"
    elif [[ $BUILD_TYPE == 'release' ]]; then
        CMAKE_BUILD_TYPE="Release"
    else
        echo "Build type '$BUILD_TYPE' is not known or can't be used when building native libraries. Please use one of the following: 'debug', 'release'" >&2
        exit 1
    fi
else
    BUILD_TYPE="release"
    CMAKE_BUILD_TYPE="Release"
fi
echo "BUILD_TYPE: $BUILD_TYPE"

# Check and validate script parameter: 2
if [[ ! -z $DESIRED_RID ]]; then
    if [[ $DESIRED_RID == 'win-x64' || $DESIRED_RID == 'win-arm64' ]]; then
        if [[ $RID == 'win-x64' || $RID == 'win-arm64' ]]; then
            RID="$DESIRED_RID"
        else
            echo "RID '$DESIRED_RID' is not known or can not be be used when building native libraries for Windows. Please use one of the following for Windows: 'win-x64', 'win-arm64'" >&2
            exit 1
        fi
    elif [[ $DESIRED_RID == 'osx-x64' || $DESIRED_RID == 'osx-arm64' ]]; then
        if [[ $RID == 'osx-x64' || $RID == 'osx-arm64' ]]; then
            RID="$DESIRED_RID"
        else
            echo "RID '$DESIRED_RID' is not known or can not be be used when building native libraries for macOS. Please use one of the following for macOS: 'osx-x64', 'osx-arm64'" >&2
            exit 1
        fi
    elif [[ $DESIRED_RID == 'linux-x64' || $DESIRED_RID == 'linux-arm64' ]]; then
        if [[ $RID == 'linux-x64' || $RID == 'linux-arm64' ]]; then
            RID="$DESIRED_RID"
        else
            echo "RID '$DESIRED_RID' is not known or can not be be used when building native libraries for Linux. Please use one of the following for Linux: 'linux-x64', 'linux-arm64'" >&2
            exit 1
        fi
    else
        echo "RID '$DESIRED_RID' is not known for building native libraries. Please use one of the following for the appropriate operating system: 'win-x64', 'win-arm64', 'osx-x64', 'osx-arm64', 'linux-x64', 'linux-arm64'" >&2
        exit 1
    fi
fi
echo "RID: $RID"

# Set CMake build files directory
CMAKE_BUILD_DIRECTORY="$DIRECTORY/.cmake-build"

# Delete CMake build files directory if it already exists (this can happen if something went wrong previously leaving the files around)
if [ -d "$CMAKE_BUILD_DIRECTORY" ]; then
    rm -rf $CMAKE_BUILD_DIRECTORY
fi

# Set CMake build flags
CMAKE_BUILD_FLAGS="$BUILD_FLAGS -DCMAKE_BUILD_TYPE=$CMAKE_BUILD_TYPE -DCMAKE_ARCHIVE_OUTPUT_DIRECTORY=$OUTPUT_DIRECTORY -DCMAKE_LIBRARY_OUTPUT_DIRECTORY=$OUTPUT_DIRECTORY -DCMAKE_RUNTIME_OUTPUT_DIRECTORY=$OUTPUT_DIRECTORY"
if [[ $RID == 'win-x64' ]]; then
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -A x64"
elif [[ $RID == 'win-arm64' ]]; then
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -A ARM64"
elif [[ $RID == 'osx-x64' ]]; then
    # https://github.com/libsdl-org/SDL/blob/main/docs/README-macos.md
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -DCMAKE_OSX_ARCHITECTURES=x86_64 -DCMAKE_OSX_DEPLOYMENT_TARGET=10.11"
elif [[ $RID == 'osx-arm64' ]]; then
    # https://en.wikipedia.org/wiki/MacOS_version_history says Apple Silicon is only supported on 11.0 or later
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -DCMAKE_OSX_ARCHITECTURES=arm64 -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0"
elif [[ $RID == 'linux-x64' ]]; then
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -DCMAKE_SYSTEM_NAME=Linux -DCMAKE_SYSTEM_PROCESSOR=x86_64 -DCMAKE_C_COMPILER=gcc -DCMAKE_CXX_COMPILER=g++ -DCMAKE_C_FLAGS=-m64 -DCMAKE_CXX_FLAGS=-m64"
elif [[ $RID == 'linux-arm64' ]]; then
    # Requires packages: gcc-aarch64-linux-gnu g++-aarch64-linux-gnu
    CMAKE_BUILD_FLAGS="$CMAKE_BUILD_FLAGS -DCMAKE_SYSTEM_NAME=Linux -DCMAKE_SYSTEM_PROCESSOR=aarch64 -DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc -DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++"
fi
echo "CMAKE_BUILD_FLAGS: $CMAKE_BUILD_FLAGS"

# Generate CMake build files
cmake -S $INPUT_DIRECTORY -B $CMAKE_BUILD_DIRECTORY $CMAKE_BUILD_FLAGS

# Build library
cmake --build $CMAKE_BUILD_DIRECTORY --config Release

# Cleanup, delete CMake build files directory if it already exists
if [ -d "$CMAKE_BUILD_DIRECTORY" ]; then
    rm -rf $CMAKE_BUILD_DIRECTORY
fi
