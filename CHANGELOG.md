# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),

## [Unreleased]
## [0.4.6] - 2026-02-24
- Added some function documentation for Extensions, and other tools.

## [0.4.5] - 2025-12-26
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
