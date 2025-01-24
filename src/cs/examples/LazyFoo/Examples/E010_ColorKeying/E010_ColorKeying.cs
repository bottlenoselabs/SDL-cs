// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E010_ColorKeying : ExampleLazyFoo
{
    private readonly Texture _textureFoo = new();
    private readonly Texture _textureBackground = new();

    public E010_ColorKeying()
        : base("10 - Color Keying")
    {
    }

    public override bool Initialize()
    {
        if (!base.Initialize())
        {
            return false;
        }

        return LoadAssets();
    }

    public override void Quit()
    {
        _textureFoo.Dispose();
        _textureBackground.Dispose();
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

        // Render background texture to screen
        _textureBackground.Render(0, 0);

        // Render Foo' to the screen
        _textureFoo.Render(240, 190);

        // Update screen
        SDL_RenderPresent(Renderer);

        return true;
    }

    private bool LoadAssets()
    {
        var colorKey = new SDL_Color { r = 0, g = 0xFF, b = 0xFF };

        if (!_textureFoo.LoadFromFile(Renderer, AssetsDirectory, "foo.png", colorKey))
        {
            return false;
        }

        if (!_textureBackground.LoadFromFile(Renderer, AssetsDirectory, "background.png"))
        {
            return false;
        }

        return true;
    }
}
