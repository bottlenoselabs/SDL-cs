# SDL3-cs

Automatically updated C# bindings for SDL and extensions on the `main` branch for v3:

-  https://github.com/libsdl-org/SDL (`main` branch)
-  https://github.com/libsdl-org/SDL_image (`main` branch)

Development is driven primarily for internal use at `bottlenoselabs` with the following goals. Pull requests are welcome as long as they match the following goals of `bottlenoselabs`.

- (1) Support open-source, developer freedom, and legal simplicity/flexibility: use the permissive  `MIT` license where ever possible.
- (2) Use the latest released .NET version: currently `.NET 9`. Please see https://github.com/dotnet/core/blob/main/release-notes/9.0/supported-os.md for limitations of supported operating systems, versions, and CPU architectures with `.NET 9`.
  - Support for Windows, macOS, and Ubuntu Linux as first class meaning they are actively tested *during* development using the *latest* operating system version. This is based on limited in-house physical hardware and [GitHub's Action runner images](https://github.com/actions/runner-images) which are [free for `standard` (latest) images in public repositories](https://docs.github.com/en/billing/managing-billing-for-your-products/managing-billing-for-github-actions/.about-billing-for-github-actions).
  - Support for other systems are tested *when* and *where* hardware and/or development licenses/kits are available by individuals.
    - iOS not yet supported. See https://github.com/bottlenoselabs/SDL3-cs/issues/547.
    - Android not yet supported. See https://github.com/bottlenoselabs/SDL3-cs/issues/548.
    - Browser (WebAssembly) not yet supported. Dependant upon `SDL_GPU` being available. See https://github.com/libsdl-org/SDL/pull/12046.
    - Consoles not yet supported. For primary support on getting `SDL` in C# running on consoles please refer to [`FNA-XNA`](`https://fna-xna.github.io`) and specifically the documentation of [`FNA on consoles`](https://fna-xna.github.io/docs/appendix/Appendix-B%3A-FNA-on-Consoles/#general-advice). If the `FNA-XNA` team helped you in anyway please consider [donating to their cause](https://github.com/sponsors/flibitijibibo).
- (3) Continuous delivery. All C functions and types intended for export found in SDL3 are automatically generated using [`c2cs`](https://github.com/bottlenoselabs/c2cs). This happens via GitHub Action workflows in this repository starting from [Dependabot](https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/about-dependabot-version-updates#) to create the pull request daily. Minimal to zero human interaction is the goal for *writing* (generating) the native interopability C# code while human interaction is required for *reviewing* (reading) the code.
  - If you need a specific released version of SDL3, please see https://github.com/bottlenoselabs/SDL3-cs/issues/549.
- (4) Follow P/Invoke [best practices](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices) including using only blittable types and C# function pointers for callbacks. C# types are 1-1 to C types. This includes naming conventions. This includes enabling and using `unsafe` code in C#. However, in some cases, C# types (e.g. `CBool`, `CString`, `Span<T>`) may be perferred over raw C type equivalents in C# for performance or idiomatic reasons.
- (5) Runtime marshalling is [disabled](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/disabled-marshalling). C# functions are 1-1 to C functions using [P/Invoke source generation](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke-source-generation). There are no overloads.

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
4. Run an example suite suite to test things are working. Use '1' and '2' on your keyboard to move between examples in the suite once run.
   - `SDL_GPU`: `dotnet run --project ./src/cs/examples/SDL_GPU/SDL_GPU.csproj`
   - `LazyFoo`: `dotnet run --project ./src/cs/examples/LazyFoo/LazyFoo.csproj`
5. Add the following C# projects to your solution and reference them in one of your C# projects:
    - `src/cs/production/Interop.SDL/Interop.SDL.csproj`
    - `src/cs/production/Interop.SDL_image/Interop.SDL_image.csproj`
6. For each C# project referenced, call the static initialize method at the start of your application.
    - `Interop.SDL.Initialize()`
    - `Interop.SDL_Image.Initialize()` 

## Developers: Documentation

For more information on how C# bindings work, see [`c2cs`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries at `bottlenoselabs`.

To learn how to use `SDL`, check out the [official documentation](https://wiki.libsdl.org/SDL3) and [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
