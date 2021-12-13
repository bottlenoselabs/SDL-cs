# sdl-cs

Automatically updated C# bindings for https://github.com/libsdl-org/SDL with native dynamic link libraries.

## How to use

### NuGet packages

1. Install the `bottlenoselabs.SDL` NuGet package. This package contains only the C# bindings. To get the pre-release development packages, use the NuGet package feed: `https://www.myget.org/F/bottlenoselabs/api/v3/index.json`.
2. Install one of the following packages which contain the native library for the runtime identifier. More than one these packages can be installed at once if desired but is not strictly required and will waste disk space unnecessarily. 
   -  `bottlenoselabs.SDL.runtime.win-x64`: The `SDL.dll` native binary for Windows (64-bit).
   -  `bottlenoselabs.SDL.runtime.osx`: The `libSDL.dylib` for macOS Intel (`osx-x64`) + macOS Apple Silicon (`osx-arm64`).
   -  `bottlenoselabs.SDL.runtime.linux-x64`: The `libSDL.so` for Linux x64.

### From source

1. Download and install [.NET 6](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/bottlenoselabs/libuv-cs`.
3. Build the native library by running `bash ./library.sh`. (Building for Windows requires MSYS2 or Windows Subsystem for Linux with Ubuntu).
4. Add the C# project `./src/cs/production/SDL/SDL.csproj` to your solution.
5. Add one of the C# shim projects for the native binary in the same way as `SDL.csproj`.
   -  `SDL.win-x64`: The `SDL.dll` native binary for Windows x64.
   -  `SDL.osx`: The `libSDL.dylib` for macOS Intel (`osx-x64`) + macOS Apple Silicon (`osx-arm64`).
   -  `SDL.linux-x64`: The `libSDL.so` for Linux x64.

If you wish to re-generate the bindings, simple run `bash ./bindgen.sh`.

## Developers: Documentation

- For more information on how C# bindings work, see [`C2CS`](https://github.com/bottlenoselabs/c2cs), the tool that generates the bindings for `SDL` and other C libraries.
- To learn how to use `SDL`, check out the [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
