# sdl-cs

Automatically updated C# bindings for https://github.com/libsdl-org/SDL with native dynamic link libraries.

## How to use

### From source

1. Download and install [.NET 6](https://dotnet.microsoft.com/download).
2. Fork the repository using GitHub or clone the repository manually with submodules: `git clone --recurse-submodules git@github.com:lithiumtoast/sdl-cs.git`.
3. Build the native library by running `bash ./library.sh`. (Windows requires Windows Subsystem for Linux with Ubuntu).
4. Add the C# project `./src/cs/production/SDL-cs/SDL-cs.csproj` to your solution:
```xml
<ItemGroup>
    <ProjectReference Include="path/to/sdl-cs/src/cs/production/SDL-cs/SDL-cs.csproj" />
</ItemGroup>
```

#### Bindgen

If you wish to re-generate the bindings, simple run `bash ./bindgen.sh`.

## Developers: Documentation

For more information on how C# bindings work, see [`C2CS`](https://github.com/lithiumtoast/c2cs), the tool that generates the bindings for `SDL` and other C libraries.

To learn how to use `SDL`, check out the [Lazy Foo' Production](https://lazyfoo.net/tutorials/SDL).

## License

`SDL-cs` is licensed under the MIT License (`MIT`) - see the [LICENSE file](LICENSE) for details.

`SDL` itself is licensed under ZLib (`zlib`) - see https://github.com/libsdl-org/SDL/blob/main/LICENSE.txt for more details.
