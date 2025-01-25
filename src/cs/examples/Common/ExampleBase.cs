// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace Common;

public abstract unsafe class ExampleBase
{
    public SDL_Window* Window { get; private set; }

    public string AssetsDirectory { get; set; }

    public WindowOptions WindowOptions { get; }

    private bool _hasQuit;

    public string Name { get; protected set; } = string.Empty;

    public int ScreenWidth { get; private set; }

    public int ScreenHeight { get; private set; }

    protected ExampleBase(WindowOptions? windowOptions = null)
    {
        AssetsDirectory = AppContext.BaseDirectory;
        WindowOptions = windowOptions ?? new WindowOptions
        {
            Width = 640,
            Height = 480
        };
    }

    public abstract bool Initialize();

    public abstract void Quit();

    public abstract void KeyboardEvent(SDL_KeyboardEvent e);

    public abstract bool Update(float deltaTime);

    public abstract bool Draw(float deltaTime);

    internal void QuitInternal()
    {
        var hasQuit = Interlocked.Exchange(ref _hasQuit, true);
        if (hasQuit)
        {
            return;
        }

        Quit();

        SDL_DestroyWindow(Window);
        Window = null;
    }

    internal bool InitializeInternal()
    {
        using var exampleNameCString = (CString)Name;
        Window = SDL_CreateWindow(
            exampleNameCString,
            WindowOptions.Width,
            WindowOptions.Height,
            0);
        if (Window == null)
        {
            Console.Error.WriteLine("CreateWindow failed: " + SDL_GetError());
            return false;
        }

        int windowWidth;
        int windowHeight;
        SDL_GetWindowSize(Window, &windowWidth, &windowHeight);
        ScreenWidth = windowWidth;
        ScreenHeight = windowHeight;

        return Initialize();
    }
}
