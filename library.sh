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
    if [[ -z "$TARGET_BUILD_OS" || $TARGET_BUILD_OS == "default" ]]; then
        uname_str="$(uname -a)"
        case "${uname_str}" in
            *Microsoft*)    TARGET_BUILD_OS="microsoft";;
            *microsoft*)    TARGET_BUILD_OS="microsoft";;
            Linux*)         TARGET_BUILD_OS="linux";;
            Darwin*)        TARGET_BUILD_OS="apple";;
            CYGWIN*)        TARGET_BUILD_OS="linux";;
            MINGW*)         TARGET_BUILD_OS="microsoft";;
            *Msys)          TARGET_BUILD_OS="microsoft";;
            *)              TARGET_BUILD_OS="UNKNOWN:${uname_str}"
        esac
        echo "Target build operating system: '$TARGET_BUILD_OS' (default)"
    else
        if [[ "$TARGET_BUILD_OS" == "microsoft" || "$TARGET_BUILD_OS" == "linux" || "$TARGET_BUILD_OS" == "apple" ]]; then
            echo "Target build operating system: '$TARGET_BUILD_OS' (override)"
        else
            echo "Unknown '$TARGET_BUILD_OS' passed as first argument. Use 'default' to use the host build platform or use either: 'microsoft', 'linux', 'apple'."
            exit 1
        fi
    fi
}

function set_target_build_arch {
    if [[ -z "$TARGET_BUILD_ARCH" || $TARGET_BUILD_ARCH == "default" ]]; then
        TARGET_BUILD_ARCH="$(uname -m)"
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

if [[ "$TARGET_BUILD_OS" == "microsoft" ]]; then
    CMAKE_TOOLCHAIN_ARGS="-DCMAKE_TOOLCHAIN_FILE=$DIR/mingw-w64-x86_64.cmake"
elif [[ "$TARGET_BUILD_OS" == "linux" ]]; then
    CMAKE_TOOLCHAIN_ARGS=""
elif [[ "$TARGET_BUILD_OS" == "apple" ]]; then
    CMAKE_TOOLCHAIN_ARGS=""
else
    echo "Unknown target build operating system: $TARGET_BUILD_OS"
    exit 1
fi

if [[ "$TARGET_BUILD_ARCH" == "x86_64" ]]; then
    CMAKE_ARCH_ARGS="-A x64"
elif [[ "$TARGET_BUILD_ARCH" == "arm64" ]]; then
    CMAKE_ARCH_ARGS="-A arm64"
else
    echo "Unknown target build CPU architecture: $TARGET_BUILD_ARCH"
    exit 1
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
    cmake $CMAKE_TOOLCHAIN_ARGS -S $DIR/ext/SDL -B $SDL_BUILD_DIR -DSDL_STATIC=OFF -DSDL_TEST=OFF -DSDL_LEAN_AND_MEAN=1
    cmake --build $SDL_BUILD_DIR --config Release $CMAKE_ARCH_ARGS

    if [[ "$TARGET_BUILD_OS" == "linux" ]]; then
        SDL_LIBRARY_FILENAME="libSDL2.so"
        SDL_LIBRARY_FILE_PATH_BUILD="$(readlink -f $SDL_BUILD_DIR/libSDL2-2.0.so)"
    elif [[ "$TARGET_BUILD_OS" == "apple" ]]; then
        SDL_LIBRARY_FILENAME="libSDL2.dylib"
        SDL_LIBRARY_FILE_PATH_BUILD="$SDL_BUILD_DIR/libSDL2-2.0.dylib"
    elif [[ "$TARGET_BUILD_OS" == "microsoft" ]]; then
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
    echo "Building sokol finished!"
}

build_sdl
ls -d "$LIB_DIR"/*

echo "Finished '$0'!"