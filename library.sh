#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
LIB_DIR="$DIR/lib"
mkdir -p $LIB_DIR
echo "Started '$0' $1 $2 $3 $4"

if [[ ! -z "$1" ]]; then
    TARGET_BUILD_OS="$1"
fi

if [[ ! -z "$2" ]]; then
    TARGET_BUILD_ARCH="$2"
fi

function set_target_build_os {
    if [[ -z "$TARGET_BUILD_OS" || $TARGET_BUILD_OS == "host" ]]; then
        uname_str="$(uname -a)"
        case "${uname_str}" in
            *Microsoft*)    TARGET_BUILD_OS="windows";;
            *microsoft*)    TARGET_BUILD_OS="windows";;
            Linux*)         TARGET_BUILD_OS="linux";;
            Darwin*)        TARGET_BUILD_OS="macos";;
            CYGWIN*)        TARGET_BUILD_OS="linux";;
            MINGW*)         TARGET_BUILD_OS="windows";;
            *Msys)          TARGET_BUILD_OS="windows";;
            *)              TARGET_BUILD_OS="UNKNOWN:${uname_str}"
        esac
        echo "Target build operating system: '$TARGET_BUILD_OS' (host)"
    else
        if [[
            "$TARGET_BUILD_OS" == "windows" ||
            "$TARGET_BUILD_OS" == "macos" ||
            "$TARGET_BUILD_OS" == "linux"
            ]]; then
            echo "Target build operating system: '$TARGET_BUILD_OS' (override)"
        else
            echo "Unknown '$TARGET_BUILD_OS' passed as first argument. Use 'host' to use the host build platform or use either: 'windows', 'macos', 'linux'."
            exit 1
        fi
    fi
}

function set_target_build_arch {
    if [[ -z "$TARGET_BUILD_ARCH" || $TARGET_BUILD_ARCH == "default" ]]; then
        if [[ "$TARGET_BUILD_OS" == "macos" ]]; then
            TARGET_BUILD_ARCH="x86_64;arm64"
        else
            TARGET_BUILD_ARCH="$(uname -m)"
        fi
        echo "Target build CPU architecture: '$TARGET_BUILD_ARCH' (default)"
    else
        if [[ "$TARGET_BUILD_ARCH" == "x86_64" || "$TARGET_BUILD_ARCH" == "arm64" ]]; then
            echo "Target build CPU architecture: '$TARGET_BUILD_ARCH' (override)"
        else
            echo "Unknown '$TARGET_BUILD_ARCH' passed as second argument. Use 'default' to use the host CPU architecture or use either: 'x86_64', 'arm64'."
            exit 1
        fi
    fi
}

set_target_build_os
set_target_build_arch

if [[
    "$TARGET_BUILD_OS" != "windows" &&
    "$TARGET_BUILD_OS" != "macos" &&
    "$TARGET_BUILD_OS" != "linux"
    ]]; then
    echo "Unknown target build operating system: $TARGET_BUILD_OS"
    exit 1
fi

if [[ "$TARGET_BUILD_OS" == "macos" ]]; then
    CMAKE_ARCH_ARGS="-DCMAKE_OSX_ARCHITECTURES=$TARGET_BUILD_ARCH"
elif [[ "$TARGET_BUILD_OS" == "windows" ]]; then
    if [[ "$TARGET_BUILD_ARCH" != "x86_64" ]]; then
        echo "Unknown target build CPU architecture for Windows: $TARGET_BUILD_ARCH"
        exit 1
    fi
fi

function exit_if_last_command_failed() {
    error=$?
    if [ $error -ne 0 ]; then
        echo "Last command failed: $error"
        exit $error
    fi
}

function build_sdl() {
    echo "Building SDL..."
    SDL_BUILD_DIR="$DIR/cmake-build-release"
    cmake -S $DIR/ext/SDL -B $SDL_BUILD_DIR $CMAKE_ARCH_ARGS -G Ninja  \
        `#Change output directories` \
        -DCMAKE_ARCHIVE_OUTPUT_DIRECTORY=$SDL_BUILD_DIR -DCMAKE_LIBRARY_OUTPUT_DIRECTORY=$SDL_BUILD_DIR -DCMAKE_RUNTIME_OUTPUT_DIRECTORY=$SDL_BUILD_DIR -DCMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE=$SDL_BUILD_DIR \
        `# project specific` \
        -DSDL_STATIC=OFF -DSDL_TEST=OFF -DSDL_LEAN_AND_MEAN=1

    cmake --build $SDL_BUILD_DIR --config Release

    if [[ "$TARGET_BUILD_OS" == "linux" ]]; then
        SDL_LIBRARY_FILENAME="libSDL2.so"
        SDL_LIBRARY_FILE_PATH_BUILD="$(readlink -f $SDL_BUILD_DIR/libSDL2-2.0.so)"
    elif [[ "$TARGET_BUILD_OS" == "macos" ]]; then
        SDL_LIBRARY_FILENAME="libSDL2.dylib"
        SDL_LIBRARY_FILE_PATH_BUILD="$SDL_BUILD_DIR/libSDL2-2.0.dylib"
    elif [[ "$TARGET_BUILD_OS" == "windows" ]]; then
        SDL_LIBRARY_FILENAME="SDL2.dll"
        SDL_LIBRARY_FILE_PATH_BUILD="$SDL_BUILD_DIR/$SDL_LIBRARY_FILENAME"
    fi
    SDL_LIBRARY_FILE_PATH="$LIB_DIR/$SDL_LIBRARY_FILENAME"

    if [[ ! -f "$SDL_LIBRARY_FILE_PATH_BUILD" ]]; then
        echo "The file '$SDL_LIBRARY_FILE_PATH_BUILD' does not exist!"
        exit 1
    fi

    mv "$SDL_LIBRARY_FILE_PATH_BUILD" "$SDL_LIBRARY_FILE_PATH"
    exit_if_last_command_failed
    echo "Copied '$SDL_LIBRARY_FILE_PATH_BUILD' to '$SDL_LIBRARY_FILE_PATH'"

    rm -r $SDL_BUILD_DIR
    exit_if_last_command_failed
    echo "Building SDL2 finished!"
}

build_sdl
ls -d "$LIB_DIR"/*

echo "Finished '$0'!"