# Contributing

### Installation Guide

This mod is a standard C# RimWorld mod, meaning once you clone the repository into your local mods folder, all you need to do is open the [solution file](../1.4/Source/Simple%20Custom%20Gas%20Framework/Simple%20Custom%20Gas%20Framework.sln) using Visual Studio, add all the required references, and then build the project.

For more information, see the [RimWorld modding tutorials](https://rimworldwiki.com/wiki/Modding_Tutorials/Setting_up_a_solution#Option_1_.28Manual_Method.29:).

### References

- `Assembly-CSharp.dll` - RimWorld's main assembly.
- `UnityEngine.CoreModule.dll` - Unity's core assembly.
- `0Harmony.dll` - Harmony library for patching code.
- `UnityEngine.IMGUIModule.dll` - For patching the UI inspector so we can display custom gases.
