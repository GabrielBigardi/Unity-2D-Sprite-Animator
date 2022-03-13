using UnityEngine;
using UnityEditorInternal;
using UnityEditor;

namespace GabrielBigardi.SpriteAnimator
{
    [CustomEditor(typeof(SpriteAnimation))]
    [CanEditMultipleObjects]
    public class SpriteAnimationEditor : Editor
    {
        private SpriteAnimation SelectedSpriteAnimation => target as SpriteAnimation;

        private float timeTracker = 0;

        private SpriteAnimationFrame currentFrame;
        private SpriteAnimationHelper spriteAnimationHelper;

        private SerializedProperty _name;
        private SerializedProperty _fps;
        private SerializedProperty _frames;
        private SerializedProperty _spriteAnimationType;

        private ReorderableList _framesReorderableList;

        private void OnEnable()
        {
            timeTracker = (float)EditorApplication.timeSinceStartup;
            spriteAnimationHelper = new SpriteAnimationHelper(SelectedSpriteAnimation);

            _name = serializedObject.FindProperty("Name");
            _fps = serializedObject.FindProperty("FPS");
            _frames = serializedObject.FindProperty("Frames");
            _spriteAnimationType = serializedObject.FindProperty("SpriteAnimationType");

            _framesReorderableList = new ReorderableList(serializedObject, _frames, true, true, true, true);
            _framesReorderableList.drawElementCallback = DrawFramesListElements;
            _framesReorderableList.drawHeaderCallback = DrawFramesListHeader;

            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_name);
            EditorGUILayout.PropertyField(_fps);
            _framesReorderableList.DoLayoutList();
            EditorGUILayout.PropertyField(_spriteAnimationType);

            serializedObject.ApplyModifiedProperties();
        }

        public override bool HasPreviewGUI()
        {
            return HasAnimationAndFrames();
        }

        public override bool RequiresConstantRepaint()
        {
            return HasAnimationAndFrames();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (currentFrame != null && currentFrame.Sprite != null)
            {
                Texture t = currentFrame.Sprite.texture;
                Rect tr = currentFrame.Sprite.textureRect;
                Rect r2 = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height);

                Rect previewRect = r;

                float targetAspectRatio = tr.width / tr.height;
                float windowAspectRatio = r.width / r.height;
                float scaleHeight = windowAspectRatio / targetAspectRatio;

                if (scaleHeight < 1f)
                {
                    previewRect.width = r.width;
                    previewRect.height = scaleHeight * r.height;
                    previewRect.x = r.x;
                    previewRect.y = r.y + (r.height - previewRect.height) / 2f;
                }
                else
                {
                    float scaleWidth = 1f / scaleHeight;

                    previewRect.width = scaleWidth * r.width;
                    previewRect.height = r.height;
                    previewRect.x = r.x + (r.width - previewRect.width) / 2f;
                    previewRect.y = r.y;
                }

                GUI.DrawTextureWithTexCoords(previewRect, t, r2, true);
            }
        }

        private void DrawFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _framesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        private void DrawFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Frames List");
        }

        private bool HasAnimationAndFrames()
        {
            return SelectedSpriteAnimation != null && SelectedSpriteAnimation.Frames.Count > 0;
        }

        private void OnUpdate()
        {
            if (SelectedSpriteAnimation != null && SelectedSpriteAnimation.Frames.Count > 0)
            {
                float deltaTime = (float)EditorApplication.timeSinceStartup - timeTracker;
                timeTracker += deltaTime;
                currentFrame = spriteAnimationHelper.UpdateAnimation(deltaTime);
            }
        }
    }
}