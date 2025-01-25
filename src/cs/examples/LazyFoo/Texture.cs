// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

public sealed unsafe class Texture : IDisposable
{
    private bool _isDisposed;
    private SDL_Renderer* _renderer;
    private SDL_Texture* _texture = null;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool LoadFromFile(
        SDL_Renderer* renderer, string assetsDirectory, string fileName, Rgba8U? colorKey = null)
    {
        var filePath = Path.Combine(assetsDirectory, fileName);
        using var filePathC = (CString)filePath;
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
            return false;
        }

        // Color key image
        if (colorKey != null)
        {
            var colorKey1 = colorKey.Value;
            SDL_SetSurfaceColorKey(
                surface, true, SDL_MapSurfaceRGB(surface, colorKey1.R, colorKey1.G, colorKey1.B));
        }

        var texture = SDL_CreateTextureFromSurface(renderer, surface);
        if (texture == null)
        {
            Console.Error.WriteLine("Failed to create texture from file '{0}'. SDL error: {1}", filePath, SDL_GetError());
            return false;
        }

        Width = surface->w;
        Height = surface->h;
        SDL_DestroySurface(surface);

        _texture = texture;
        _renderer = renderer;
        return true;
    }

    public void Render(int x, int y, SDL_Rect? sourceRectangle = null)
    {
        // Set rendering space and render to screen
        SDL_FRect renderQuad = default;
        renderQuad.x = x;
        renderQuad.y = y;
        renderQuad.w = Width;
        renderQuad.h = Height;

        // Set clip rendering dimensions
        if (sourceRectangle.HasValue)
        {
            renderQuad.w = sourceRectangle.Value.w;
            renderQuad.h = sourceRectangle.Value.h;

            SDL_FRect sourceQuad = default;
            sourceQuad.x = sourceRectangle.Value.x;
            sourceQuad.y = sourceRectangle.Value.y;
            sourceQuad.w = sourceRectangle.Value.w;
            sourceQuad.h = sourceRectangle.Value.h;

            SDL_RenderTexture(_renderer, _texture, &sourceQuad, &renderQuad);
        }
        else
        {
            SDL_RenderTexture(_renderer, _texture, null, &renderQuad);
        }
    }

    public void SetColor(byte red, byte green, byte blue)
    {
        SDL_SetTextureColorMod(_texture, red, green, blue);
    }

    public void SetAlpha(byte alpha)
    {
        SDL_SetTextureAlphaMod(_texture, alpha);
    }

    public void SetBlendMode(SDL_BlendMode blendMode)
    {
        SDL_SetTextureBlendMode(_texture, blendMode);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        if (_texture != null)
        {
            SDL_DestroyTexture(_texture);
            _texture = null;
        }

        Width = 0;
        Height = 0;

        _isDisposed = true;
    }
}
