## How to use
* 1 - Right click on your "Assets" folder > Create > Sprite Animation  
* 2 - Rename it as you like, add a name to the "Name" field and add/remove frames or a texture to generate it automatically, then select the desired animation type and FPS.  
* 3 - Add a Sprite Animator component to the object you wan't to run these animations.  
* 4 - Add the previously created Sprite Animations from your assets folder to the "Sprite Animations" list in the Sprite Animator component.  
* 5 - Select if you wan't to play the animation automatically (it will play the first on the list) or do it by code.  
  
## Useful coding things
### SpriteAnimator Functions
```cs
// Play the animation (use it when your code play it only once, like State-Machines do), you can pass a animation name or a SpriteAnimation and start frame, fluent interface
public SpriteAnimator Play(string name, int startFrame = 0) {}
public SpriteAnimator Play(SpriteAnimation spriteAnimation, int startFrame = 0) {}

// Play the animation only if it's not already being played (use this on Update functions), you can pass a animation name or a SpriteAnimation and start frame, fluent interface
public SpriteAnimator PlayIfNotPlaying(string name, int startFrame = 0) {}
public SpriteAnimator PlayIfNotPlaying(SpriteAnimation spriteAnimation, int startFrame = 0) {}

// Pause the animator
public void Pause() {}

// Resumes the current animation.
public void Resume() {}

// Set the current frame of the animation
public void SetCurrentFrame(int frame) {}

// Set the Action to run when a animation completes, fluent interface
public SpriteAnimator OnComplete(Action onAnimationComplete) {}
```  
  
### SpriteAnimator Getters
```cs
// Returns true if has the animation named "name"
public bool HasAnimation(string name) {}

// The default animation (index 0 on Sprite Animations Object) to be played
public SpriteAnimation DefaultAnimation;
```
  
### SpriteAnimator Events
```cs
// Called every time a animation completes for both "Looping" and "Play Once"
public event action OnAnimationComplete;
```
