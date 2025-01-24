// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public sealed unsafe class App : IDisposable
{
    public int ScreenWidth = 640;
    public int ScreenHeight = 480;

    public SDL_Window* Window = null;
    public SDL_Renderer* Renderer = null;

    public Texture TextureSpriteSheet = null!;
    public SDL_Rect[] SpriteSourceRectangles = new SDL_Rect[4];

    public void Run()
    {
        Initialize();
        LoadMedia();
        Loop();
        Close();
    }

    public void Dispose()
    {
        TextureSpriteSheet.Dispose();

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
                (CString)"SDL Example: Clip Rendering and Sprite Sheets"u8,
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
        TextureSpriteSheet = new Texture(Renderer);
        TextureSpriteSheet.LoadFromFile("dots.png");

        // Set top left sprite
        ref var topLeftSprite = ref SpriteSourceRectangles[0];
        topLeftSprite.x = 0;
        topLeftSprite.y = 0;
        topLeftSprite.w = 100;
        topLeftSprite.h = 100;

        // Set top right sprite
        ref var topRightSprite = ref SpriteSourceRectangles[1];
        topRightSprite.x = 100;
        topRightSprite.y = 0;
        topRightSprite.w = 100;
        topRightSprite.h = 100;

        // Set bottom left sprite
        ref var bottomLeftSprite = ref SpriteSourceRectangles[2];
        bottomLeftSprite.x = 0;
        bottomLeftSprite.y = 100;
        bottomLeftSprite.w = 100;
        bottomLeftSprite.h = 100;

        // Set bottom right sprite
        ref var bottomRightSprite = ref SpriteSourceRectangles[3];
        bottomRightSprite.x = 100;
        bottomRightSprite.y = 100;
        bottomRightSprite.w = 100;
        bottomRightSprite.h = 100;
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

        // Render top left sprite
        TextureSpriteSheet.Render(
            0,
            0,
            SpriteSourceRectangles[0]);

        // Render top right sprite
        TextureSpriteSheet.Render(
            ScreenWidth - SpriteSourceRectangles[1].w,
            0,
            SpriteSourceRectangles[1]);

        // Render bottom left sprite
        TextureSpriteSheet.Render(
            0,
            ScreenHeight - SpriteSourceRectangles[2].h,
            SpriteSourceRectangles[2]);

        // Render bottom right sprite
        TextureSpriteSheet.Render(
            ScreenWidth - SpriteSourceRectangles[3].w,
            ScreenHeight - SpriteSourceRectangles[3].h,
            SpriteSourceRectangles[3]);

        // Update screen
        SDL_RenderPresent(Renderer);
    }

    private void Close()
    {
        Dispose();
        SDL_Quit();
    }
}
