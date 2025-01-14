// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/05_optimized_surface_loading_and_soft_stretching/index.php

    public static readonly ProgramState State = new();

    private static int Main()
    {
        Initialize();
        TryLoadMedia();
        Loop();
        Close();
        return 0;
    }

    private static void Initialize()
    {
        if (!SDL_Init(SDL_INIT_VIDEO))
        {
            Console.Error.WriteLine("Failed to initialize SDL. SDL error: " + SDL_GetError());
            Environment.Exit(1);
        }

        State.Window = SDL_CreateWindow(
            (CString)"SDL Example: Optimized surface loading and soft stretching"u8, State.ScreenWidth, State.ScreenHeight, 0);
        if (State.Window == null)
        {
            Console.Error.WriteLine("Failed to create window. SDL error: " + SDL_GetError());
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
        // Apply the image stretched
        SDL_Rect stretchRectangle;
        stretchRectangle.x = 0;
        stretchRectangle.y = 0;
        stretchRectangle.w = State.ScreenWidth;
        stretchRectangle.h = State.ScreenHeight;
        _ = SDL_BlitSurfaceScaled(
            State.UserSurface,
            default,
            State.ScreenSurface,
            &stretchRectangle,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST);

        // flip back and front buffer
        _ = SDL_UpdateWindowSurface(State.Window);
    }

    private static void TryLoadMedia()
    {
        State.UserSurface = TryLoadSurface("stretch.bmp");
    }

    private static SDL_Surface* TryLoadSurface(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        using var filePathC = (CString)fileName;
        var loadedSurface = SDL_LoadBMP(filePathC);
        if (loadedSurface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
            return null;
        }

        var screenSurface = State.ScreenSurface->format;
        var optimizedSurface = SDL_ConvertSurface(loadedSurface, screenSurface);
        if (optimizedSurface == null)
        {
            Console.Error.WriteLine("Failed to optimize image '{0}'. SDL Error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
        }

        SDL_DestroySurface(loadedSurface);

        return optimizedSurface;
    }
}
