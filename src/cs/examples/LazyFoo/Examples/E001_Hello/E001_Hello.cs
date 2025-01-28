// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E001_Hello : ExampleLazyFoo
{
    public E001_Hello()
        : base("1 - Hello", createRenderer: false)
    {
    }

    public override bool Initialize(INativeAllocator allocator)
    {
        if (!base.Initialize(allocator))
        {
            return false;
        }

        var screenSurface = SDL_GetWindowSurface(Window);
        var (r, g, b) = (100, 149, 237);
        var color = SDL_MapSurfaceRGB(screenSurface, (byte)r, (byte)g, (byte)b);
        _ = SDL_FillSurfaceRect(screenSurface, null, color);
        _ = SDL_UpdateWindowSurface(Window);

        return true;
    }

    public override void Quit()
    {
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
}
