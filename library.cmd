call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\vcvars64.bat"
cmake -S .\src\c\sdl -B .\cmake-build-release -G "Visual Studio 16 2019" -DCMAKE_CONFIGURATION_TYPES=Release
devenv .\cmake-build-release\sdl.sln /build
if exist ".\lib\SDL2.exp" (
    del /q ".\lib\SDL2.exp"
)
if exist ".\lib\SDL2.lib" (
    del /q ".\lib\SDL2.lib"
)
if exist ".\lib\SDL2main.lib" (
    del /q ".\lib\SDL2main.lib"
)
if exist ".\lib\SDL2-static.lib" (
    del ".\lib\SDL2-static.lib"
)
rmdir /s /q .\cmake-build-release
