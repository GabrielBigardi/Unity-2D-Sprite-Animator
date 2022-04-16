using UnityEngine;
using System.Collections.Generic;
using System;

namespace GabrielBigardi.SpriteAnimator.Runtime
{
    public class PaperDollSpriteAnimator : MonoBehaviour
    {
        [SerializeField] public int _spriteAnimationsIndex = 0;
        [SerializeField] private List<SpriteAnimationList> _spriteAnimations = new List<SpriteAnimationList>();
        [SerializeField] private bool _playAutomatically = true;

        public SpriteAnimation DefaultAnimation => _spriteAnimations[_spriteAnimationsIndex].spriteAnimations.Count > 0 ? _spriteAnimations[_spriteAnimationsIndex].spriteAnimations[0] : null;
        public SpriteAnimation CurrentAnimation => _spriteAnimationHelper.CurrentAnimation;

        public bool Playing => _state == SpriteAnimationState.Playing;
        public bool Paused => _state == SpriteAnimationState.Paused;
        public int CurrentFrame => _spriteAnimationHelper.GetCurrentFrame();
        public bool IsLastFrame => CurrentFrame == CurrentAnimation.Frames.Count - 1;
        public float AnimationDurationMilisseconds => (1000f / CurrentAnimation.FPS) * CurrentAnimation.Frames.Count;
        public float AnimationDurationSeconds => (1f / CurrentAnimation.FPS) * CurrentAnimation.Frames.Count;

        private SpriteRenderer _spriteRenderer;
        private SpriteAnimationHelper _spriteAnimationHelper;
        private SpriteAnimationState _state = SpriteAnimationState.Playing;
        private SpriteAnimationFrame _previousAnimationFrame;
        private int _previousSpriteAnimationsIndex = 0;

        private bool triggerAnimationEndedEvent = false;

        public event Action SpriteChanged;
        public event Action<SpriteAnimation> AnimationPlayed;
        public event Action<SpriteAnimation> AnimationPaused;
        public event Action<SpriteAnimation> AnimationEnded;
        public event Action<string> AnimationEventCalled;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            _spriteAnimationHelper = new SpriteAnimationHelper();
        }

        private void Start()
        {
            if (_playAutomatically)
            {
                Play(DefaultAnimation);
            }
        }

        private void LateUpdate()
        {
            if (Playing)
            {
                if (_previousSpriteAnimationsIndex != _spriteAnimationsIndex)
                {
                    RefreshSpriteAnimations(_spriteAnimationsIndex);
                }

                SpriteAnimationFrame currentFrame = _spriteAnimationHelper.UpdateAnimation(Time.deltaTime);

                if (currentFrame != null)
                {
                    if (currentFrame != _previousAnimationFrame)
                    {
                        if (triggerAnimationEndedEvent)
                        {
                            AnimationEnded?.Invoke(CurrentAnimation);
                            triggerAnimationEndedEvent = false;

                            if (CurrentAnimation.SpriteAnimationType != SpriteAnimationType.Looping) return;
                        }

                        if ((CurrentFrame + 1) > (CurrentAnimation.Frames.Count - 1))
                        {
                            triggerAnimationEndedEvent = true;
                        }

                        _previousAnimationFrame = currentFrame;

                        _spriteRenderer.sprite = currentFrame.Sprite;

                        SpriteChanged?.Invoke();
                        if (currentFrame.EventName != "") AnimationEventCalled?.Invoke(currentFrame.EventName);
                    }
                }
            }
        }

        public void PlayIfNotPlaying(string name)
        {
            PlayIfNotPlaying(GetAnimationByName(name));
        }

        public void PlayIfNotPlaying(SpriteAnimation animation)
        {
            if (CurrentAnimation.name != animation.name)
            {
                _state = SpriteAnimationState.Playing;
                _spriteAnimationHelper.ChangeAnimation(animation);
                AnimationPlayed?.Invoke(animation);

                triggerAnimationEndedEvent = false;
            }
        }

        public void Play()
        {
            if (CurrentAnimation == null)
            {
                _spriteAnimationHelper.ChangeAnimation(DefaultAnimation);
            }

            Play(CurrentAnimation);
        }

        public void Play(int startFrame)
        {
            Play(GetAnimationByName(name));
        }

        public void Play(string name)
        {
            Play(GetAnimationByName(name));
        }

        public void Play(SpriteAnimation animation)
        {
            _state = SpriteAnimationState.Playing;
            _spriteAnimationHelper.ChangeAnimation(animation);
            AnimationPlayed?.Invoke(animation);

            triggerAnimationEndedEvent = false;
        }

        public void Pause()
        {
            _state = SpriteAnimationState.Paused;
            AnimationPaused?.Invoke(CurrentAnimation);
        }

        public void Resume()
        {
            _state = SpriteAnimationState.Playing;
        }

        public SpriteAnimation GetAnimationByName(string name)
        {
            for (int i = 0; i < _spriteAnimations[_spriteAnimationsIndex].spriteAnimations.Count; i++)
            {
                if (_spriteAnimations[_spriteAnimationsIndex].spriteAnimations[i].Name == name)
                {
                    return _spriteAnimations[_spriteAnimationsIndex].spriteAnimations[i];
                }
            }

            return null;
        }

        public void RefreshSpriteAnimations(int index)
        {
            // Check if the new spritesheet index is out of range
            if (index < 0 || index > (_spriteAnimations.Count - 1))
            {
                _spriteAnimationsIndex = _previousSpriteAnimationsIndex;
                Debug.Log($"SpriteAnimations index is out of range, not doing it.");
                return;
            }

            // Check if can find the sprite animation
            var currentSpriteAnimation = GetAnimationByName(CurrentAnimation.Name);
            if (currentSpriteAnimation == null)
            {
                _spriteAnimationsIndex = _previousSpriteAnimationsIndex;
                Debug.Log($"SpriteAnimations index name was not found.");
                return;
            }

            // Check if the new spritesheet index.frames.count is not bigger/smaller to prevent desync
            if (currentSpriteAnimation.Frames.Count != CurrentAnimation.Frames.Count)
            {
                _spriteAnimationsIndex = _previousSpriteAnimationsIndex;
                Debug.Log($"SpriteAnimations frames amount is not equals.");
                return;
            }

            _spriteAnimationsIndex = index;
            _spriteAnimationHelper.CurrentAnimation = currentSpriteAnimation;

            _previousSpriteAnimationsIndex = _spriteAnimationsIndex;
        }

        public void SetSpriteAnimationsIndex(int index)
        {
            _spriteAnimationsIndex = index;
        }
    }
}