#!/bin/bash
script_dir="$(dirname $0)"
echo "Started building native library... Directory: $script_dir"

cmake -S $script_dir/ext/SDL -B $script_dir/cmake-build-release
cmake --build $script_dir/cmake-build-release --config Release
mkdir -p "$script_dir/lib/"

uname_str="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$uname_str" == "linux" ]]; then
    shared_object_path="$(readlink -f $script_dir/cmake-build-release/libSDL2-2.0.so)"
    mv "$shared_object_path" "$script_dir/lib/libSDL2.so"
    echo "Moved $shared_object_path to $script_dir/lib/libSDL2.so"
elif [[ "$uname_str" == "darwin" ]]; then
    mv "$script_dir/cmake-build-release/libSDL2-2.0.dylib" "$script_dir/lib/libSDL2.dylib"
    echo "Moved $script_dir/cmake-build-release/libSDL2-2.0.dylib to $script_dir/lib/libSDL2.dylib"
fi

rm -r $script_dir/cmake-build-release
echo "Finished building native library..."