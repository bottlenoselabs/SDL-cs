#!/bin/bash

cmake -S ./ext/SDL -B ./cmake-build-release
cmake --build ./cmake-build-release --config Release
mkdir -p "./lib/"

unamestr="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$unamestr" == "linux" ]]; then
    sharedobject="$(readlink -f ./cmake-build-release/libSDL2-2.0.so)"
    mv "$sharedobject" "./lib/libSDL2.so"
    echo "Moved $sharedobject to ./lib/libSDL2.so"
elif [[ "$unamestr" == "darwin" ]]; then
    mv "./cmake-build-release/libSDL2-2.0.dylib" "./lib/libSDL2.dylib"
    echo "Moved ./cmake-build-release/libSDL2-2.0.dylib to ./lib/libSDL2.dylib"
fi

rm -r ./cmake-build-release