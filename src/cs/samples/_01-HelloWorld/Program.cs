// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace SDL.Samples;

internal static unsafe class Program
{
    private static int Main()
    {
        if (SDL_Init((uint)SDL_InitFlags.SDL_INIT_VIDEO) < 0)
        {
            Console.Error.WriteLine("Failed to initialize SDL. Message: " + SDL_GetError());
            Environment.Exit(1);
        }

        const string windowName = "SDL Sample: Hello world!";
        using var windowNameC = (CString)windowName;
        var window = SDL_CreateWindow(
            windowNameC,
            800,
            600,
            (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        if (window == null)
        {
            Console.Error.WriteLine("Failed to create window. Message: " + SDL_GetError());
            Environment.Exit(1);
        }

        var screenSurface = SDL_GetWindowSurface(window);
        var (r, g, b) = (100, 149, 237);
        SDL_FillSurfaceRect(screenSurface, default, SDL_MapRGB(screenSurface->format, (byte)r, (byte)g, (byte)b));
        SDL_UpdateWindowSurface(window);

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
