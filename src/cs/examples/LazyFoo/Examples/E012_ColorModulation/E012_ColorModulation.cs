// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E012_ColorModulation : ExampleLazyFoo
{
    private readonly Texture _texture = new();
    private byte _r = 255;
    private byte _g = 255;
    private byte _b = 255;

    public E012_ColorModulation()
        : base("12 - Color Modulation")
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
        _texture.Dispose();
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        var key = e.scancode;
        switch (key)
        {
            case SDL_Scancode.SDL_SCANCODE_Q:
                _r += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_W:
                _g += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_E:
                _b += 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_A:
                _r -= 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_S:
                _g -= 32;
                break;
            case SDL_Scancode.SDL_SCANCODE_D:
                _b -= 32;
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

        _texture.SetColor(_r, _g, _b);
        _texture.Render(0, 0);

        // Update screen
        SDL_RenderPresent(Renderer);

        return true;
    }

    private bool LoadAssets()
    {
        if (!_texture.LoadFromFile(Renderer, AssetsDirectory, "colors.png", Rgba8U.Cyan))
        {
            return false;
        }

        return true;
    }
}
