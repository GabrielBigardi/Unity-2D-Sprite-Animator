# 2D Sprite Animator
 Performatic, simple and easy animator for 2D games

## How to install
1 - Open the package manager (Window > Package Manager)
2 - Click on the plus icon and "Add package from git URL..."
3 - Enter https://github.com/GabrielBigardi/2D-Sprite-Animator.git and click "Add"
4 - Wait until the package manager finishes installing the package and recompiling

## How to use
1 - Right click on your "Assets" folder > Create > Sprite Animation
2 - Rename it as you like, add a name to the "Name" field and add/remove frames, select animation type and framerate.
3 - Add a Sprite Animator component to the object you wan't to add these animations.
4 - Add the previously created Sprite Animations from your assets folder to the "Sprite Animations" list in the Sprite Animator component.
5 - Select if you wan't to play the animation automatically or do it by code.

## Useful codes
### Functions
SpriteAnimator.Play() = Play the animation only if it's not already being played (use it on Update functions), you can pass a animation name or a spriteanimation class.
SpriteAnimator.PlayIfNotPlaying() = Play the animation (use it when your code play it only once, like State-Machines do), you can pass a animation name or a spriteanimation class.
SpriteAnimator.Pause() = Pause the current animation.
SpriteAnimator.Resume() = Resumes the current animation.

### Getters
SpriteAnimator.DefaultAnimation = Returns the default animation (index 0 on Sprite Animations List) to be played.
SpriteAnimator.CurrentAnimation = Returns the current animation being played.
SpriteAnimator.Playing = Returns true if the current animation being played.
SpriteAnimator.Paused = Returns true if the current animation is paused.
SpriteAnimator.CurrentFrame = Returns the current frame being played.
SpriteAnimator.IsLastFrame = Returns true if the last frame of the current animation is being played.

### Events
SpriteAnimator.SpriteChanged = Called every time the sprite is changed (no arguments).
SpriteAnimator.AnimationPlayed = Called every time a animation is played (args = SpriteAnimation).
SpriteAnimator.AnimationPaused = Called every time a animation is paused (args = SpriteAnimation).
SpriteAnimator.AnimationEnded = Called every time a animation is ended (args = SpriteAnimation).
SpriteAnimator.AnimationEventCalled = Called every time a Animation Event is called (args = string).

Now you can rename separator like any other GameObject and change color on inspector