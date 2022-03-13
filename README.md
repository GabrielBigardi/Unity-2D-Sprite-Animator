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
### SpriteAnimator Functions
```cs
// Play the animation (use it when your code play it only once, like State-Machines do), you can pass a animation name or a spriteanimation class
SpriteAnimator.Play()
SpriteAnimator.Play(string animationName)
SpriteAnimator.Play(SpriteAnimation animation)

// Play the animation only if it's not already being played (use it on Update functions), you can pass a animation name or a spriteanimation class
SpriteAnimator.PlayIfNotPlaying()
SpriteAnimator.PlayIfNotPlaying(string animationName)
SpriteAnimator.PlayIfNotPlaying(SpriteAnimation animation)

// Pause the current animation
SpriteAnimator.Pause()

// Resumes the current animation.
SpriteAnimator.Resume() 
```  
  
### SpriteAnimator Getters
```cs
// The default animation (index 0 on Sprite Animations List) to be played
public SpriteAnimation DefaultAnimation

// The current animation being played
public SpriteAnimation CurrentAnimation

// True if the current animation being played
public bool Playing

// True if the current animation is paused
public bool Paused

// Current frame being played
public int CurrentFrame

// True if the last frame of the current animation is being played
public bool IsLastFrame 
```
  
### SpriteAnimator Events
```cs
// Called every time the sprite is changed
public event Action SpriteChanged;

// Called every time a animation is played
public event Action<SpriteAnimation> AnimationPlayed;

// Called every time a animation is paused
public event Action<SpriteAnimation> AnimationPaused;

// Called every time a animation is ended
public event Action<SpriteAnimation> AnimationEnded;

// Called every time a Animation Event is called
public event Action<string> AnimationEventCalled;
```