// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Interop;

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/09_the_viewport/index.php

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
        // Clear screen
        SDL_SetRenderDrawColor(State.Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(State.Renderer);

        // Top left corner viewport
        SDL_Rect topLeftViewport;
        topLeftViewport.x = 0;
        topLeftViewport.y = 0;
        topLeftViewport.w = State.ScreenWidth / 2;
        topLeftViewport.h = State.ScreenHeight / 2;
        SDL_SetRenderViewport(State.Renderer, &topLeftViewport);
        // Render texture to screen
        SDL_RenderTexture(State.Renderer, State.Texture, null, null);

        // Top right viewport
        SDL_Rect topRightViewport;
        topRightViewport.x = State.ScreenWidth / 2;
        topRightViewport.y = 0;
        topRightViewport.w = State.ScreenWidth / 2;
        topRightViewport.h = State.ScreenHeight / 2;
        SDL_SetRenderViewport(State.Renderer, &topRightViewport);
        // Render texture to screen
        SDL_RenderTexture(State.Renderer, State.Texture, null, null);

        // Bottom viewport
        SDL_Rect bottomViewport;
        bottomViewport.x = 0;
        bottomViewport.y = State.ScreenHeight / 2;
        bottomViewport.w = State.ScreenWidth;
        bottomViewport.h = State.ScreenHeight / 2;
        SDL_SetRenderViewport(State.Renderer, &bottomViewport);
        // Render texture to screen
        SDL_RenderTexture(State.Renderer, State.Texture, null, null);

        // Update screen
        SDL_RenderPresent(State.Renderer);
    }

    private static void TryLoadMedia()
    {
        State.Texture = TryLoadTexture("viewport.png");
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
