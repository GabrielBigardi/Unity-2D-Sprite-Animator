#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class AssetHandler
    {
        [OnOpenAsset]
        public static bool OpenEditor(EntityId entityId, int line)
        {
            var obj = EditorUtility.EntityIdToObject(entityId) as SpriteAnimationObject;
            
            if (obj == null)
                return false;
            
            SpriteAnimationEditorWindow.Open(obj);
            return true;
        }
    }

    [CustomEditor(typeof(SpriteAnimationObject))]
    public class SpriteAnimationCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                SpriteAnimationEditorWindow.Open((SpriteAnimationObject)target);
            }
        }
    }

}
#endif
