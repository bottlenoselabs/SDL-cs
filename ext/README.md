# External Git submodules

Run `./build-native-libraries.sh [debug|release] [RID]` Bash script to build all the native libraries for SDL and SDL extensions.

e.g. `./build-native-libraries.sh debug osx-64`.

## SDL

SDL3 on `main` branch. Updated using Dependabot for automatic pull requests in [`.github/dependabot.yml`](../.github/dependabot.yml). Note that Dependabot respects the branch set in [`.gitmodules`](../.gitmodules).

## SDL_image

SDL3_image on `main` branch. Updated using Dependabot for automatic pull requests in [`.github/dependabot.yml`](../.github/dependabot.yml). Note that Dependabot respects the branch set in [`.gitmodules`](../.gitmodules).
