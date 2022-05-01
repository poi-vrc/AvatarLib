# AvatarLib

Contains a bunch of quick-to-use APIs for plugins and applications to add, modify a VRChat avatar. This is created because one or more tools of mine share the same code.

## API Documentation

Documentation is available at https://poi-vrc.github.io/AvatarLib

## How to install

AvatarLib uses the Unity Package Manager (UPM) for version control. You can install the library by:

1. Start the Package Manager from `Window -> Package Manager`

2. Press the `+` icon and `Add package from git URL...`

3. Enter the URL: `https://github.com/poi-vrc/AvatarLib.git`

  - This URL tracks the latest `master` branch version
  - To change to a specific version, you can append `#1.x.x` at the end
    - e.g. `https://github.com/poi-vrc/AvatarLib.git#1.0.0`
  - For details, check https://docs.unity3d.com/Manual/upm-git.html

## How to update

1. Start the Package Manager from `Window -> Package Manager`

2. Next to the `+` icon, change to `In Project`

3. Find `AvatarLib` and press the `Update to x.x.x` button if available.

## Why this library?

It is created to simply allow me to re-use some codes in multiple tools. But it is a really good library with handy library functions:

1. Easier `VRCExpressionsMenu` generation

With plain VRCSDK code, you will need to:

```csharp
...
VRCExpressionsMenu menu = new VRCExpressionsMenu(); 
menu.controls.Add(new VRCExpressionsMenu.Control()
{
    name = "MyButton",
    type = VRCExpressionsMenu.Control.ControlType.Button,
    parameter = "this_is_a_parameter_1",
    value = 1.0f
});
menu.controls.Add(new VRCExpressionsMenu.Control()
{
    name = "MyToggle",
    type = VRCExpressionsMenu.Control.ControlType.Toggle,
    parameter = "this_is_a_parameter_2",
    value = 1.0f
});
VRCExpressionsMenu subMenuHere = new VRCExpressionsMenu(); 
menu.controls.Add(new VRCExpressionsMenu.Control()
{
    name = "MyAnotherSubMenu",
    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
    subMenu = subMenuHere
});
AssetDatabase.CreateAsset(menu, "Assets/Menu1.asset");
AssetDatabase.CreateAsset(subMenuHere, "Assets/Menu2.asset");
...
```

...which is very long. But you can do this easier with AvatarLib:

```csharp
// you can create a ExpressionMenuBuilder or directly create a sub-menu the avatar root menu
VRCExpressionsMenu myMenu = new ExpressionMenuBuilder()
    .AddButton("MyButton", "this_is_a_parameter_1", 1.0f)
    .CreateAsset("Assets/Menu1.asset")
    .GetMenu();

// or..
ExpressionMenuUtils.CreateSubMenu(avatar.rootMenu, "MySubMenu")
    .AddButton("MyButton", "this_is_a_parameter_1", 1.0f)
    .AddToggle("MyToggle", "this_is_a_parameter_2", 1.0f)
    .BeginNewSubMenu("MyAnotherSubMenu")
        .AddButton("SubMenuButtonHere", "this_is_a_parameter_3", 1.0f)
        .CreateAsset("Assets/Menu2.asset")
    .EndNewSubMenu()
    .CreateAsset("Assets/Menu1.asset");
```

2. Clean up your existing expression menus and animator stuff easier using Regex

```csharp
// removes controls with name starting with "MyMenu "
ExpressionMenuUtils.RemoveExpressionMenuControls(menu, "^MyMenu ");

// removes parameters with name starting with "MyParameter_"
ExpressionMenuUtils.RemoveExpressionParameters(parameters, "^MyParameter_");

// removes layers with name starting with "MyLayer_"
AnimationUtils.RemoveAnimatorLayers(controller, "^MyLayer_");

// removes parameters with name starting with "MyParameter_"
AnimationUtils.RemoveAnimatorParameters(controller, "^MyParameter_");
```

3. Quick create animator layers with one-line of code

```csharp
// Creates a AnyState--{condition}-->State layer!
//
//    [Entry] --------------------|
//                                v
//    [AnyState] --{if 0}-> [ myDefaultMotion ]
//         |
//          -------{if 1}-> [ motion1 ]
//         |
//          -------{if 2}-> [ motion2 ]
//         |
//          -------{if 3}-> [ motion3 ]

Dictionary<int, Motion> pairs = new Dictionary<int, Motion>();

pairs.Add(0, myDefaultMotion);
pairs.Add(1, motion1);
pairs.Add(2, motion2);
pairs.Add(3, motion3);

AnimationUtils.GenerateAnyStateLayer(controller, "MyLayer", "MyParameter_Integer", pairs, true);
```

4. Create World Constraint with code

```csharp
Transform root = avatar.transform;

WorldConstraint wc = WorldConstraintUtils.CreateWorldConstraint(myGameObjectToBeInWorldSpace, root);

// animation clips generation

AnimationClip lockClip = WorldConstraintUtils.GenerateLockAnimation(wc);
AnimationClip unlockClip = WorldConstraintUtils.GenerateUnlockAnimation(wc);
AnimationClip showClip = WorldConstraintUtils.GenerateShowAnimation(wc);
AnimationClip hideClip = WorldConstraintUtils.GenerateHideAnimation(wc);
```

## Contributing

Please use `dotnet-format` to format your code before committing. The `master` branch is the branch holding the latest stable version. Only submit code to `develop` branch. And it will be merged to `master` soon if it is time.

To install `dotnet-format`, you can use: `dotnet tool install -g dotnet-format`

To format code, do `dotnet-format ".\" --folder` in the repository root.

## License

This project is licensed under the LGPLv3 License. [tl;dr](https://tldrlegal.com/license/gnu-lesser-general-public-license-v3-(lgpl-3))