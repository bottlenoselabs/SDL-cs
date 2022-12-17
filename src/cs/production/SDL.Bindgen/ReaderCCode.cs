using System.Collections.Immutable;
using C2CS;
using C2CS.Options;
using JetBrains.Annotations;

namespace SDL.Bindgen;

[UsedImplicitly]
public class ReaderCCode : IReaderCCode
{
    public ReaderCCodeOptions Options { get; } = new();

    public ReaderCCode()
    {
        Configure(Options);
    }

    private static void Configure(ReaderCCodeOptions options)
    {
        options.InputHeaderFilePath =
            "../src/c/production/SDL/include/SDL_pinvoke.h";
        options.UserIncludeDirectories = new[] { "../ext/SDL/include" }.ToImmutableArray();
        options.OutputAbstractSyntaxTreesFileDirectory =
            "./ast";

        ConfigurePlatforms(options);
    }

    private static void ConfigurePlatforms(ReaderCCodeOptions options)
    {
	    options.HeaderFilesBlocked = new[] { "SDL_thread.h", "SDL_stdinc.h" }.ToImmutableArray();
	    options.OpaqueTypeNames = new[] { "SDL_AudioCVT", "SDL_RWops" }.ToImmutableArray();
	    options.IsEnabledEnumsDangling = true;
	    options.PassThroughTypeNames = new[]
	    {
		    "SDL_bool", "Uint8", "Uint16", "Uint32", "Uint64", "Sint8", "Sint16", "Sint32", "Sint64"
	    }.ToImmutableArray();

	    var platforms = new Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform>();

        var hostOperatingSystem = Native.OperatingSystem;
        switch (hostOperatingSystem)
        {
            case NativeOperatingSystem.Windows:
                ConfigureHostOsWindows(options, platforms);
                break;
            case NativeOperatingSystem.macOS:
                ConfigureHostOsMac(options, platforms);
                break;
            case NativeOperatingSystem.Linux:
                ConfigureHostOsLinux(options, platforms);
                break;
            default:
                throw new NotImplementedException();
        }

        options.Platforms = platforms.ToImmutableDictionary();
    }

    private static void ConfigureHostOsWindows(ReaderCCodeOptions options,
        Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform> platforms)
    {
        platforms.Add(TargetPlatform.aarch64_pc_windows_msvc, new ReaderCCodeOptionsPlatform());
        platforms.Add(TargetPlatform.x86_64_pc_windows_msvc, new ReaderCCodeOptionsPlatform());
    }

    private static void ConfigureHostOsMac(ReaderCCodeOptions options,
        Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform> platforms)
    {
        platforms.Add(TargetPlatform.aarch64_apple_darwin, new ReaderCCodeOptionsPlatform());
        platforms.Add(TargetPlatform.x86_64_apple_darwin, new ReaderCCodeOptionsPlatform());
    }

    private static void ConfigureHostOsLinux(ReaderCCodeOptions options,
        Dictionary<TargetPlatform, ReaderCCodeOptionsPlatform> platforms)
    {
        platforms.Add(TargetPlatform.aarch64_unknown_linux_gnu, new ReaderCCodeOptionsPlatform());
        platforms.Add(TargetPlatform.x86_64_unknown_linux_gnu, new ReaderCCodeOptionsPlatform());
    }
}
