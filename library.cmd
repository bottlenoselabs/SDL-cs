@echo off
setlocal

cmake -S .\src\c\sdl -B .\cmake-build-release -A x64
cmake --build .\cmake-build-release --config Release
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
