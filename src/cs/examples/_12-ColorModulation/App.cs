// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public sealed unsafe class App : IDisposable
{
    public int ScreenWidth = 640;
    public int ScreenHeight = 480;

    public SDL_Window* Window = null;
    public SDL_Renderer* Renderer = null;

    public Texture TextureModulated = null!;

    // Modulation components
    public byte R = 255;
    public byte G = 255;
    public byte B = 255;

    public void Run()
    {
        Initialize();
        LoadMedia();
        Loop();
        Close();
    }

    public void Dispose()
    {
        TextureModulated.Dispose();

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
                (CString)"SDL Example: Color Modulation"u8,
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
        TextureModulated = new Texture(Renderer);
        TextureModulated.LoadFromFile("colors.png");
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
            switch (eventType)
            {
                case SDL_EventType.SDL_EVENT_QUIT:
                    isExiting = true;
                    break;
                case SDL_EventType.SDL_EVENT_KEY_DOWN:
                {
                    var keyCode = (uint)e.key.key;
                    if (keyCode == SDLK_Q)
                    {
                        R += 32;
                    }
                    else if (keyCode == SDLK_W)
                    {
                        G += 32;
                    }
                    else if (keyCode == SDLK_E)
                    {
                        B += 32;
                    }
                    else if (keyCode == SDLK_A)
                    {
                        R -= 32;
                    }
                    else if (keyCode == SDLK_S)
                    {
                        G -= 32;
                    }
                    else if (keyCode == SDLK_D)
                    {
                        B -= 32;
                    }

                    break;
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

        TextureModulated.SetColor(R, G, B);
        TextureModulated.Render(0, 0);

        // Update screen
        SDL_RenderPresent(Renderer);
    }

    private void Close()
    {
        Dispose();
        SDL_Quit();
    }
}
