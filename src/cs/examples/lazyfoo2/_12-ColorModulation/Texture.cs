// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public sealed unsafe class Texture : IDisposable
{
    private bool _isDisposed;
    private readonly SDL_Renderer* _renderer;
    private SDL_Texture* _texture = null;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public Texture(SDL_Renderer* renderer)
    {
        _renderer = renderer;
    }

    public void LoadFromFile(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        using var filePathC = (CString)filePath;
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
        }

        // Color key image
        SDL_SetSurfaceColorKey(surface, true, SDL_MapSurfaceRGB(surface, 0, 0xFF, 0xFF));

        var texture = SDL_CreateTextureFromSurface(_renderer, surface);
        if (texture == null)
        {
            Console.Error.WriteLine("Failed to create texture from file '{0}'. SDL error: {1}", filePath, SDL_GetError());
            Environment.Exit(1);
        }

        Width = surface->w;
        Height = surface->h;
        SDL_DestroySurface(surface);

        _texture = texture;
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
