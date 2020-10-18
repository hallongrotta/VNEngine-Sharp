# VNEngine/SceneSaveState Sharp

![Promo Image](https://github.com/hallongrotta/VNEngine-Sharp/blob/master/promo.png)

A C# port of VNEngine and SceneSaveState (but mostly SceneSaveState).

SceneConsole window is opened with 'B' by default. Scene data stored with the scene card upon save.
Ported from VNEngine 16.0. This plugin can be used in conjunction with the Python version.

Post any bug reports to the issue tab.

## Quick Start
1. Open the studio of whichever game you want to make scenes in.
2. Press 'B' to bring up the Scene Console.
3. Go to the 'Tracking' tab.
4. In the studio, select a character, prop or light you wish to track.
5. Press 'Track selected' in Scene Console, to start tracking the object.
6. Go to 'Edit', set up your scene and press 'Add scene' to store the scene state.
8. Repeat step 6 until you are satisfied.
7. Save scene card to store your work.


## Pros and Cons Compared With Python SceneSaveState

### Pros 
* 100% less Python related bugs.
* BepInEx plugin.
* Scene data persists even if UI is closed.

### Cons
* Larger file sizes
* Less features and flexibility.
* 100% more C# related bugs.

## Attributions

VNEngine and SceneSaveState by @keitaro.
Original SceneConsole code by chickenManX.

[IllusionModdingAPI](https://github.com/IllusionMods/IllusionModdingAPI)

[ExtensibleSaveFormat](https://github.com/IllusionMods/BepisPlugins)
