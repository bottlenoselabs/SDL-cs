# SDL3-cs

Automatically updated C# bindings for SDL and extensions on the `main` branch for v3:

-  https://github.com/libsdl-org/SDL (`main` branch)
-  https://github.com/libsdl-org/SDL_image (`main` branch)

Used primarily for internal use at `bottlenoselabs` with the following goals:

- Use the latest released .NET version: currently `.NET 9`.
- All C functions and types intended for export found in SDL3 are automatically generated using [`c2cs`](https://github.com/bottlenoselabs/c2cs). This happens via GitHub Action workflows in this repository starting from [Dependabot](https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/about-dependabot-version-updates#) to create the pull request. Minimal to zero human interaction is the goal for *writing* native interopability C# code.
- Follows [best practices](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices) for P/Invoke including using only blittable types and C# function pointers for callbacks. C# types are 1-1 to C types. This includes naming conventions. This includes enabling and using `unsafe` code in C#. However, in some cases, C# types (e.g. `CBool`, `CString`, `Span<T>`) may be perferred over raw C type equivalents in C# for performance or idiomatic reasons.
- Runtime marshalling is [disabled](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/disabled-marshalling). C# functions are 1-1 to C functions using [P/Invoke source generation](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke-source-generation). There are no overloads.

These goals might not align to your goals or your organization's goals to which I recommend looking at other similiar bindings for `SDL3` in C#:

- https://github.com/dotnet/Silk.NET
- https://github.com/flibitijibibo/SDL3-CS
- https://github.com/ppy/SDL3-CS
- https://github.com/edwardgushchin/SDL3-CS

## How to use

### From source

1. Download and install [.NET 9](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules https://github.com/bottlenoselabs/SDL3-cs`.
3. Build the native shared libraries (SDL and SDL extensions) by running [`ext/build-native-libraries.sh`](./ext/build-native-libraries.sh). To execute `.sh` (Bash) scripts on Windows, use Git Bash which can be installed with Git itself: https://git-scm.com/download/win. The `build-native-libraries.sh` script requires that CMake and SDL build dependencies ([see Linux docs for required packages](https://wiki.libsdl.org/SDL3/README/linux)) are installed and in your environment variable `PATH`.
4. Add the `src/cs/production/Interop.SDL/Interop.SDL.csproj` C# project to your solution as an existing project and then reference it within your own solution.

## Developers: Documentation

For more information on how C# bindings work, see [`c2cs`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries at `bottlenoselabs`.

To learn how to use `SDL`, check out the [official documentation](https://wiki.libsdl.org/SDL3) and [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
