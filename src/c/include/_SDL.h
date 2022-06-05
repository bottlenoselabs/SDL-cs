#pragma once

#define XSTR(x) STR(x)
#define STR(x) #x
#define ABC 1

#if defined(__APPLE__)
    #if __has_include("AvailabilityMacros.h")
        #include <AvailabilityMacros.h>
    #endif

    #if MAC_OS_X_VERSION_MIN_REQUIRED < 1060
        # error "Test1"
    #endif
#else
    #define MAC_OS_X_VERSION_MIN_REQUIRED 1070
    #if MAC_OS_X_VERSION_MIN_REQUIRED < 1060
        # error "Test2"
    #endif
#endif

#define SDL_DISABLE_IMMINTRIN_H
#define SDL_DISABLE_MMINTRIN_H
#define SDL_DISABLE_XMMINTRIN_H
#define SDL_DISABLE_EMMINTRIN_H
#define SDL_DISABLE_PMMINTRIN_H

#include "SDL.h"