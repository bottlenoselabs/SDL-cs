// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;

namespace LazyFoo;

public static class Program
{
    private static int Main()
    {
        Interop.SDL.Initialize();
        Interop.SDL_image.Initialize();

        using var app = new App();
        var exitCode = app.Run();
        return exitCode;
    }
}
