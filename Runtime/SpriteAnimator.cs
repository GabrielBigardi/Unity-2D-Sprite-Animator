using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GabrielBigardi.SpriteAnimator.Runtime
{
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private List<SpriteAnimation> _spriteAnimations = new();
        [SerializeField] private bool _playAutomatically = true;
        [SerializeField] private bool _debugMode = false;

        public SpriteAnimation DefaultAnimation => _spriteAnimations.Count > 0 ? _spriteAnimations[0] : null;
        public SpriteAnimation CurrentAnimation => SpriteAnimationHelper.CurrentAnimation;

        public bool Playing => _state == SpriteAnimationState.Playing;
        public bool Paused => _state == SpriteAnimationState.Paused;
        public int CurrentFrame => SpriteAnimationHelper.GetCurrentFrame();
        public bool IsLastFrame => CurrentFrame == CurrentAnimation.Frames.Count - 1;

        private SpriteRenderer _spriteRenderer;
        public SpriteAnimationHelper SpriteAnimationHelper { get; private set; }
        private SpriteAnimationState _state = SpriteAnimationState.Playing;
        private SpriteAnimationFrame _previousAnimationFrame;

        private bool triggerAnimationEndedEvent = false;

        public event Action SpriteChanged;
        public event Action<SpriteAnimation> AnimationPlayed;
        public event Action<SpriteAnimation> AnimationPaused;
        public event Action<SpriteAnimation> AnimationEnded;
        public event Action<string> AnimationEventCalled;

        private void Awake()
        {
            SpriteAnimationHelper = new SpriteAnimationHelper();

            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogError("No Sprite Renderers were found on the object or the child objects");
                return;
            }
        }

        private void Start()
        {
            if (_playAutomatically)
            {
                if(DefaultAnimation == null)
                {
                    Debug.LogError($"PlayAutomatically is True but no default animations were found, drag a animation into SpriteAnimations list");
                    return;
                }

                DebugModeMessage($"Playing default animation {DefaultAnimation.name}");
                Play(DefaultAnimation);
            }
        }

        private void LateUpdate()
        {
            if (!Playing || _spriteRenderer == null)
                return;

            SpriteAnimationFrame currentFrame = SpriteAnimationHelper.UpdateAnimation(Time.deltaTime);

            if (currentFrame == null)
                return;

            if (currentFrame == _previousAnimationFrame)
                return;

            if (triggerAnimationEndedEvent)
            {
                DebugModeMessage($"Calling AnimationEnded event");
                AnimationEnded?.Invoke(CurrentAnimation);
                triggerAnimationEndedEvent = false;

                if (CurrentAnimation.SpriteAnimationType != SpriteAnimationType.Looping)
                {
                    DebugModeMessage($"Reached end of non-loopable animation {CurrentAnimation.name}");
                    return;
                }
            }

            if ((CurrentFrame + 1) > (CurrentAnimation.Frames.Count - 1))
            {
                triggerAnimationEndedEvent = true;
            }

            _previousAnimationFrame = currentFrame;

            _spriteRenderer.sprite = currentFrame.Sprite;

            SpriteChanged?.Invoke();
            if (currentFrame.EventName != "")
            {
                AnimationEventCalled?.Invoke(currentFrame.EventName);
                DebugModeMessage($"Calling animation frame event {currentFrame.EventName}");
            }
        }

        public bool HasAnimation(string name)
        {
            return _spriteAnimations.Exists(a => a.Name == name);
        }

        public void PlayIfNotPlaying(string name)
        {
            if (!HasAnimation(name))
            {
                Debug.LogError($"Animation with name '{name}' not found");
                return;
            }

            PlayIfNotPlaying(GetAnimationByName(name));
        }

        public void PlayIfNotPlaying(SpriteAnimation animation)
        {
            if (CurrentAnimation.name != animation.name)
            {
                _state = SpriteAnimationState.Playing;
                SpriteAnimationHelper.ChangeAnimation(animation);
                AnimationPlayed?.Invoke(animation);

                triggerAnimationEndedEvent = false;

                DebugModeMessage($"Playing animation {animation.name}");
            }
        }

        public void Play()
        {
            if (CurrentAnimation == null)
            {
                SpriteAnimationHelper.ChangeAnimation(DefaultAnimation);
            }

            Play(CurrentAnimation);
        }

        public void Play(string name)
        {
            if (!HasAnimation(name))
            {
                Debug.LogError($"Animation with name '{name}' not found");
                return;
            }

            Play(GetAnimationByName(name));
        }

        public void Play(SpriteAnimation animation)
        {
            if (animation == null)
            {
                Debug.LogError("An null or invalid SpriteAnimation object was passed.");
                return;
            }

            _state = SpriteAnimationState.Playing;
            SpriteAnimationHelper.ChangeAnimation(animation);
            AnimationPlayed?.Invoke(animation);

            triggerAnimationEndedEvent = false;

            DebugModeMessage($"Playing animation {animation.name}");
        }

        public void Play(SpriteAnimation animation, int startFrame = 0)
        {
            if (animation == null)
            {
                Debug.LogError("An null or invalid SpriteAnimation object was passed.");
                return;
            }

            if (CurrentAnimation == null || CurrentAnimation.name != animation.name)
            {
                _state = SpriteAnimationState.Playing;
                SpriteAnimationHelper.ChangeAnimation(animation);
                AnimationPlayed?.Invoke(animation);

                triggerAnimationEndedEvent = false;
            }

            _state = SpriteAnimationState.Playing;
            SpriteAnimationHelper.SetCurrentFrame(startFrame);

            DebugModeMessage($"Playing animation {animation.name} starting on frame {startFrame}");
        }

        public void Play(string name, int startFrame = 0)
        {
            Play(GetAnimationByName(name), startFrame);
        }

        public void Pause()
        {
            DebugModeMessage($"Pausing animator");

            _state = SpriteAnimationState.Paused;
            AnimationPaused?.Invoke(CurrentAnimation);
        }

        public void Resume()
        {
            DebugModeMessage($"Resuming animator");

            _state = SpriteAnimationState.Playing;
        }

        public SpriteAnimation GetAnimationByName(string name)
        {
            for (int i = 0; i < _spriteAnimations.Count; i++)
            {
                if (_spriteAnimations[i].Name == name)
                {
                    return _spriteAnimations[i];
                }
            }

            Debug.LogError($"Can't find animation named {name}");
            return null;
        }

        private void DebugModeMessage(string message)
        {
            if (!_debugMode)
                return;

            Debug.Log(message);
        }
    }
}