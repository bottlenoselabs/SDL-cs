// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.CompilerServices;
using Interop.Runtime;
using static Interop.SDL;

#pragma warning disable IDE0008
#pragma warning disable IDE0160
#pragma warning disable IDE0210
[assembly: DisableRuntimeMarshalling]

namespace SDL.Samples;

internal static unsafe class Program
{
    private static int Main()
    {
        if (!SDL_Init(SDL_INIT_VIDEO))
        {
            Console.Error.WriteLine("Failed to initialize SDL. Message: " + SDL_GetError());
            Environment.Exit(1);
        }

        var windowNameC = (CString)"SDL Sample: Hello world!"u8;
        var window = SDL_CreateWindow(
            windowNameC,
            800,
            600,
            0);
        if (window == null)
        {
            Console.Error.WriteLine("Failed to create window. Message: " + SDL_GetError());
            Environment.Exit(1);
        }

        var screenSurface = SDL_GetWindowSurface(window);
        var (r, g, b) = (100, 149, 237);
        var pixelFormat = SDL_GetWindowPixelFormat(window);
        var details = SDL_GetPixelFormatDetails(pixelFormat);
        var color = SDL_MapRGB(details, null, (byte)r, (byte)g, (byte)b);
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
#pragma warning restore IDE0210
}
