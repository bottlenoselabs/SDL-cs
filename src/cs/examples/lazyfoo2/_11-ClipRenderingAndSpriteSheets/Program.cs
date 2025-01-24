// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace SDL.Examples;

public static class Program
{
    // https://lazyfoo.net/tutorials/SDL/11_clip_rendering_and_sprite_sheets/index.php

    private static int Main()
    {
        Interop.SDL.Initialize();
        Interop.SDL_image.Initialize();

        using var app = new App();
        app.Run();

        return 0;
    }
}
