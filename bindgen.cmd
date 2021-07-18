@echo off
setlocal

call:download_C2CS_windows
call:bindgen_windows_64
EXIT /B %errorlevel%

:exit_if_last_command_failed
if %errorlevel% neq 0 (
    exit %errorlevel%
)
goto:eof

:download_C2CS_windows
if not exist ".\C2CS.exe" (
    powershell -Command "(New-Object Net.WebClient).DownloadFile('https://nightly.link/lithiumtoast/c2cs/workflows/build-test-deploy/develop/win-x64.zip', '%cd%\win-x64.zip')"
    "C:\Program Files\7-Zip\7z.exe" x "%cd%\win-x64.zip" -o"%cd%"
    del "%cd%\win-x64.zip"
)
goto:eof

:bindgen_windows_64
    .\C2CS ast -i .\ext\SDL\include\SDL.h -o .\ast\SDL.win64.json -s .\ext\SDL\include -b 64 -g SDL_main.h -d ^
SDL_DISABLE_MM3DNOW_H ^
SDL_DISABLE_IMMINTRIN_H ^
SDL_DISABLE_MMINTRIN_H ^
SDL_DISABLE_XMMINTRIN_H ^
SDL_DISABLE_EMMINTRIN_H ^
SDL_DISABLE_PMMINTRIN_H
    call:exit_if_last_command_failed
    .\C2CS cs -i .\ast\SDL.win64.json -o .\src\cs\production\SDL-cs\SDL.win64.cs -l "SDL2" -c "SDL" -g "SDL_bool" -a ^
 "SDL_bool -> CBool"^
 "Uint8 -> byte"^
 "Uint16 -> ushort"^
 "Uint32 -> uint"^
 "Uint64 -> ulong"^
 "Sint8 -> sbyte"^
 "Sint16 -> short"^
 "Sint32 -> int"^
 "Sint64 -> long"
    call:exit_if_last_command_failed


:bindgen_windows_32
    .\C2CS ast -i .\ext\SDL\include\SDL.h -o .\ast\SDL.win32.json -s .\ext\SDL\include -b 32 -g SDL_main.h -d ^
SDL_DISABLE_MM3DNOW_H ^
SDL_DISABLE_IMMINTRIN_H ^
SDL_DISABLE_MMINTRIN_H ^
SDL_DISABLE_XMMINTRIN_H ^
SDL_DISABLE_EMMINTRIN_H ^
SDL_DISABLE_PMMINTRIN_H
    call:exit_if_last_command_failed
    .\C2CS cs -i .\ast\SDL.win32.json -o .\src\cs\production\SDL-cs\SDL.win32.cs -l "SDL2" -c "SDL" -g "SDL_bool" -a ^
 "SDL_bool -> CBool"^
 "Uint8 -> byte"^
 "Uint16 -> ushort"^
 "Uint32 -> uint"^
 "Uint64 -> ulong"^
 "Sint8 -> sbyte"^
 "Sint16 -> short"^
 "Sint32 -> int"^
 "Sint64 -> long"
    call:exit_if_last_command_failed