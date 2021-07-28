#!/usr/bin/env pwsh

$file='.\ext\SDL\include\SDL_version.h'

$regex_version_major='^#define\s+SDL_MAJOR_VERSION\s+([0-9]+)\s*$'
$regex_version_minor='^#define\s+SDL_MINOR_VERSION\s+([0-9]+)\s*$'
$regex_version_patch='^#define\s+SDL_PATCHLEVEL\s+([0-9]+)\s*$'

$version_major = select-string -Path $file -Pattern $regex_version_major -AllMatches | Select-Object -ExpandProperty Matches -First 1 | % { $_.Groups[1].Value }
$version_minor = select-string -Path $file -Pattern $regex_version_minor -AllMatches | Select-Object -ExpandProperty Matches -First 1 | % { $_.Groups[1].Value }
$version_patch = select-string -Path $file -Pattern $regex_version_patch -AllMatches | Select-Object -ExpandProperty Matches -First 1 | % { $_.Groups[1].Value }

echo "$version_major.$version_minor.$version_patch"