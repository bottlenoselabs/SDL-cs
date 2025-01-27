// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;

namespace LazyFoo;

public abstract unsafe class ExampleLazyFoo : ExampleBase
{
    private readonly bool _createRenderer;

    public SDL_Renderer* Renderer { get; private set; }

    protected ExampleLazyFoo(
        string name,
        bool createRenderer = true,
        WindowOptions? windowOptions = null)
        : base(windowOptions)
    {
        Name = name;
        _createRenderer = createRenderer;
        AssetsDirectory = Path.Combine(AppContext.BaseDirectory, "Examples", GetType().Name);
    }

    public override bool Initialize(IAllocator allocator)
    {
        if (_createRenderer)
        {
            Renderer = SDL_CreateRenderer(Window, null);
            if (Renderer == null)
            {
                Console.Error.WriteLine("Failed to create renderer. SDL error: " + SDL_GetError());
                return false;
            }
        }

        return true;
    }

    public override void Quit()
    {
        if (_createRenderer)
        {
            SDL_DestroyRenderer(Renderer);
            Renderer = null;
        }
    }
}
