# External Git submodules

Run `./build-native-library.sh [debug|release] [RID]` Bash script to build all the native libraries that are external Git submodules **locally**.

e.g. `./build-native-library.sh debug osx-64`.

If you omit the first parameter `[debug|release]`, `release` is used as the default.
If you omit the second parameter `[RID]` is the same as host operating system.

## SDL

SDL3 on `main` branch. Updated using Dependabot for automatic pull requests in [`.github/dependabot.yml`](../.github/dependabot.yml). Note that Dependabot respects the branch set in [`.gitmodules`](../.gitmodules).
