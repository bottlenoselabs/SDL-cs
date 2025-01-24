// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LazyFoo.Examples;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public sealed unsafe class E008_GeometryRendering : ExampleLazyFoo
{
    public E008_GeometryRendering()
        : base("8 - Geometry Rendering")
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
        // Clear screen
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL_RenderClear(Renderer);

        // Render red filled quad
        SDL_FRect fillRect = default;
        fillRect.x = ScreenWidth / 4.0f;
        fillRect.y = ScreenHeight / 4.0f;
        fillRect.w = ScreenWidth / 2.0f;
        fillRect.h = ScreenHeight / 2.0f;
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0x00, 0x00, 0xFF);
        SDL_RenderFillRect(Renderer, &fillRect);

        // Render green outlined quad
        SDL_FRect outlineRect = default;
        outlineRect.x = ScreenWidth / 6.0f;
        outlineRect.y = ScreenHeight / 6.0f;
        outlineRect.w = ScreenWidth * 2.0f / 3.0f;
        outlineRect.h = ScreenHeight * 2.0f / 3.0f;
        SDL_SetRenderDrawColor(Renderer, 0x00, 0xFF, 0x00, 0xFF);
        SDL_RenderRect(Renderer, &outlineRect);

        // Draw blue horizontal line
        SDL_SetRenderDrawColor(Renderer, 0x00, 0x00, 0xFF, 0xFF);
        SDL_RenderLine(Renderer, 0, ScreenHeight / 2.0f, ScreenWidth, ScreenHeight / 2.0f);

        // Draw vertical line of yellow dots
        SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0x00, 0xFF);
        for (var i = 0; i < ScreenHeight; i += 4)
        {
            SDL_RenderPoint(Renderer, ScreenWidth / 2.0f, i);
        }

        // Update screen
        SDL_RenderPresent(Renderer);

        return true;
    }
}
