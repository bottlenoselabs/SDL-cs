#!/bin/bash

function exit_if_last_command_failed() {
    error=$?
    if [ $error -ne 0 ]; then
        exit $error
    fi
}

function download_C2CS_ubuntu() {
    if [ ! -f "./C2CS" ]; then
        wget https://nightly.link/lithiumtoast/c2cs/workflows/build-test-deploy/develop/ubuntu.20.04-x64.zip
        unzip ./ubuntu.20.04-x64.zip
        rm ./ubuntu.20.04-x64.zip
        chmod +x ./C2CS
    fi
}

function download_C2CS_osx() {
    if [ ! -f "./C2CS" ]; then
        wget https://nightly.link/lithiumtoast/c2cs/workflows/build-test-deploy/develop/osx-x64.zip
        unzip ./osx-x64.zip
        rm ./osx-x64.zip
        chmod +x ./C2CS
    fi
}

function bindgen_linux_64 {
    ./C2CS ast -i ./ext/SDL/include/SDL.h -o ./ast/SDL.linux64.json -s ./ext/SDL/include -b 64 -g SDL_main.h -d \
SDL_DISABLE_MM3DNOW_H \
SDL_DISABLE_IMMINTRIN_H \
SDL_DISABLE_MMINTRIN_H \
SDL_DISABLE_XMMINTRIN_H \
SDL_DISABLE_EMMINTRIN_H \
SDL_DISABLE_PMMINTRIN_H \
    exit_if_last_command_failed
    ./C2CS cs -i ./ast/SDL.linux64.json -o ./src/cs/production/SDL-cs/SDL.linux64.cs -l "SDL2" -c "SDL" -g SDL_bool -a \
"SDL_bool -> CBool" \
"Uint8 -> byte" \
"Uint16 -> ushort" \
"Uint32 -> uint" \
"Uint64 -> ulong" \
"Sint8 -> sbyte" \
"Sint16 -> short" \
"Sint32 -> int" \
"Sint64 -> long" \
    exit_if_last_command_failed
}

function bindgen_linux_32 {
    ./C2CS ast -i ./ext/SDL/include/SDL.h -o ./ast/SDL.linux32.json -s ./ext/SDL/include -b 32 -g SDL_main.h -d \
SDL_DISABLE_MM3DNOW_H \
SDL_DISABLE_IMMINTRIN_H \
SDL_DISABLE_MMINTRIN_H \
SDL_DISABLE_XMMINTRIN_H \
SDL_DISABLE_EMMINTRIN_H \
SDL_DISABLE_PMMINTRIN_H \
    exit_if_last_command_failed
    ./C2CS cs -i ./ast/SDL.linux32.json -o ./src/cs/production/SDL-cs/SDL.linux32.cs -l "SDL2" -c "SDL" -g SDL_bool -a \
"SDL_bool -> CBool" \
"Uint8 -> byte" \
"Uint16 -> ushort" \
"Uint32 -> uint" \
"Uint64 -> ulong" \
"Sint8 -> sbyte" \
"Sint16 -> short" \
"Sint32 -> int" \
"Sint64 -> long" \
    exit_if_last_command_failed
}

unamestr="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$unamestr" == "linux" ]]; then
    download_C2CS_ubuntu
    bindgen_linux_64
    bindgen_linux_32
elif [[ "$unamestr" == "darwin" ]]; then
    download_C2CS_osx
else
    echo "Unknown platform: '$unamestr'."
fi
