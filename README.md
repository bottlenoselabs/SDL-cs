# SDL-cs

Automatically updated C# bindings for `https://github.com/libsdl-org/SDL`, on the `main` branch for v3, with native dynamic link libraries. For v2 of SDL, please see `https://github.com/bottlenoselabs/SDL2-cs`.

## How to use

### From source

1. Download and install [.NET 7](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/bottlenoselabs/SDL-cs`.
3. Build the native library by running `library.sh`. To execute `.sh` scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `library.sh` script requires that CMake and Ninja is installed and in your path.
4. Add the `src/cs/production/SDL/SDL.csproj` C# project to your solution as an existing project and then reference it within your own solution.

## Developers: Documentation

For more information on how C# bindings work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries.

To learn how to use `SDL`, check out the [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
