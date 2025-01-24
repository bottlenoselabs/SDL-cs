// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E002_ImageOnScreen : ExampleLazyFoo
{
    private SDL_Surface* _surface;

    public E002_ImageOnScreen()
        : base("2 - Image on Screen", createRenderer: false)
    {
    }

    public override bool Initialize()
    {
        if (!base.Initialize())
        {
            return false;
        }

        if (!LoadAssets())
        {
            return false;
        }

        var screenSurface = SDL_GetWindowSurface(Window);
        _ = SDL_BlitSurface(_surface, null, screenSurface, null);
        _ = SDL_UpdateWindowSurface(Window); // flip back and front buffer

        return true;
    }

    public override void Quit()
    {
        SDL_DestroySurface(_surface);
        _surface = null;
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
        return true;
    }

    private bool LoadAssets()
    {
        var assetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", nameof(E002_ImageOnScreen));

        var filePath = Path.Combine(assetsDirectory, "hello_world.bmp");
        using var filePathC = (CString)filePath;
        _surface = SDL_LoadBMP(filePathC);
        if (_surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return false;
        }

        return true;
    }
}
