using System.Collections.Generic;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    [CreateAssetMenu]
    public class SpriteAnimationObject : ScriptableObject
    {
        public List<SpriteAnimation> SpriteAnimations = new List<SpriteAnimation>(5);
    }
}