@echo off
setlocal

cmake -S .\ext\SDL -B .\cmake-build-release -A x64
cmake --build .\cmake-build-release --config Release
if not exist "%~dp0\lib" mkdir %~dp0\lib
move /y "%~dp0\cmake-build-release\Release\*.dll" "%~dp0\lib"
rmdir /s /q .\cmake-build-release
