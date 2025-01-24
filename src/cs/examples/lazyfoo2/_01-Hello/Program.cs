// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace SDL.Examples;

internal static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/01_hello_SDL/index2.php

    private static int Main()
    {
        Initialize();

        if (!SDL_Init(SDL_INIT_VIDEO))
        {
            Console.Error.WriteLine("Failed to initialize SDL. SDL_Error: " + SDL_GetError());
            Environment.Exit(1);
        }

        var windowNameC = (CString)"SDL Tutorial: Hello world!"u8;
        var window = SDL_CreateWindow(
            windowNameC,
            800,
            600,
            0);
        if (window == null)
        {
            Console.Error.WriteLine("Failed to create window. SDL_Error: " + SDL_GetError());
            Environment.Exit(1);
        }

        var screenSurface = SDL_GetWindowSurface(window);
        var (r, g, b) = (100, 149, 237);
        var color = SDL_MapSurfaceRGB(screenSurface, (byte)r, (byte)g, (byte)b);
        _ = SDL_FillSurfaceRect(screenSurface, default, color);
        _ = SDL_UpdateWindowSurface(window);

        while (true)
        {
            SDL_Event e;
            if (!SDL_PollEvent(&e))
            {
                continue;
            }

            if (e.type == (ulong)SDL_EventType.SDL_EVENT_QUIT)
            {
                break;
            }
        }

        SDL_DestroyWindow(window);
        SDL_Quit();

        return 0;
    }
}
