// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Interop;

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/07_texture_loading_and_rendering/index.php

    public static readonly ProgramState State = new();

    private static int Main()
    {
        Interop.SDL.Initialize();
        Interop.SDL_image.Initialize();

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

        SDL_Window* window;
        SDL_Renderer* renderer;
        if (!SDL_CreateWindowAndRenderer(
                (CString)"SDL Example: Texture Loading and Rendering"u8,
                State.ScreenWidth,
                State.ScreenHeight,
                0,
                &window,
                &renderer))
        {
            Console.Error.WriteLine("Failed to create renderer. SDL error: " + SDL_GetError());
            Environment.Exit(1);
        }

        State.Window = window;
        State.Renderer = renderer;
        SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
    }

    private static void Close()
    {
        SDL_DestroyTexture(State.Texture);
        State.Texture = null;

        SDL_DestroyRenderer(State.Renderer);
        State.Renderer = null;

        SDL_DestroyWindow(State.Window);
        State.Window = null;

        SDL_Quit();
    }

    private static void Loop()
    {
        while (true)
        {
            var isExit = HandleEvents();
            if (isExit)
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
        SDL_RenderClear(State.Renderer);
        SDL_RenderTexture(State.Renderer, State.Texture, null, null);
        SDL_RenderPresent(State.Renderer);
    }

    private static void TryLoadMedia()
    {
        State.Texture = TryLoadTexture("texture.png");
    }

    private static SDL_Texture* TryLoadTexture(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        using var filePathC = (CString)filePath;
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
        }

        var texture = SDL_CreateTextureFromSurface(State.Renderer, surface);
        if (texture == null)
        {
            Console.Error.WriteLine("Failed to create texture from file '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
        }

        return texture;
    }
}
