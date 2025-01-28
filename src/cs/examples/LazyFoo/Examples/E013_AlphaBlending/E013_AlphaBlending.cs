// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E013_AlphaBlending : ExampleLazyFoo
{
    private readonly Texture _textureFadeout = new();
    private readonly Texture _textureFadein = new();
    private byte _a = 255;

    public E013_AlphaBlending()
        : base("13 - Alpha Blending")
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        return LoadAssets(allocator);
    }

    public override void Quit()
    {
        _textureFadeout.Dispose();
        _textureFadein.Dispose();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        var key = e.scancode;
        switch (key)
        {
            case SDL_Scancode.SDL_SCANCODE_W:
                if (_a + 32 > 255)
                {
                    _a = 255;
                }
                else
                {
                    _a += 32;
                }

                break;
            case SDL_Scancode.SDL_SCANCODE_S:
                if (_a - 32 < 0)
                {
                    _a = 0;
                }
                else
                {
                    _a -= 32;
                }

                break;
        }
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

        _textureFadein.Render(0, 0);

        _textureFadeout.SetAlpha(_a);
        _textureFadeout.Render(0, 0);

        // Update screen
        SDL_RenderPresent(Renderer);

        return true;
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        if (!_textureFadeout.LoadFromFile(allocator, Renderer, AssetsDirectory, "fadeout.png"))
        {
            return false;
        }

        _textureFadeout.SetBlendMode(SDL_BLENDMODE_BLEND);

        if (!_textureFadein.LoadFromFile(allocator, Renderer, AssetsDirectory, "fadein.png"))
        {
            return false;
        }

        return true;
    }
}
