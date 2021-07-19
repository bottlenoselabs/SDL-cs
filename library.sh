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
if [ -f "./lib/libSDL2.a" ]; then
    rm "./lib/libSDL2.a"
fi
for x in ./lib/libSDL2-2.0.so*; do
    if [ -f "$x" ]; then
        mv "$x" "./lib/libSDL2.so"
    fi
done
for x in ./lib/libSDL2-2.0.dylib*; do
    if [ -f "$x" ]; then
        mv "$x" "./lib/libSDL2.dylib"
    fi
done
rm -r ./cmake-build-release