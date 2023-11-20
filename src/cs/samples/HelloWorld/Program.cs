// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using bottlenoselabs.C2CS.Runtime;
using static bottlenoselabs.SDL;

namespace HelloWorld;

internal static unsafe class Program
{
    private struct ProgramState
    {
        public SDL_Window* Window;
    }

    private static ProgramState _state;

    private static int Main()
    {
        var errorCode = SDL_Init((uint)SDL_InitFlags.SDL_INIT_VIDEO);
        CheckError(errorCode);
        CreateWindow();

        while (true)
        {
            // Get the next event
            SDL_Event e;
            if (SDL_PollEvent(&e))
            {
                Frame();

                if (e.type == (ulong)SDL_EventType.SDL_EVENT_QUIT)
                {
                    // Break out of the loop on quit
                    break;
                }
            }
        }

        SDL_DestroyWindow(_state.Window);
        SDL_Quit();

        return 0;
    }

    private static void Frame()
    {
        var screenSurface = SDL_GetWindowSurface(_state.Window);
        var errorCode = SDL_FillSurfaceRect(screenSurface, default, SDL_MapRGB(screenSurface->format, 0xFF, 0x00, 0x00));
        CheckError(errorCode);
        errorCode = SDL_UpdateWindowSurface(_state.Window);
        CheckError(errorCode);
    }

    private static void CreateWindow()
    {
        _state.Window = SDL_CreateWindow(
            (CString)"SDL: Hello, world!",
            800,
            600,
            (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        if (_state.Window == null)
        {
            CheckError();
        }

        PrintWindowFlags(_state.Window);
    }

    private static void PrintWindowFlags(SDL_Window* window)
    {
        // See: SDL_WindowFlags @ SDL_video.h
        var windowFlags = SDL_GetWindowFlags(window);

        Console.WriteLine(@$"Window: ""Fullscreen"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0}");
        Console.WriteLine(@$"WindowL ""OpenGL"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_OPENGL) != 0}");
        Console.WriteLine(@$"Window: ""Hidden"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_HIDDEN) != 0}");
        Console.WriteLine(@$"Window: ""Borderless"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0}");
        Console.WriteLine(@$"Window: ""Resizeable"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0}");
        Console.WriteLine(@$"Window: ""Minimized"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MINIMIZED) != 0}");
        Console.WriteLine(@$"Window: ""Maximized"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) != 0}");
        Console.WriteLine(@$"Window: ""Mouse grabbed"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED) != 0}");
        Console.WriteLine(@$"Window: ""Input focus"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0}");
        Console.WriteLine(@$"Window: ""Mouse focus"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) != 0}");
        Console.WriteLine(@$"Window: ""Mouse capture"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE) != 0}");
        Console.WriteLine(@$"Window: ""Always on top"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP) != 0}");
        Console.WriteLine(@$"Window: ""Utility"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_UTILITY) != 0}");
        Console.WriteLine(@$"Window: ""Tooltip"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_TOOLTIP) != 0}");
        Console.WriteLine(@$"Window: ""Popup menu"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_POPUP_MENU) != 0}");
        Console.WriteLine(@$"Window: ""Keyboard grabbed"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_KEYBOARD_GRABBED) != 0}");
        Console.WriteLine(@$"Window: ""Vulkan"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_VULKAN) != 0}");
        Console.WriteLine(@$"Window: ""Metal"" = {(windowFlags & (ulong)SDL_WindowFlags.SDL_WINDOW_METAL) != 0}");
    }

    private static void CheckError(int? errorCode = -1)
    {
        if (errorCode >= 0)
        {
            return;
        }

        var error = SDL_GetError();
        Console.Error.WriteLine($"SDL2 error: {error.ToString()}");
        Environment.Exit(1);
    }
}
