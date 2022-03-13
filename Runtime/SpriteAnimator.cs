using UnityEngine;
using System.Collections.Generic;
using System;

namespace GabrielBigardi.SpriteAnimator
{
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private List<SpriteAnimation> _spriteAnimations = new List<SpriteAnimation>();
        [SerializeField] private bool _playAutomatically = true;

        public SpriteAnimation DefaultAnimation => _spriteAnimations.Count > 0 ? _spriteAnimations[0] : null;
        public SpriteAnimation CurrentAnimation => _spriteAnimationHelper.CurrentAnimation;

        public bool Playing => _state == SpriteAnimationState.Playing;
        public bool Paused => _state == SpriteAnimationState.Paused;
        public int CurrentFrame => _spriteAnimationHelper.GetCurrentFrame();
        public bool IsLastFrame => CurrentFrame == CurrentAnimation.Frames.Count - 1;

        private SpriteRenderer _spriteRenderer;
        private SpriteAnimationHelper _spriteAnimationHelper;
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
                SpriteAnimationFrame currentFrame = _spriteAnimationHelper.UpdateAnimation(Time.deltaTime);

                //Change sprite only when animation frame changes, animationended event
                if (currentFrame != null)
                {
                    if (currentFrame != _previousAnimationFrame)
                    {
                        if (triggerAnimationEndedEvent)
                        {
                            AnimationEnded?.Invoke(CurrentAnimation);

                            if (CurrentAnimation.SpriteAnimationType != SpriteAnimationType.Looping) return;
                        }

                        if ((CurrentFrame + 1) > (CurrentAnimation.Frames.Count - 1))
                        {
                            triggerAnimationEndedEvent = true;
                        }

                        _previousAnimationFrame = currentFrame;

                        _spriteRenderer.sprite = currentFrame.Sprite;

                        SpriteChanged?.Invoke();
                        if(currentFrame.EventName != "") AnimationEventCalled?.Invoke(currentFrame.EventName);
                    }
                }

                ////Change sprite every LateUpdate tick, but call sprite changed event based on frame change
                //if(currentFrame != null)
                //{
                //    _spriteRenderer.sprite = currentFrame.Sprite;
                //    if (currentFrame != _previousAnimationFrame)
                //    {
                //        _previousAnimationFrame = currentFrame;
                //        SpriteChanged?.Invoke();
                //    }
                //}

                //Change sprite only on frame change
                //if (currentFrame != null && currentFrame != _previousAnimationFrame)
                //{
                //    _previousAnimationFrame = currentFrame;
                //    _spriteRenderer.sprite = currentFrame.Sprite;
                //}
            }
        }

        public void PlayIfNotPlaying(string name)
        {
            PlayIfNotPlaying(GetAnimationByName(name));
        }

        public void PlayIfNotPlaying(SpriteAnimation animation)
        {
            if (CurrentAnimation.name != name)
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

        public void Play(string name)
        {
            Play(GetAnimationByName(name));
        }

        public void Play(SpriteAnimation animation)
        {
            _state = SpriteAnimationState.Playing;
            _spriteAnimationHelper.ChangeAnimation(animation);
            AnimationPlayed?.Invoke(animation);

            //_previousFrame = 0;
            //_currentAnimationLoops = 0;
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
            for (int i = 0; i < _spriteAnimations.Count; i++)
            {
                if (_spriteAnimations[i].Name == name)
                {
                    return _spriteAnimations[i];
                }
            }

            return null;
        }
    }
}