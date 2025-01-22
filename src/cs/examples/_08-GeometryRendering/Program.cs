// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public static unsafe class Program
{
    // https://lazyfoo.net/tutorials/SDL/08_geometry_rendering/index.php

    public static readonly ProgramState State = new();

    private static int Main()
    {
        Interop.SDL.Initialize();

        Initialize();
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

        // Render red filled quad
        SDL_FRect fillRect = default;
        fillRect.x = State.ScreenWidth / 4.0f;
        fillRect.y = State.ScreenHeight / 4.0f;
        fillRect.w = State.ScreenWidth / 2.0f;
        fillRect.h = State.ScreenHeight / 2.0f;
        SDL_SetRenderDrawColor(State.Renderer, 0xFF, 0x00, 0x00, 0xFF);
        SDL_RenderFillRect(State.Renderer, &fillRect);

        // Render green outlined quad
        SDL_FRect outlineRect = default;
        outlineRect.x = State.ScreenWidth / 6.0f;
        outlineRect.y = State.ScreenHeight / 6.0f;
        outlineRect.w = State.ScreenWidth * 2.0f / 3.0f;
        outlineRect.h = State.ScreenHeight * 2.0f / 3.0f;
        SDL_SetRenderDrawColor(State.Renderer, 0x00, 0xFF, 0x00, 0xFF);
        SDL_RenderRect(State.Renderer, &outlineRect);

        // Draw blue horizontal line
        SDL_SetRenderDrawColor(State.Renderer, 0x00, 0x00, 0xFF, 0xFF);
        SDL_RenderLine(State.Renderer, 0, State.ScreenHeight / 2.0f, State.ScreenWidth, State.ScreenHeight / 2.0f);

        // Draw vertical line of yellow dots
        SDL_SetRenderDrawColor(State.Renderer, 0xFF, 0xFF, 0x00, 0xFF);
        for (var i = 0; i < State.ScreenHeight; i += 4)
        {
            SDL_RenderPoint(State.Renderer, State.ScreenWidth / 2.0f, i);
        }

        // Update screen
        SDL_RenderPresent(State.Renderer);
    }
}
