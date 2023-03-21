using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            SpriteAnimationObject obj = EditorUtility.InstanceIDToObject(instanceId) as SpriteAnimationObject;
            if (obj != null)
            {
                SpriteAnimationEditorWindow.Open(obj);
                return true;
            }
            return false;
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