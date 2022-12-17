using System.Collections.Immutable;
using C2CS;
using C2CS.Options;
using JetBrains.Annotations;

namespace SDL.Bindgen;

[UsedImplicitly]
public class WriterCSharpCode : IWriterCSharpCode
{
	public WriterCSharpCodeOptions Options { get; } = new();

	public WriterCSharpCode()
	{
		Configure(Options);
	}

	private static void Configure(WriterCSharpCodeOptions options)
	{
		options.InputAbstractSyntaxTreesFileDirectory = "./ast";

		options.OutputCSharpCodeFilePath = "../src/cs/production/SDL/SDL.cs";
		options.NamespaceName = "bottlenoselabs";
		options.LibraryName = "SDL2";
		options.MappedNames = new[]
		{
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "SDL_bool", Target = "CBool"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Uint8", Target = "byte"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Uint16", Target = "ushort"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Uint32", Target = "uint"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Uint64", Target = "ulong"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Sint8", Target = "sbyte"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Sint16", Target = "short"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Sint32", Target = "int"
			},
			new WriterCSharpCodeOptionsMappedName
			{
				Source = "Sint64", Target = "long"
			}
		}.ToImmutableArray();
		options.IgnoredNames = new[]
		{
			"MAC_OS_X_VERSION_MIN_REQUIRED",
			"SDL_bool",
			"SDL_PRIs64",
			"SDL_PRIu64",
			"SDL_PRIx64",
			"SDL_PRIX64",
			"SDL_PRIs32",
			"SDL_PRIu32",
			"SDL_PRIx32",
			"SDL_PRIX32",
			"SDL_ICONV_ERROR",
			"SDL_ICONV_E2BIG",
			"SDL_ICONV_EILSEQ",
			"SDL_ICONV_EINVAL",
			"SDL_ASSERT_LEVEL",
			"SDL_NULL_WHILE_LOOP_CONDITION",
			"SDL_LIL_ENDIAN",
			"SDL_BIG_ENDIAN",
			"SDL_BYTEORDER",
			"SDL_HAPTIC_STATUS",
			"SDL_MAX_SINT8",
			"SDL_MIN_SINT8",
			"SDL_MAX_UINT8",
			"SDL_MIN_UINT8",
			"SDL_MAX_SINT16",
			"SDL_MIN_SINT16",
			"SDL_MAX_UINT16",
			"SDL_MIN_UINT16",
			"SDL_MAX_SINT32",
			"SDL_MIN_SINT32",
			"SDL_MAX_UINT32",
			"SDL_MIN_UINT32",
			"SDL_MAX_SINT64",
			"SDL_MIN_SINT64",
			"SDL_MAX_UINT64",
			"SDL_MIN_UINT64"
		}.ToImmutableArray()!;
	}
}
