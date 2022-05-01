# AvatarLib

Contains a bunch of quick-to-use APIs for plugins and applications to add, modify a VRChat avatar. This is created because one or more tools of mine share the same code.

## API Documentation

Documentation is available at https://poi-vrc.github.io/AvatarLib

## How to install

AvatarLib uses the Unity Package Manager (UPM) for version control. You can install the library by:

1. Start the Package Manager from `Window -> Package Manager`

2. Press the `+` icon and `Add package from git URL...`

3. Enter the URL: `https://github.com/poi-vrc/AvatarLib.git?path=/Packages/com.chocopoi.vrc.avatarlib`

  - This URL tracks the latest `master` branch version
  - To change to a specific version, you can append `#1.x.x` at the end
    - e.g. `https://github.com/poi-vrc/AvatarLib.git?path=/Packages/com.chocopoi.vrc.avatarlib#1.0.0`
  - For details, check https://docs.unity3d.com/Manual/upm-git.html

## How to update

1. Start the Package Manager from `Window -> Package Manager`

2. Next to the `+` icon, change to `In Project`

3. Find `AvatarLib` and press the `Update to x.x.x` button if available.

## Contributing

Please use `dotnet-format` to format your code before committing. The `master` branch is the branch holding the latest stable version. Only submit code to `develop` branch. And it will be merged to `master` soon if it is time.

To install `dotnet-format`, you can use: `dotnet tool install -g dotnet-format`

To format code, do `dotnet-format "Packages\com.chocopoi.vrc.avatarlib" --folder` in the repository root.

## License

This project is licensed under the LGPLv3 License. [tl;dr](https://tldrlegal.com/license/gnu-lesser-general-public-license-v3-(lgpl-3))