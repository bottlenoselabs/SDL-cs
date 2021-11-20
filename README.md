# sdl-cs

Automatically updated C# bindings for https://github.com/libsdl-org/SDL with native dynamic link libraries.

## How to use

### NuGet packages

1. Install the .NET 6 `bottlenoselabs.SDL2` NuGet package from `nuget.org`. This package contains only the C# bindings.
2. Install one of the following packages which contain the native library for the runtime identifier. More than one these packages can be installed at once if desired but is not strictly required and will waste disk space unnecessarily. 
   -  `bottlenoselabs.SDL2.runtime.win-x64`: The `SDL.dll` native binary for Windows (64-bit).
   -  `bottlenoselabs.SDL2.runtime.osx`: The `libSDL.dylib` for macOS Intel (`osx-x64`) + macOS Apple Silicon (`osx-arm64`).
   -  `bottlenoselabs.SDL2.runtime.linux-x64`: The `libSDL.so` for Linux x64.

### From source

1. Download and install [.NET 6](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/bottlenoselabs/SDL-cs`.
3. Build the native library by running `bash ./library.sh`. (Building for Windows requires Windows Subsystem for Linux with Ubuntu).
4. Add the C# project `./src/cs/production/SDL-cs/SDL-cs.csproj` to your solution.
5. Add one of the C# shim projects for the native binary in the same way as `SDL-cs`.
   -  `SDL2-cs.win-x64`: The `SDL.dll` native binary for Windows x64.
   -  `SDL2-cs.osx`: The `libSDL.dylib` for macOS Intel (`osx-x64`) + macOS Apple Silicon (`osx-arm64`).
   -  `SDL2-cs.linux-x64`: The `libSDL.so` for Linux x64.

If you wish to re-generate the bindings, simple run `bash ./bindgen.sh`.

## Developers: Documentation

- For more information on how C# bindings work, see [`C2CS`](https://github.com/bottlenoselabs/c2cs), the tool that generates the bindings for `SDL` and other C libraries.
- To learn how to use `SDL`, check out the [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
