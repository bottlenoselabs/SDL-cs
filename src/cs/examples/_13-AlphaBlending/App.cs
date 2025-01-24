// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public sealed unsafe class App : IDisposable
{
    public int ScreenWidth = 640;
    public int ScreenHeight = 480;

    public SDL_Window* Window = null;
    public SDL_Renderer* Renderer = null;

    public Texture TextureFadeout = null!;
    public Texture TextureFadein = null!;

    public byte Alpha;

    public void Run()
    {
        Initialize();
        LoadMedia();
        Loop();
        Close();
    }

    public void Dispose()
    {
        TextureFadeout.Dispose();
        TextureFadein.Dispose();

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
                (CString)"SDL Example: Alpha Blending"u8,
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
        TextureFadeout = new Texture(Renderer);
        TextureFadeout.LoadFromFile("fadeout.png");
        TextureFadeout.SetBlendMode(SDL_BLENDMODE_BLEND);

        TextureFadein = new Texture(Renderer);
        TextureFadein.LoadFromFile("fadein.png");
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
            else if (eventType == SDL_EventType.SDL_EVENT_KEY_DOWN)
            {
                var keyCode = (uint)e.key.key;
                if (keyCode == SDLK_W)
                {
                    // Increase alpha on w
                    if (Alpha + 32 > 255)
                    {
                        // Cap if over 255
                        Alpha = 255;
                    }
                    else
                    {
                        // Increment otherwise
                        Alpha += 32;
                    }
                }
                else if (keyCode == SDLK_S)
                {
                    // Decrease alpha on s
                    if (Alpha - 32 < 0)
                    {
                        // Cap if below 0
                        Alpha = 0;
                    }
                    else
                    {
                        // Decrement otherwise
                        Alpha -= 32;
                    }
                }
            }

            Frame();
        }
    }

    private void Frame()
    {
        // Clear screen
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(Renderer);

        // Render background
        TextureFadein.Render(0, 0);

        // Render front blended
        TextureFadeout.SetAlpha(Alpha);
        TextureFadeout.Render(0, 0);

        // Update screen
        SDL_RenderPresent(Renderer);
    }

    private void Close()
    {
        Dispose();
        SDL_Quit();
    }
}
