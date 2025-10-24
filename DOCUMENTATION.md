## How to use
* 1 - Right click on your "Assets" folder > Create > Sprite Animation  
* 2 - Rename it as you like, add a name to the "Name" field and add/remove frames or a texture to generate it automatically, then select the desired animation type and FPS.  
* 3 - Add a Sprite Animator component to the object you wan't to run these animations.  
* 4 - Add the previously created Sprite Animations from your assets folder to the "Sprite Animations" list in the Sprite Animator component.  
* 5 - Select if you wan't to play the animation automatically (it will play the first on the list) or do it by code.  
  
## Useful coding things
### SpriteAnimator Functions and Variables
```cs
// The Current SpriteAnimationObject being used
public SpriteAnimationObject SpriteAnimationObject

// If the animation is completed or not
public bool AnimationCompleted

// Check if the current SpriteAnimationObject has a SpriteAnimation with that name
public bool HasAnimation(string name)

// The current SpriteAnimation being played
public SpriteAnimation CurrentAnimation

// The default animation for this SpriteAnimator (0 or null)
public SpriteAnimation DefaultAnimation

// The current frame of the SpriteAnimation being played
public int CurrentFrame

// Event called when the frame is changed
public event Action OnAnimationFrameChanged;

// Event called when the animation is completed
public event Action OnAnimationComplete;

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

// Set the current sprite based on that frame
public void SetSpriteByFrame(int frame)

// Sets the animation complete callback, required to be called after the animation is played
public SpriteAnimator SetOnComplete(Action onAnimationComplete)

// Sets the animation frame changed callback, required to call after the animation is played
public SpriteAnimator SetOnFrameChanged(Action onFrameChanged)

// Sets the events for a specific animation frame, recommended to be called before the animation is played to avoid missing first frame animation
public void SetAnimationFrameEvents(string animationName, Dictionary<int, List<Action>> frameEvents)

// Sets the current animation frame
public void SetCurrentFrame(int frame)

// Sets the current animation time (just like SetCurrentFrame, but more accurate)
public void SetCurrentAnimationTime(float time) => _animationTime = time;

// Changes the sprite animation object
public void ChangeAnimationObject(SpriteAnimationObject spriteAnimationObject) => _spriteAnimationObject = spriteAnimationObject;
```  
