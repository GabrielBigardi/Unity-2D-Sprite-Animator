using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public enum SpriteAnimationType
    {
        Looping = 0,
        PlayOnce = 1
    }

    [Serializable]
    public class SpriteAnimation
    {
        public string Name;
        public int FPS;
        public List<Sprite> Frames;
        public SpriteAnimationType SpriteAnimationType;
    }
}