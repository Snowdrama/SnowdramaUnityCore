# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),

## [Unreleased]
## [0.6.7] - 2026-04-28
- Updated ControlSchemeManager to add input references for KBM/Gamepad/Touch inputs
- Finishing and overhauling the WindowSettingsManager
- Added WindowResolution and WindowFullscreen options tools.
- Added UI Horizontal/Vertical Flex tool for non-uniform sized layouts
- Fix to PauseMenuManager to fix cancel button also pausing
- Fix to NewGame not actually setting the loaded save to the GameDataManager.
- NewGame now has an option to auto load the scene listed in the DefaultSave.jsonc once it loads it

## [0.6.6] - 2026-04-28
- BIG overhaul to save game!
    - Removed need for wrapper scenes entirely
    - Save/GameData Managers are now globally bootstrapped fixing issues with launch from editor problems
    - SaveManager still loads save 0 if it exists on start even when launching from Editor
- Changed the SceneController Default SceneLayoutJSON to match with the removal of need for Save/GameData Manager
- Fixed a bunch of issues with the save/load modals.

## [0.6.5] - 2026-04-28
- Added Color and Color[] to the GameData/GameDataManager

## [0.6.4] - 2026-04-28
- Make the SaveGameManager.NewGame function static so it can be called from other scripts
- Added SaveManager.LoadSaveScene to trigger SceneController.GoToScene on the scene that was listed in the save data
- SaveManager no longer automatically sets the current scene when saving, to allow another scene to save before loading the next scene

## [0.6.3] - 2026-04-27
- Fix to the SceneController again to fix issues with weird loading behavior when loading a scene more than once. 

## [0.6.2] - 2026-04-26
- Exposed static IsPaused bool in the PauseManager
- Added format to the loading screen text so you can append the tooltip text to a format like "Loading: \[Replace\]"
- Made the main content of the UIRoute into a list, so you can open multiple things when a UI opens(like a background texture)

## [0.6.1] - 2026-04-25
- Added SmoothStep and MoveTowards variants for Color
- Fix to scene controller to work correctly when the target scene is a sub-string of scene like going from "GameLevel1" to "Game" would break

## [0.6.0] - 2026-04-22
- Added new collection StackList, a Stack that uses a List internally so you can do things like Remove and RemoveAt
- Changes to router to use StackList and now when unregistering a route we remove remove it from the Route Stack to ensure it can't be opened if say it closes, destroys and then can't be navigated back to.

## [0.5.9] - 2026-04-21
- Moved SplashScreen and SplashScreenImage to GameFeatures Folder
- Reworked SplashScreen and SplashScreenImage slightly, removed silly editor tooling simplified code
- Moved ControlSchemeManager to Input Folder
- Moved PauseManager and PauseMenuController to Pause Folder
- Fixed PauseMenuController to stop request unpausing and close all pause menu routes when disabling
- Added RandomSprite to SimpleTools/2DHelpers
- Moved HideInWebGL to SimpleTools Folder
- Moved SelectFirstSelectable to UI Folder

## [0.5.8] - 2026-04-20
- Added LayerMask helper ToLayerInt

## [0.5.7] - 2026-04-18
- Removed old dialog system

## [0.5.6] - 2026-04-18
- Added Physics tools for calculating gravity/jump force
- Added warning to music manager if there's no music

## [0.5.5] - 2026-04-09
- Change to List Extensions to use RandomAndNoise which uses System.Random instead of UnityEngine.Random to allow things like Shuffle/MutateSelf to work outside of things like Start/Awake

## [0.5.4] - 2026-04-08
- UIRoute now remembers the last selection until closed
- UIRouter now uses private properties and exposes a getter for the current stack
- UIRouter now has a counter of how many routes are open and a getter for it, this is better instead of calling count on the stack every time.

## [0.5.3] - 2026-04-07
- Fix to Transition driver to call OnTransitionStart/Complete used by some sample transitions like the Text one
- Add WindowResolutionManager to set window stuff
- Added random option to LoadingScreenTextObject.
- Changed Scene controller to use unscaledDeltaTime in case the game is paused

## [0.5.2] - 2026-04-07
- Added/Fixed Vector3/Int Extension stuff
- Added MoveUp/Down functions to ListExtensions

## [0.5.1] - 2026-03-04
- Added QuaternionExtensions RandomRotation
- Added Vector3Extension RandomDirection
- Added Line Gizmo Simple Tool
- Added Arrow Gizmo Simple Tool
- Added DistributeInGrid3D Simple Tool
- Added Vector3Extension RandomDirection

## [0.5.0] - 2026-03-01
### Changed
- Added ability to get single save info
- Made it so it doesn't error if mainContent of a UI Route isn't set
- Fix for dumb infinite recursive mistake in DebugLog thing in Scene Controller
- Fixed some save modal stuff
- Fix to Spring 3D to use Z and not Y
- Added Exit Game option to UI Router as a shortcut
- Overhaul to Options system

## [0.4.9] - 2026-03-01
### Changed
- Changed GameData to allow serializing and deserializing Structs.

## [0.4.8] - 2026-02-28
### Changed
- Updated loot table to add more features

## [0.4.7] - 2026-02-25
### Changed
- Fix to not SerializeField a field in Timer

## [0.4.6] - 2026-02-24
### Changed
- Added some function documentation for Extensions, and other tools.

## [0.4.5] - 2025-12-26
### Changed
- Fixed new games not actually loading the default save into the loadedSaveSlot

## [0.4.4] - 2025-12-26
### Changed
- Added "Snowdrama" menu at the top
- Added a "JSONC importer" so .jsonc files show up as text assets.
- Added helper to Snowdrama menu to open things like Data Path and Console Log
- Added 'Required' menu for creating required things like the Default Save and SceneLayoutJson
- Save tools now require a DefaultSave.jsonc to be present, this lets you have a default set of data for new games. Calling SaveManager.NewGame() will create new data from the default data and set that that as the currently loaded save.

## [0.4.3] - 2025-12-07
### Changed
- Fixed some unused usings causing builds to not work

## [0.4.2] - 2025-12-07
### Changed
- Main fix: Can no longer start a transition until the transition is finished
- Lots of changes, I was forgetting to do this for a while but we're going to start...

## [0.2.0] - 2024-04-01
### Changed
- Combined all the packages back into this package after being split off in 2020-2024
### Added
- Transitions
- Springs
- Timers and Iterators
- Signals
- Options and Settings
- Generic LootTable implementation
- UI Components and UI Router tools
- GameData object and save/load system
- Other Things... 

## [0.1.7] - 2019-11-15
### Added
- Added the RequiredInterfaceAttribute class and editor


## [0.1.6] - 2019-11-15
### Added
- Added in all the tools, previously unlogged changes so this is the "first" release
