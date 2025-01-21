#!/bin/bash
# https://google.github.io/styleguide/shellguide.html

if ! [[ -x "$(command -v cmake)" ]]; then
  echo "Error: 'cmake' is not installed" >&2
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

function validate_build_type() {
    if [[ ! -z $BUILD_TYPE ]]; then
        if [[ $BUILD_TYPE != 'debug' && $BUILD_TYPE != 'release' ]]; then
            echo "Build type '$BUILD_TYPE' is not known or can't be used when building native libraries. Please use one of the following: 'debug', 'release'" >&2
            exit 1
        fi
    else
        BUILD_TYPE="release"
    fi
    echo "BUILD_TYPE: $BUILD_TYPE"
}

function validate_rid {
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
}

function initialize() {
    # Get the directory where this script exists so that this script can be run from anywhere (e.g. doesn't have to be executed in the same directory)
    DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

    # Get the name of this script file without extension
    FILE_NAME_WITHOUT_EXTENSION=`basename ${BASH_SOURCE[0]} ".sh"`

    # Directory used to create this script's files/folders
    DIRECTORY_CONTAINER="$DIRECTORY/.$FILE_NAME_WITHOUT_EXTENSION"
    mkdir -p "$DIRECTORY_CONTAINER"

    # Directory to place CMake build files per project
    DIRECTORY_CONTAINER_BUILDS="$DIRECTORY_CONTAINER/builds"
    mkdir -p "$DIRECTORY_CONTAINER_BUILDS"

    # Directories to place built artifacts
    DIRECTORY_OUTPUT="$DIRECTORY_CONTAINER/out/$RID-$BUILD_TYPE"
    if [[ -d "$DIRECTORY_OUTPUT" ]]; then
        rm -r "$DIRECTORY_OUTPUT"
        mkdir "$DIRECTORY_OUTPUT"
    fi
}

function build_library_cmake() {
    local LIB_NAME=$1
    local LIB_VERSION=$2
    local LIB_DIRECTORY_SOURCE=$3
    local LIB_CMAKE_FLAGS=`echo $4 | tr '\n' ' '`
    local LIB_CMAKE_OVERRIDE_FILE_PATH=$5

    local LIB_NAME_UPPERCASE="$(echo "$LIB_NAME" | tr 'a-z' 'A-Z')"

    if [[ -z $LIB_VERSION ]]; then
        local LIB_DIRECTORY_BUILD="$DIRECTORY_CONTAINER_BUILDS/$LIB_NAME-$RID-$BUILD_TYPE"
    else
        local LIB_DIRECTORY_BUILD="$DIRECTORY_CONTAINER_BUILDS/$LIB_NAME-$LIB_VERSION-$RID-$BUILD_TYPE"
    fi

    # Make the variables usable outside this function with unique names
    eval "LIB_${LIB_NAME_UPPERCASE}_DIRECTORY_BUILD=\"$LIB_DIRECTORY_BUILD\""

    echo "Building library '$LIB_NAME' ($BUILD_TYPE) using CMake..."

    if [[ -z $LIB_DIRECTORY_SOURCE ]]; then
        eval "LIB_DIRECTORY_SOURCE=\"\${LIB_${LIB_NAME_UPPERCASE}_DIRECTORY_SOURCE}\""

        if [[ -z $LIB_DIRECTORY_SOURCE ]]; then
            echo "Building library '$LIB_NAME' failed: the source directory is not provided" >&2
            exit 1
        fi
    fi

    if [ ! -d "$LIB_DIRECTORY_SOURCE" ]; then
        echo "Building library '$LIB_NAME' ($BUILD_TYPE) failed: the source directory '$LIB_DIRECTORY_SOURCE' does not exist" >&2
        exit 1
    fi

    if [ ! -f "$LIB_DIRECTORY_SOURCE/CMakeLists.txt" ]; then
        echo "Building library '$LIB_NAME' ($BUILD_TYPE) failed: the source directory '$LIB_DIRECTORY_SOURCE' does not have a 'CMakeLists.txt' file" >&2
        exit 1
    fi

    if [ -d "$LIB_DIRECTORY_BUILD" ]; then
        rm -r $LIB_DIRECTORY_BUILD
    fi

    if [[ $BUILD_TYPE == 'debug' ]]; then
        local CMAKE_BUILD_TYPE="Debug"
    elif [[ $BUILD_TYPE == 'release' ]]; then
        local CMAKE_BUILD_TYPE="Release"
    else
        echo "Unknown build type '$BUILD_TYPE' for CMake" >&2
        exit 1
    fi

    if [[ $RID == 'win-x64' ]]; then
        local CMAKE_FLAGS_ARCH="-A x64"
    elif [[ $RID == 'win-arm64' ]]; then
        local CMAKE_FLAGS_ARCH="-A ARM64"
    elif [[ $RID == 'osx-x64' ]]; then
        local CMAKE_FLAGS_ARCH="-D CMAKE_SYSTEM_NAME=Darwin -D CMAKE_OSX_ARCHITECTURES=x86_64 -D CMAKE_OSX_DEPLOYMENT_TARGET=10.11"
    elif [[ $RID == 'osx-arm64' ]]; then
        # https://en.wikipedia.org/wiki/MacOS_version_history says Apple Silicon is only supported on 11.0 or later
        local CMAKE_FLAGS_ARCH="-D CMAKE_SYSTEM_NAME=Darwin -D CMAKE_OSX_ARCHITECTURES=arm64 -D CMAKE_OSX_DEPLOYMENT_TARGET=11.0"
    elif [[ $RID == 'linux-x64' ]]; then
        local CMAKE_FLAGS_ARCH="-D CMAKE_SYSTEM_NAME=Linux -D CMAKE_SYSTEM_PROCESSOR=x86_64 -D CMAKE_C_COMPILER=gcc -D CMAKE_CXX_COMPILER=g++ -D CMAKE_C_FLAGS=-m64 -D CMAKE_CXX_FLAGS=-m64"
    elif [[ $RID == 'linux-arm64' ]]; then
        # Requires packages: gcc-aarch64-linux-gnu g++-aarch64-linux-gnu
        local CMAKE_FLAGS_ARCH="$CMAKE_FLAGS -D CMAKE_SYSTEM_NAME=Linux -D CMAKE_SYSTEM_PROCESSOR=aarch64 -D CMAKE_C_COMPILER=aarch64-linux-gnu-gcc -D CMAKE_CXX_COMPILER=aarch64-linux-gnu-g++"
    else
        echo "Unknown RID '$RID' for CMake" >&2
        exit 1
    fi

    local CMAKE_FLAGS_BUILD_TYPE="-D CMAKE_BUILD_TYPE=$CMAKE_BUILD_TYPE"
    local CMAKE_FLAGS_OUTPUT="-D CMAKE_INSTALL_PREFIX=$DIRECTORY_OUTPUT"
    cmake -S "$LIB_DIRECTORY_SOURCE" -B "$LIB_DIRECTORY_BUILD" $LIB_CMAKE_FLAGS $CMAKE_FLAGS_OUTPUT $CMAKE_FLAGS_ARCH $CMAKE_FLAGS_BUILD_TYPE -DCMAKE_SHARED_LIBRARY_SOVERSION="" -DCMAKE_SHARED_LIBRARY_VERSION="" -DSO_VERSION_MAJOR="" -DSO_VERSION="" -DDYLIB_COMPAT_VERSION="" -DDYLIB_CURRENT_VERSION=""
    if [[ $? -ne 0 ]]; then
        echo "Building library '$LIB_NAME' ($BUILD_TYPE) failed: CMake was not able to generate build files" >&2
        exit 1
    fi

    cmake --build "$LIB_DIRECTORY_BUILD" --config $CMAKE_BUILD_TYPE --parallel
    if [[ $? -ne 0 ]]; then
        echo "Building library '$LIB_NAME' ($BUILD_TYPE) failed: CMake build was not successful" >&2
        exit 1
    fi

    cmake --install "$LIB_DIRECTORY_BUILD"
    if [[ $? -ne 0 ]]; then
        echo "Building library '$LIB_NAME' ($BUILD_TYPE) failed: CMake install was not successful" >&2
        exit 1
    fi

    echo "Building library '$LIB_NAME' ($BUILD_TYPE) using CMake... done"
}

function build_SDL3() {
    local DIRECTORY_SOURCE="$DIRECTORY/SDL"
    local CMAKE_BUILD_FLAGS="
        -D SDL_SHARED=ON
        -D SDL_STATIC=OFF
        -D SDL_TEST_LIBRARY=OFF
        -D SDL_TEST_LIBRARY=OFF
        -D SDL_TESTS=OFF
        -D SDL_DISABLE_INSTALL_DOCS=OFF"

    build_library_cmake "SDL3" "" "$DIRECTORY_SOURCE" "$CMAKE_BUILD_FLAGS"
}

function build_SDL3_image() {
    local DIRECTORY_SOURCE="$DIRECTORY/SDL_image"
    local CMAKE_BUILD_FLAGS="
        -D SDL3_DIR=$LIB_SDL3_DIRECTORY_BUILD
        -D BUILD_SHARED_LIBS=ON
        -D SDLIMAGE_DEPS_SHARED=OFF
        -D SDLIMAGE_VENDORED=ON
        -D SDLIMAGE_STRICT=ON
        -D SDLIMAGE_SAMPLES=OFF
        -D SDLIMAGE_TESTS=OFF
        "

    build_library_cmake "SDL3_image" "" "$DIRECTORY_SOURCE" "$CMAKE_BUILD_FLAGS"
}

function copy_files() {
    echo "Copying files..."

    DIRECTORY_LIB_BUILD="$DIRECTORY_OUTPUT/lib"
    DIRECTORY_LIB="$DIRECTORY/../lib/$RID"
    if [[ ! -d "$DIRECTORY_LIB" ]]; then
        rm -r $DIRECTORY_LIB
    fi
    mkdir -p $DIRECTORY_LIB
    rsync -av --include="*.dll" --include="*.dylib" --include="*.so" --include=".a" --include=".lib" --exclude="*" "$DIRECTORY_LIB_BUILD/" $DIRECTORY_LIB

    echo "Copying files... done"
}

BUILD_TYPE=$1
validate_build_type

DESIRED_RID=$2
RID=`get_dotnet_rid`
validate_rid

initialize
build_SDL3
build_SDL3_image
copy_files
