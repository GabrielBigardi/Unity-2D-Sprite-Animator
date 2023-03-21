# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2023-03-21
### Added
- "Editor Coroutines" package is needed for the preview window to work properly.
- Custom editor made from scratch for a better project structure and window docking support, you can now have all object animations in a single Scriptable Object called "Sprite Animation Object".
- Sheet Generation, now you can drag a prepared (pre-cut on Unity's Sprite Editor) Texture2D to a field and auto-generate the animation, no more dragging 20 sprites one by one.

### Fixed
- The event that runs when a animation completes now works properly both with "Looping" and "Play Once" animations.

### Changed
- Reworked SpriteAnimator from scratch for better code.
- "Play Once" animations will still loop in the preview for better visualization.
- "Play" methods now returns a SpriteAnimator, this allows use of "Fluent Interfaces".

### Removed
- Assembly Definitions, i never liked it and should remove some compatibility problems.
- Events that probably would never be used by anyone.
- Scripts with low amount of code like "SpriteAnimationFrame.cs", "SpriteAnimationHelper.cs", "SpriteAnimationList.cs", "SpriteAnimationState.cs", "SpriteAnimationType.cs".
- Previous Editor scripts as it was remade from scratch.
- "Paper Doll Animator", due to not being properly tested on any project.

## [1.0.1] - 2022-03-13
### Fixed
- Fixed PlayIfNotPlaying function, it was not checking properly due to a recent change.

## [1.0.0] - 2022-12-03
### Added
- Initial commit of the project.
