using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private bool _playAutomatically = false;
        [SerializeField] private SpriteAnimationObject _spriteAnimationObject;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public SpriteAnimationObject SpriteAnimationObject => _spriteAnimationObject;

        private SpriteAnimation _currentAnimation;
        private float _animationTime = 0f;
        private bool _paused = false;
        private bool _animationCompleted = false;
        public bool AnimationCompleted => _animationCompleted;

        public bool HasAnimation(string name) => _spriteAnimationObject.SpriteAnimations.Exists(a => a.Name == name);
        public SpriteAnimation CurrentAnimation => _currentAnimation;
        public SpriteAnimation DefaultAnimation => _spriteAnimationObject.SpriteAnimations.Count > 0 ? _spriteAnimationObject.SpriteAnimations[0] : null;
        private int GetLoopingFrame() => (int)_animationTime % _currentAnimation.Frames.Count;
        private int GetPlayOnceFrame() => Mathf.Min((int)_animationTime, _currentAnimation.Frames.Count - 1);
        public int CurrentFrame => _currentAnimation.SpriteAnimationType == SpriteAnimationType.Looping ? GetLoopingFrame() : GetPlayOnceFrame();

        private Dictionary<string, Dictionary<int, List<Action>>> _currentAnimationEvents = new();

        private int _previousFrame;
        private bool _firstFrame;

        public event Action OnAnimationFrameChanged;
        public event Action OnAnimationComplete;

        private void OnEnable()
        {
            if (_playAutomatically)
            {
                if (_spriteAnimationObject.SpriteAnimations[0] == null)
                {
                    Debug.LogWarning($"PlayAutomatically is set to TRUE but no default animations were found.");
                    return;
                }

                Play(DefaultAnimation);
            }
        }

        private void LateUpdate()
        {
            if (_paused || _spriteRenderer == null)
                return;

            Sprite currentFrame = UpdateAnimation(Time.deltaTime);

            if (currentFrame == null)
                return;

            _spriteRenderer.sprite = currentFrame;

            if (_firstFrame || _previousFrame != CurrentFrame)
            {
                _firstFrame = false;
                _previousFrame = CurrentFrame;

                OnAnimationFrameChanged?.Invoke();

                if (AnimationFrameHasNoEvents(CurrentAnimation.Name, CurrentFrame))
                    return;

                foreach (var action in _currentAnimationEvents[CurrentAnimation.Name][CurrentFrame])
                    action?.Invoke();
            }
        }

        /// <summary>
        /// Play the animation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public SpriteAnimator Play(string name, int startFrame = 0)
        {
            if (_spriteAnimationObject == null)
            {
                Debug.LogWarning($"Sprite animation object is null");
                return null;
            }

            if (!HasAnimation(name))
            {
                Debug.LogWarning($"Animation with name '{name}' not found");
                return null;
            }

            Play(GetAnimationByName(name), startFrame);
            return this;
        }

        /// <summary>
        /// Play the animation
        /// </summary>
        /// <param name="spriteAnimation"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public SpriteAnimator Play(SpriteAnimation spriteAnimation, int startFrame = 0)
        {
            if (spriteAnimation == null)
            {
                Debug.LogWarning("An null or invalid SpriteAnimation object was passed.");
                return null;
            }

            Resume();
            ChangeAnimation(spriteAnimation);
            SetCurrentFrame(startFrame);
            _firstFrame = true;

            return this;
        }

        /// <summary>
        /// Play the animation only if it's not already being played
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public SpriteAnimator PlayIfNotPlaying(string name, int startFrame = 0)
        {
            if (!HasAnimation(name))
            {
                Debug.LogWarning($"Animation with name '{name}' not found");
                return null;
            }

            PlayIfNotPlaying(GetAnimationByName(name), startFrame);
            return this;
        }

        /// <summary>
        /// Play the animation only if it's not already being played
        /// </summary>
        /// <param name="spriteAnimation"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public SpriteAnimator PlayIfNotPlaying(SpriteAnimation spriteAnimation, int startFrame = 0)
        {
            if (spriteAnimation == null)
            {
                Debug.LogWarning("An null or invalid SpriteAnimation object was passed.");
                return null;
            }

            if (_currentAnimation.Name == spriteAnimation.Name)
                return null;
            Resume();
            ChangeAnimation(spriteAnimation);
            SetCurrentFrame(startFrame);
            _firstFrame = true;

            return this;
        }

        /// <summary>
        /// Pauses the animation
        /// </summary>
        public void Pause() => _paused = true;

        /// <summary>
        /// Resumes the animation
        /// </summary>
        public void Resume() => _paused = false;

        /// <summary>
        /// Sets the sprite by the frame index
        /// </summary>
        /// <param name="frame"></param>
        public void SetSpriteByFrame(int frame)
        {
            if (_currentAnimation == null)
                return;

            int clampedFrame = Math.Clamp(frame, 0, _currentAnimation.Frames.Count - 1);

            _spriteRenderer.sprite = _currentAnimation.Frames[clampedFrame];
        }

        /// <summary>
        /// Sets the animation complete callback, required to call after the animation is played
        /// </summary>
        /// <param name="onAnimationComplete"></param>
        /// <returns></returns>
        public SpriteAnimator SetOnComplete(Action onAnimationComplete)
        {
            this.OnAnimationComplete = onAnimationComplete;
            return this;
        }

        /// <summary>
        /// Sets the animation frame changed callback, required to call after the animation is played
        /// </summary>
        /// <param name="onFrameChanged"></param>
        /// <returns></returns>
        public SpriteAnimator SetOnFrameChanged(Action onFrameChanged)
        {
            this.OnAnimationFrameChanged = onFrameChanged;
            return this;
        }

        /// <summary>
        /// Sets the events for a specific animation frame, recommended to call before the animation is played to avoid missing first frame animation
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="frameEvents"></param>
        public void SetAnimationFrameEvents(string animationName, Dictionary<int, List<Action>> frameEvents)
        {
            if (!_currentAnimationEvents.ContainsKey(animationName))
                _currentAnimationEvents.Add(animationName, new Dictionary<int, List<Action>>());

            _currentAnimationEvents[animationName] = frameEvents;
        }

        /// <summary>
        /// Sets the current animation frame
        /// </summary>
        /// <param name="frame"></param>
        public void SetCurrentFrame(int frame)
        {
            if (frame < 0 || frame >= _currentAnimation.Frames.Count)
            {
                Debug.LogWarning($"Invalid frame index {frame} for animation '{_currentAnimation.Name}' with {_currentAnimation.Frames.Count} frames");
                return;
            }

            _animationTime = frame;
        }

        /// <summary>
        /// Sets the current animation time (just like SetCurrentFrame, but more accurate)
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentAnimationTime(float time) => _animationTime = time;

        /// <summary>
        /// Changes the sprite animation object
        /// </summary>
        /// <param name="spriteAnimationObject"></param>
        public void ChangeAnimationObject(SpriteAnimationObject spriteAnimationObject) => _spriteAnimationObject = spriteAnimationObject;

        private SpriteAnimation GetAnimationByName(string name)
        {
            for (int i = 0; i < _spriteAnimationObject.SpriteAnimations.Count; i++)
            {
                if (_spriteAnimationObject.SpriteAnimations[i].Name == name)
                {
                    return _spriteAnimationObject.SpriteAnimations[i];
                }
            }

            Debug.LogWarning($"Can't find animation named {name}");
            return null;
        }

        private void ChangeAnimation(SpriteAnimation spriteAnimation)
        {
            OnAnimationComplete = null;
            OnAnimationFrameChanged = null;

            _previousFrame = 0;
            _animationTime = 0f;
            _animationCompleted = false;
            _currentAnimation = spriteAnimation;
        }

        private Sprite UpdateAnimation(float deltaTime)
        {
            if (_currentAnimation != null)
            {
                if (!_animationCompleted)
                {
                    _animationTime += deltaTime * _currentAnimation.FPS;

                    var frameDuration = 1f / _currentAnimation.FPS;
                    var animationDuration = frameDuration * (_currentAnimation.Frames.Count);
                    if (_animationTime >= (animationDuration * _currentAnimation.FPS))
                    {
                        OnAnimationComplete?.Invoke();

                        _animationTime = _currentAnimation.SpriteAnimationType == SpriteAnimationType.Looping ? 0f : _currentAnimation.Frames.Count;
                        _animationCompleted = _currentAnimation.SpriteAnimationType == SpriteAnimationType.Looping ? false : true;
                    }
                }

                return GetAnimationFrame();
            }

            return null;
        }

        private Sprite GetAnimationFrame()
        {
            int currentFrame = 0;

            switch (_currentAnimation.SpriteAnimationType)
            {
                case SpriteAnimationType.Looping:
                    currentFrame = GetLoopingFrame();
                    break;
                case SpriteAnimationType.PlayOnce:
                    currentFrame = GetPlayOnceFrame();
                    break;
            }

            return _currentAnimation.Frames[currentFrame];
        }

        private bool AnimationFrameHasEvents(string animationName, int frameIndex)
        {
            return _currentAnimationEvents != null
                    && _currentAnimationEvents.Count > 0
                    && _currentAnimationEvents.ContainsKey(animationName)
                    && _currentAnimationEvents[animationName] != null
                    && _currentAnimationEvents[animationName].Count > 0
                    && _currentAnimationEvents[animationName].ContainsKey(frameIndex)
                    && _currentAnimationEvents[animationName][frameIndex] != null
                    && _currentAnimationEvents[animationName][frameIndex].Count > 0;
        }

        private bool AnimationFrameHasNoEvents(string animationName, int frameIndex)
        {
            return _currentAnimationEvents == null
                    || _currentAnimationEvents.Count == 0
                    || !_currentAnimationEvents.ContainsKey(animationName)
                    || _currentAnimationEvents[animationName] == null
                    || _currentAnimationEvents[animationName].Count == 0
                    || !_currentAnimationEvents[animationName].ContainsKey(frameIndex)
                    || _currentAnimationEvents[animationName][frameIndex] == null
                    || _currentAnimationEvents[animationName][frameIndex].Count == 0;
        }
    }
}