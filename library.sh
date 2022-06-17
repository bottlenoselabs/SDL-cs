#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
. $DIR/ext/scripts/utility.sh

if [[ ! -z "$1" ]]; then
    TARGET_BUILD_OS="$1"
    OS="$_TARGET_BUILD_OS"
else
    TARGET_BUILD_OS="host"
fi
if [[ "$TARGET_BUILD_OS" == "host" ]]; then
    OS="$(get_operating_system)"
fi
if [[ "$OS" == "windows" ]]; then
    LIBRARY_NAME="SDL2"
else
    LIBRARY_NAME="SDL2-2.0"
fi

if [[ ! -z "$2" ]]; then
    TARGET_BUILD_ARCH="$2"
else
    TARGET_BUILD_ARCH="default"
fi
LIBRARY_NAME_PINVOKE="SDL2"

$DIR/ext/scripts/c/library/main.sh \
    $DIR/ext/SDL \
    $DIR/lib \
    $LIBRARY_NAME \
    $LIBRARY_NAME_PINVOKE \
    $TARGET_BUILD_OS \
    $TARGET_BUILD_ARCH \
    "-DSDL_STATIC=OFF" "-DSDL_TEST=OFF" "-DSDL_LEAN_AND_MEAN=1"