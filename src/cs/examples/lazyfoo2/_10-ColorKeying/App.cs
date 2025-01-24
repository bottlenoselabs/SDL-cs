// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public sealed unsafe class App : IDisposable
{
    public int ScreenWidth = 640;
    public int ScreenHeight = 480;

    public SDL_Window* Window = null;
    public SDL_Renderer* Renderer = null;

    public Texture TextureFoo = null!;
    public Texture TextureBackground = null!;

    public void Run()
    {
        Initialize();
        LoadMedia();
        Loop();
        Close();
    }

    public void Dispose()
    {
        TextureFoo.Dispose();
        TextureBackground.Dispose();

        SDL_DestroyRenderer(Renderer);
        Renderer = null;

        SDL_DestroyWindow(Window);
        Window = null;
    }

    private void Initialize()
    {
        if (!SDL_Init(SDL_INIT_VIDEO))
        {
            Console.Error.WriteLine("Failed to initialize SDL. SDL error: " + SDL_GetError());
            Environment.Exit(1);
        }

        SDL_Window* window;
        SDL_Renderer* renderer;
        if (!SDL_CreateWindowAndRenderer(
                (CString)"SDL Example: Color Keying"u8,
                ScreenWidth,
                ScreenHeight,
                0,
                &window,
                &renderer))
        {
            Console.Error.WriteLine("Failed to create renderer. SDL error: " + SDL_GetError());
            Environment.Exit(1);
        }

        Window = window;
        Renderer = renderer;
    }

    private void LoadMedia()
    {
        TextureFoo = new Texture(Renderer);
        TextureFoo.LoadFromFile("foo.png");

        TextureBackground = new Texture(Renderer);
        TextureBackground.LoadFromFile("background.png");
    }

    private void Loop()
    {
        var isExiting = false;

        while (!isExiting)
        {
            SDL_Event e;
            if (!SDL_PollEvent(&e))
            {
                continue;
            }

            var eventType = (SDL_EventType)e.type;
            if (eventType == SDL_EventType.SDL_EVENT_QUIT)
            {
                isExiting = true;
            }

            Frame();
        }
    }

    private void Frame()
    {
        // Clear screen
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(Renderer);

        // Render background texture to screen
        TextureBackground.Render(0, 0);

        // Render Foo' to the screen
        TextureFoo.Render(240, 190);

        // Update screen
        SDL_RenderPresent(Renderer);
    }

    private void Close()
    {
        Dispose();
        SDL_Quit();
    }
}
