// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E004_KeyPresses : ExampleLazyFoo
{
    private SDL_Surface* _screenSurface;
    private SDL_Surface* _currentKeyPressSurface;
    private readonly SDL_Surface*[] _keyPressSurfaces = new SDL_Surface*[Enum.GetValues<KeyPressSurfaceIndex>().Length];

    public E004_KeyPresses()
        : base("4 - KeyPresses", createRenderer: false)
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
        foreach (var keyPressSurface in _keyPressSurfaces)
        {
            if (keyPressSurface == null)
            {
                SDL_DestroySurface(keyPressSurface);
            }
        }
    }

    public override void KeyboardEvent(SDL_KeyboardEvent e)
    {
        if (e.down)
        {
            var keyPressSurface = e.scancode switch
            {
                SDL_Scancode.SDL_SCANCODE_UP => KeyPressSurfaceIndex.Up,
                SDL_Scancode.SDL_SCANCODE_DOWN => KeyPressSurfaceIndex.Down,
                SDL_Scancode.SDL_SCANCODE_LEFT => KeyPressSurfaceIndex.Left,
                SDL_Scancode.SDL_SCANCODE_RIGHT => KeyPressSurfaceIndex.Right,
                _ => KeyPressSurfaceIndex.Press
            };

            _currentKeyPressSurface = _keyPressSurfaces[(int)keyPressSurface];
        }
    }

    public override bool Update(float deltaTime)
    {
        return true;
    }

    public override bool Draw(float deltaTime)
    {
        _ = SDL_BlitSurface(_currentKeyPressSurface, null, _screenSurface, null);
        _ = SDL_UpdateWindowSurface(Window); // flip back and front buffer
        return true;
    }

    private bool LoadAssets()
    {
        _currentKeyPressSurface = _keyPressSurfaces[(int)KeyPressSurfaceIndex.Press] = LoadSurface("press.bmp");
        _keyPressSurfaces[(int)KeyPressSurfaceIndex.Up] = LoadSurface("up.bmp");
        _keyPressSurfaces[(int)KeyPressSurfaceIndex.Down] = LoadSurface("down.bmp");
        _keyPressSurfaces[(int)KeyPressSurfaceIndex.Left] = LoadSurface("left.bmp");
        _keyPressSurfaces[(int)KeyPressSurfaceIndex.Right] = LoadSurface("right.bmp");

        foreach (var keyPressSurface in _keyPressSurfaces)
        {
            if (keyPressSurface == null)
            {
                return false;
            }
        }

        return true;
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
