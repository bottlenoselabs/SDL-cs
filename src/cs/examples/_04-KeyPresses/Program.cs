// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/03_event_driven_programming/index.php

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
            (CString)"SDL Example: Event driven programming"u8, 640, 480, 0);
        if (State.Window == null)
        {
            Console.Error.WriteLine("Failed to create window. SDL error: " + SDL_GetError());
            Environment.Exit(1);
        }

        State.ScreenSurface = SDL_GetWindowSurface(State.Window);
    }

    private static void Close()
    {
        for (var i = 0; i < State.KeyPressSurfaces.Length; i++)
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
            var keyPressSurface = e.key.scancode switch
            {
                SDL_Scancode.SDL_SCANCODE_UP => KeyPressSurfaceIndex.Up,
                SDL_Scancode.SDL_SCANCODE_DOWN => KeyPressSurfaceIndex.Down,
                SDL_Scancode.SDL_SCANCODE_LEFT => KeyPressSurfaceIndex.Left,
                SDL_Scancode.SDL_SCANCODE_RIGHT => KeyPressSurfaceIndex.Right,
                _ => KeyPressSurfaceIndex.Press
            };

            State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)keyPressSurface];
        }

        return false;
    }

    private static void Frame()
    {
        _ = SDL_BlitSurface(State.CurrentKeyPressSurface, default, State.ScreenSurface, default);
        _ = SDL_UpdateWindowSurface(State.Window); // flip back and front buffer
    }

    private static void TryLoadMedia()
    {
        State.CurrentKeyPressSurface = State.KeyPressSurfaces[(int)KeyPressSurfaceIndex.Press] = TryLoadSurface("press.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaceIndex.Up] = TryLoadSurface("up.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaceIndex.Down] = TryLoadSurface("down.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaceIndex.Left] = TryLoadSurface("left.bmp");
        State.KeyPressSurfaces[(int)KeyPressSurfaceIndex.Right] = TryLoadSurface("right.bmp");
    }

    private static SDL_Surface* TryLoadSurface(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        using var filePathC = (CString)fileName;
        var result = SDL_LoadBMP(filePathC);
        if (result != null)
        {
            return result;
        }

        Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
        return result;
    }
}
