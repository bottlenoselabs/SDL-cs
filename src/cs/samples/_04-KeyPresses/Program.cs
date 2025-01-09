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
        LoadMedia();
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

        const string windowName = "SDL Sample: Key presses";
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
        for (int i = 0; i < (int)KeyPressSurfaces.Total; i++)
        {
            SDL_DestroySurface(State.KeyPressSurfaces[i]);
            State.KeyPressSurfaces[i] = null;
        }

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

        if (e.type == (ulong)SDL_EventType.SDL_EVENT_KEY_DOWN)
        {
            switch (e.key.keysym.scancode)
            {
                case SDL_Scancode.SDL_SCANCODE_UP:
                    State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Up];
                    break;
                case SDL_Scancode.SDL_SCANCODE_DOWN:
                    State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Down];
                    break;
                case SDL_Scancode.SDL_SCANCODE_LEFT:
                    State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Left];
                    break;
                case SDL_Scancode.SDL_SCANCODE_RIGHT:
                    State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Right];
                    break;
                default:
                    State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Default];
                    break;
            }
        }

        return false;
    }

    private static void Frame()
    {
        SDL_BlitSurface(State.CurrentKeyPressSurface, default, State.ScreenSurface, default);
        SDL_UpdateWindowSurface(State.Window); // flip back and front buffer
    }

    private static void LoadMedia()
    {
        State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaces.Default] = LoadSurface(AppContext.BaseDirectory + "/press.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaces.Up] = LoadSurface(AppContext.BaseDirectory + "/up.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaces.Down] = LoadSurface(AppContext.BaseDirectory + "/down.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaces.Left] = LoadSurface(AppContext.BaseDirectory + "/left.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaces.Right] = LoadSurface(AppContext.BaseDirectory + "/right.bmp");
    }

    private static SDL_Surface* LoadSurface(string filePath)
    {
        using var filePathC = (CString)filePath;
        var result = SDL_LoadBMP(filePathC);
        if (result != null)
        {
            return result;
        }

        using var errorMessageC = SDL_GetError();
        var errorMessage = errorMessageC.ToString();
        Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, errorMessage);
        return result;
    }
}
