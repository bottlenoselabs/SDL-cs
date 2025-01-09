// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace SDL.Samples;

internal static unsafe class Program
{
    public static readonly ProgramState State = new();

    private static int Main()
    {
        Initialize();

        if (LoadMedia())
        {
        }

        Loop();
        Close();

        return 0;
    }

    private static void Initialize()
    {
        if (SDL_Init((uint)SDL_InitFlags.SDL_INIT_VIDEO) < 0)
        {
            using var errorMessageC = SDL_GetError();
            var errorMessage = errorMessageC.ToString();
            Console.Error.WriteLine("Failed to initialize SDL. SDL error: " + errorMessage);
            Environment.Exit(1);
        }

        const string windowName = "SDL Sample: Event driven programming";
        using var windowNameC = (CString)windowName;
        State.Window = SDL_CreateWindow(
            windowNameC,
            800,
            600,
            (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        if (State.Window == null)
        {
            using var errorMessageC = SDL_GetError();
            var errorMessage = errorMessageC.ToString();
            Console.Error.WriteLine("Failed to create window. SDL error: " + errorMessage);
            Environment.Exit(1);
        }

        State.ScreenSurface = SDL_GetWindowSurface(State.Window);
    }

    private static void Close()
    {
        SDL_DestroySurface(State.UserSurface);
        State.UserSurface = null;

        SDL_DestroyWindow(State.Window);
        State.Window = null;

        SDL_Quit();
    }

    private static void Loop()
    {
        while (true)
        {
            if (HandleEvents())
            {
                break;
            }

            Frame();
        }
    }

    private static bool HandleEvents()
    {
        SDL_Event e;
        if (!SDL_PollEvent(&e))
        {
            return false;
        }

        if (e.type == (ulong)SDL_EventType.SDL_EVENT_QUIT)
        {
            return true;
        }

        return false;
    }

    private static void Frame()
    {
        SDL_BlitSurface(State.UserSurface, default, State.ScreenSurface, default);
        SDL_UpdateWindowSurface(State.Window); // flip back and front buffer
    }

    private static bool LoadMedia()
    {
        var filePath = AppContext.BaseDirectory + "/x.bmp";
        using var filePathC = (CString)filePath;
        State.UserSurface = SDL_LoadBMP(filePathC);
        if (State.UserSurface != null)
        {
            return true;
        }

        using var errorMessageC = SDL_GetError();
        var errorMessage = errorMessageC.ToString();
        Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, errorMessage);

        Console.WriteLine("Failed to load media.");
        return false;
    }
}
