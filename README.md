# sdl-cs

Automatically updated C# bindings for https://github.com/libsdl-org/SDL with native dynamic link libraries. C# bindings are *exactly* how they are found in the C headers.

## How to use

### NuGet package

1. Create a new `nuget.config` file if you don't have one already. Put it beside your `.sln`. If you don't have a `.sln` put it beside your `.csproj`.

```bash
dotnet new nuget
```

2. Add the following NuGet package source to the file.

```xml
<add key="lithiumtoast" value="https://www.myget.org/F/lithiumtoast/api/v3/index.json" />
```

3. Install the package for your runtime identifier.

Windows: `SDL-cs.win-x64`

macOS: `SDL-cs.osx-x64` 

Linux: `SDL-cs.linux-x64` 

For more information on runtime identifiers see the [RID catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids). Runtime identifiers currently made available for NuGet packages for this project include: `win-x64`, `osx-x64`, `linux-x64`. If you need an other runtime identifier create an issue and I'll set it up.

## Building from Source

1. Download and install [.NET 5](https://dotnet.microsoft.com/download).
2. Clone the repository with submodules: `git clone --recurse-submodules git@github.com:lithiumtoast/sdl-cs.git`.
3. Build the native library by running `./library.sh` on macOS or Linux and `.\library.sh` on Windows.
3. If using IDE (Visual Studio / Rider): Open `SDL.sln` and build solution.
4. If using CLI: `dotnet build`

Additionally, if at any time you wish to re-generate the bindings, simple run `./bindgen.sh` on macOS or Linux and `.\bindgen.cmd` on Windows.

## Developers: Documentation

For more information on how C# bindings work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries.

To learn how to use `SDL`, check out the [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
