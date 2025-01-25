// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace bottlenoselabs.Interop;

// ReSharper disable once InconsistentNaming
public static partial class SDL_image
{
    /// <summary>
    ///     Initializes SDL native interoperability.
    /// </summary>
    public static void Initialize()
    {
        NativeLibrary.SetDllImportResolver(
            Assembly.GetExecutingAssembly(),
            static (libraryName, assembly, searchPath) =>
                SDL.ResolveNativeLibrary(libraryName, assembly, searchPath, "SDL3_image"));
    }
}
