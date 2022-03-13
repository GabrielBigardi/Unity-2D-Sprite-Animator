using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class SpriteAnimationHelper
    {
        private float _animationTime = 0.0f;

        public SpriteAnimation CurrentAnimation { get; set; }

        private int GetLoopingFrame() => (int)_animationTime % CurrentAnimation.Frames.Count;
        private int GetPlayOnceFrame() => Mathf.Min((int)_animationTime, CurrentAnimation.Frames.Count - 1);

        public SpriteAnimationHelper()
        {
        }

        public SpriteAnimationHelper(SpriteAnimation spriteAnimation)
        {
            CurrentAnimation = spriteAnimation;
        }

        public SpriteAnimationFrame UpdateAnimation(float deltaTime)
        {
            if (CurrentAnimation)
            {
                _animationTime += deltaTime * CurrentAnimation.FPS;

                return GetAnimationFrame();
            }

            return null;
        }

        public void ChangeAnimation(SpriteAnimation spriteAnimation)
        {
            _animationTime = 0f;
            CurrentAnimation = spriteAnimation;
        }

        private SpriteAnimationFrame GetAnimationFrame()
        {
            int currentFrame = 0;

            switch (CurrentAnimation.SpriteAnimationType)
            {
                case SpriteAnimationType.Looping:
                    currentFrame = GetLoopingFrame();
                    break;
                case SpriteAnimationType.PlayOnce:
                    currentFrame = GetPlayOnceFrame();
                    break;
            }

            return CurrentAnimation.Frames[currentFrame];
        }

        public int GetCurrentFrame()
        {
            int currentFrame = 0;

            switch (CurrentAnimation.SpriteAnimationType)
            {
                case SpriteAnimationType.Looping:
                    currentFrame = GetLoopingFrame();
                    break;
                case SpriteAnimationType.PlayOnce:
                    currentFrame = GetPlayOnceFrame();
                    break;
            }

            return currentFrame;
        }
    }
}