#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
if [[ -z $SCRIPTS_DIRECTORY ]]; then
    SCRIPTS_DIRECTORY="$DIRECTORY/ext/scripts"
    git clone "https://github.com/bottlenoselabs/scripts" "$SCRIPTS_DIRECTORY" 2> /dev/null 1> /dev/null || git -C "$SCRIPTS_DIRECTORY" pull 1> /dev/null
fi

. $SCRIPTS_DIRECTORY/utility.sh

if [[ ! -z "$1" ]]; then
    TARGET_BUILD_OS="$1"
    OS="$TARGET_BUILD_OS"
else
    TARGET_BUILD_OS="host"
fi

if [[ "$TARGET_BUILD_OS" == "host" ]]; then
    OS="$(get_operating_system)"
fi

if [[ "$OS" == "windows" ]]; then
    LIBRARY_NAME="SDL3"
else
    LIBRARY_NAME="SDL3"
fi

if [[ ! -z "$2" ]]; then
    TARGET_BUILD_ARCH="$2"
else
    TARGET_BUILD_ARCH="default"
fi
LIBRARY_NAME_PINVOKE="SDL3"

$SCRIPTS_DIRECTORY/c/library/main.sh \
    $DIRECTORY/ext/SDL \
    $DIRECTORY/build \
    $DIRECTORY/lib \
    $LIBRARY_NAME \
    $LIBRARY_NAME_PINVOKE \
    $TARGET_BUILD_OS \
    $TARGET_BUILD_ARCH \
    "-DSDL_STATIC=OFF" "-DSDL_TEST=OFF" "-DSDL_LEAN_AND_MEAN=1" "-DCMAKE_OSX_DEPLOYMENT_TARGET=10.9"