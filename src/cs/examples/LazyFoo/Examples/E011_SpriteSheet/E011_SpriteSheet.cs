// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E011_SpriteSheet : ExampleLazyFoo
{
    private readonly Texture _textureDots = new();
    private readonly SDL_Rect[] _spriteSourceRectangles = new SDL_Rect[4];

    public E011_SpriteSheet()
        : base("11 - Sprite Sheet")
    {
    }

    public override bool Initialize(IAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        return LoadAssets();
    }

    public override void Quit()
    {
        _textureDots.Dispose();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
    }

    public override bool Update(float deltaTime)
    {
        return true;
    }

    public override bool Draw(float deltaTime)
    {
        // Clear screen
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(Renderer);

        // Render top left sprite
        _textureDots.Render(
            0,
            0,
            _spriteSourceRectangles[0]);

        // Render top right sprite
        _textureDots.Render(
            ScreenWidth - _spriteSourceRectangles[1].w,
            0,
            _spriteSourceRectangles[1]);

        // Render bottom left sprite
        _textureDots.Render(
            0,
            ScreenHeight - _spriteSourceRectangles[2].h,
            _spriteSourceRectangles[2]);

        // Render bottom right sprite
        _textureDots.Render(
            ScreenWidth - _spriteSourceRectangles[3].w,
            ScreenHeight - _spriteSourceRectangles[3].h,
            _spriteSourceRectangles[3]);

        // Update screen
        SDL_RenderPresent(Renderer);

        return true;
    }

    private bool LoadAssets()
    {
        if (!_textureDots.LoadFromFile(Renderer, AssetsDirectory, "dots.png", Rgba8U.Cyan))
        {
            return false;
        }

        // Set top left sprite
        ref var topLeftSprite = ref _spriteSourceRectangles[0];
        topLeftSprite.x = 0;
        topLeftSprite.y = 0;
        topLeftSprite.w = 100;
        topLeftSprite.h = 100;

        // Set top right sprite
        ref var topRightSprite = ref _spriteSourceRectangles[1];
        topRightSprite.x = 100;
        topRightSprite.y = 0;
        topRightSprite.w = 100;
        topRightSprite.h = 100;

        // Set bottom left sprite
        ref var bottomLeftSprite = ref _spriteSourceRectangles[2];
        bottomLeftSprite.x = 0;
        bottomLeftSprite.y = 100;
        bottomLeftSprite.w = 100;
        bottomLeftSprite.h = 100;

        // Set bottom right sprite
        ref var bottomRightSprite = ref _spriteSourceRectangles[3];
        bottomRightSprite.x = 100;
        bottomRightSprite.y = 100;
        bottomRightSprite.w = 100;
        bottomRightSprite.h = 100;

        return true;
    }
}
