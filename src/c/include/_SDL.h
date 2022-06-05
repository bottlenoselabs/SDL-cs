#pragma once

#if defined(__APPLE__) && __has_include("AvailabilityMacros.h")
    #include <AvailabilityMacros.h>
#endif

#define SDL_DISABLE_IMMINTRIN_H
#define SDL_DISABLE_MMINTRIN_H
#define SDL_DISABLE_XMMINTRIN_H
#define SDL_DISABLE_EMMINTRIN_H
#define SDL_DISABLE_PMMINTRIN_H

#include "SDL.h"