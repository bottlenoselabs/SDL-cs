#!/bin/bash

function exit_if_last_command_failed() {
    error=$?
    if [ $error -ne 0 ]; then
        exit $error
    fi
}

function bindgen {
    c2cs ast -i ./ext/SDL/include/SDL.h -o ./ast/SDL.json -s ./ext/SDL/include -b 64 -g \
"begin_code.h" \
"end_code.h" \
"SDL_endian.h" \
"SDL_config_android.h" \
"SDL_config_emscripten.h" \
"SDL_config_iphoneos.h" \
"SDL_config_macosx.h" \
"SDL_config_minimal.h" \
"SDL_config_os2.h" \
"SDL_config_pandora.h" \
"SDL_config_psp.h" \
"SDL_config_windows.h" \
"SDL_config_winrt.h" \
"SDL_config_wiz.h" \
"SDL_config.h" \
-d \
SDL_DISABLE_IMMINTRIN_H \
SDL_DISABLE_MMINTRIN_H \
SDL_DISABLE_XMMINTRIN_H \
SDL_DISABLE_EMMINTRIN_H \
SDL_DISABLE_PMMINTRIN_H \
    exit_if_last_command_failed
    c2cs cs -i ./ast/SDL.json -o ./src/cs/production/SDL/SDL.cs -l "SDL2" -c "SDL" -g ./ignored.txt \
-a \
"_SDL_iconv_t* -> nint" \
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

bindgen
