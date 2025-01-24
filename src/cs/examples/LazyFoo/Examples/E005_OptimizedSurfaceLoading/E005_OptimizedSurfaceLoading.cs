// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E005_OptimizedSurfaceLoading : ExampleLazyFoo
{
    private SDL_Surface* _screenSurface;
    private SDL_Surface* _surface;

    public E005_OptimizedSurfaceLoading()
        : base("5 - Optimized Surface Loading", createRenderer: false)
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

        _screenSurface = SDL_GetWindowSurface(Window);
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
        // Apply the image stretched
        SDL_Rect stretchRectangle;
        stretchRectangle.x = 0;
        stretchRectangle.y = 0;
        stretchRectangle.w = ScreenWidth;
        stretchRectangle.h = ScreenHeight;
        _ = SDL_BlitSurfaceScaled(
            _surface,
            null,
            _screenSurface,
            &stretchRectangle,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST);

        // flip back and front buffer
        _ = SDL_UpdateWindowSurface(Window);
        return true;
    }

    private bool LoadAssets()
    {
        _surface = LoadSurface("stretch.bmp");
        return _surface != null;
    }

    private SDL_Surface* LoadSurface(string fileName)
    {
        var filePath = Path.Combine(AssetsDirectory, fileName);
        using var filePathC = (CString)filePath;
        var surface = SDL_LoadBMP(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        return surface;
    }
}
