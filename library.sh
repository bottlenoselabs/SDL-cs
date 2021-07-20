#!/bin/bash

unamestr="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$unamestr" == "linux" ]]; then
    cmake -S ./ext/SDL -B ./cmake-build-release -G 'Ninja'
elif [[ "$unamestr" == "darwin" ]]; then
    cmake -S ./ext/SDL -B ./cmake-build-release
else
    echo "Unknown platform: '$unamestr'."
fi

cmake --build ./cmake-build-release --config Release

mkdir -p "./lib/"
if [[ "$unamestr" == "linux" ]]; then
    echo "TODO"
elif [[ "$unamestr" == "darwin" ]]; then
    if [ -f "./cmake-build-release/libSDL2-2.0.dylib" ]; then
        mv "./cmake-build-release/libSDL2-2.0.dylib" "./lib/libSDL2.dylib"
    fi
fi

rm -r ./cmake-build-release