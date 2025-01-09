// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using static bottlenoselabs.SDL;

namespace SDL.Samples;

public unsafe class ProgramState
{
    public int ScreenWidth = 800;
    public int ScreenHeight = 600;

    public SDL_Window* Window = null;
    public SDL_Surface* ScreenSurface = null;
    public SDL_Surface* UserSurface = null;

    public ProgramState()
    {
    }
}
