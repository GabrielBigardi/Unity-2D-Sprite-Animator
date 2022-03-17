## How to use
* 1 - Right click on your "Assets" folder > Create > Sprite Animation  
* 2 - Rename it as you like, add a name to the "Name" field and add/remove frames, select animation type and framerate.  
* 3 - Add a Sprite Animator component to the object you wan't to add these animations.  
* 4 - Add the previously created Sprite Animations from your assets folder to the "Sprite Animations" list in the Sprite Animator component.  
* 5 - Select if you wan't to play the animation automatically or do it by code.  
  
## Useful coding things
### SpriteAnimator Functions
```cs
// Play the animation (use it when your code play it only once, like State-Machines do), you can pass a animation name or a spriteanimation class
public void Play() {}
public void Play(string animationName) {}
public void Play(SpriteAnimation animation) {}

// Play the animation only if it's not already being played (use it on Update functions), you can pass a animation name or a spriteanimation class
public void PlayIfNotPlaying(string animationName) {}
public void PlayIfNotPlaying(SpriteAnimation animation) {}

// Pause the current animation
public void Pause() {}

// Resumes the current animation.
public void Resume() {}
```  
  
### SpriteAnimator Getters
```cs
// The default animation (index 0 on Sprite Animations List) to be played
public SpriteAnimation DefaultAnimation;

// The current animation being played
public SpriteAnimation CurrentAnimation;

// True if the current animation being played
public bool Playing;

// True if the current animation is paused
public bool Paused;

// Current frame being played
public int CurrentFrame;

// True if the last frame of the current animation is being played
public bool IsLastFrame;
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
