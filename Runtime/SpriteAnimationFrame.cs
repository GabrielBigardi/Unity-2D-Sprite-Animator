using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace GabrielBigardi.SpriteAnimator.Runtime
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        public Sprite Sprite;
        public string EventName;
    }
}