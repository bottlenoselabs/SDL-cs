#!/bin/bash

cmake -S ./src/c/sdl -B ./cmake-build-release -G 'Unix Makefiles' -DCMAKE_BUILD_TYPE=Release
make -C ./cmake-build-release
if [ -e "./lib/libSDL2-2.0.so.0" ]; then
    rm "./lib/libSDL2-2.0.so.0"
fi
if [ -e "./lib/libSDL2-2.0.so" ]; then
    rm "./lib/libSDL2-2.0.so"
fi
if [ -f "./lib/libSDL2main.a" ]; then
    rm "./lib/libSDL2main.a"
fi
if [ ! -f "./lib/libSDL2-2.0.so.*" ]; then
    for x in ./lib/libSDL2-2.0.so*; do
        mv "$x" "./lib/libSDL2.so"
    done
fi
rm -r ./cmake-build-release