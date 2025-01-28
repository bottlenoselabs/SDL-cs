// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E007_TextureLoading : ExampleLazyFoo
{
    private SDL_Texture* _texture;

    public E007_TextureLoading()
        : base("7 - Texture Loading")
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
        SDL_DestroyTexture(_texture);
        _texture = null;
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
        SDL_RenderClear(Renderer);
        SDL_RenderTexture(Renderer, _texture, null, null);
        SDL_RenderPresent(Renderer);

        return true;
    }

    private bool LoadAssets(INativeAllocator allocator)
    {
        _texture = LoadTexture(allocator, "texture.png");
        return _texture != null;
    }

    private SDL_Texture* LoadTexture(INativeAllocator allocator, string fileName)
    {
        var filePath = Path.Combine(AssetsDirectory, fileName);
        var filePathC = allocator.AllocateCString(filePath);
        var surface = IMG_Load(filePathC);
        if (surface == null)
        {
            Console.Error.WriteLine("Failed to load image '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        var texture = SDL_CreateTextureFromSurface(Renderer, surface);
        SDL_DestroySurface(surface);
        if (texture == null)
        {
            Console.Error.WriteLine("Failed to create texture from file '{0}': {1}", filePath, SDL_GetError());
            return null;
        }

        return texture;
    }
}
