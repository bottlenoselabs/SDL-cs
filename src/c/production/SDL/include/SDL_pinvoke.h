#pragma once

#if defined(__APPLE__) && __has_include("AvailabilityMacros.h")
    #include <AvailabilityMacros.h>
    #if MAC_OS_X_VERSION_MIN_REQUIRED < 1060
        #undef MAC_OS_X_VERSION_MIN_REQUIRED
        #define MAC_OS_X_VERSION_MIN_REQUIRED MAC_OS_X_VERSION_10_7
    #endif
#endif

#define SDL_DISABLE_IMMINTRIN_H
#define SDL_DISABLE_MMINTRIN_H
#define SDL_DISABLE_XMMINTRIN_H
#define SDL_DISABLE_EMMINTRIN_H
#define SDL_DISABLE_PMMINTRIN_H

#include "SDL3/SDL.h"