// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using Common;

namespace SDL_GPU;

public static class Program
{
    private static int Main()
    {
        SDL.Initialize();
        SDL_image.Initialize();

        using var app = new App();
        var exitCode = app.Run();
        return exitCode;
    }
}
