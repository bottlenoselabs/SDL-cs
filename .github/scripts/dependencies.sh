#!/bin/bash

unamestr="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$unamestr" == "linux" ]]; then
    sudo apt-get update -y
    sudo apt-get install wayland-protocols \ 
        pkg-config \
        ninja-build \
        libasound2-dev \
        libdbus-1-dev \
        libegl1-mesa-dev \
        libgl1-mesa-dev \
        libgles2-mesa-dev \
        libglu1-mesa-dev \
        libibus-1.0-dev \
        libpulse-dev \
        libsdl2-2.0-0 \
        libsndio-dev \
        libudev-dev \
        libwayland-dev \
        libwayland-client++0 \
        wayland-scanner++ \
        libwayland-cursor++0 \
        libx11-dev \
        libxcursor-dev \
        libxext-dev \
        libxi-dev \
        libxinerama-dev \
        libxkbcommon-dev \
        libxrandr-dev \
        libxss-dev \
        libxt-dev \
        libxv-dev \
        libxxf86vm-dev \
        libdrm-dev \
        libgbm-dev\
        libpulse-dev
    cmake -S ./src/c/sdl -B ./cmake-build-release -G 'Ninja'
else
    echo "Unknown platform: '$unamestr'."
    exit 1
fi
