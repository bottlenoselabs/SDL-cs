// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/02_getting_an_image_on_the_screen/index.php

    public static readonly ProgramState State = new();

    private static int Main()
    {
        Initialize();

        if (LoadMedia())
        {
            _ = SDL_BlitSurface(State.UserSurface, default, State.ScreenSurface, default);
            _ = SDL_UpdateWindowSurface(State.Window); // flip back and front buffer
        }

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
            (CString)"SDL Sample: Getting an image on screen"u8,
            640,
            480,
            SDL_WINDOW_RESIZABLE);
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
    }

    private static bool LoadMedia()
    {
        var filePath = AppContext.BaseDirectory + "/hello_world.bmp";
        using var filePathC = (CString)filePath;
        State.UserSurface = SDL_LoadBMP(filePathC);

        if (State.UserSurface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
            return false;
        }

        return true;
    }
}
