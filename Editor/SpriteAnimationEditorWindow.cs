using System.Collections;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class SpriteAnimationEditorWindow : ExtendedEditorWindow
    {
        private Texture2D _sheetGenerationTexture;
        private int _currentFrame = 0;
        private EditorCoroutine _coroutine;
        private Texture2D _transparentCheckboardTexture;
        [SerializeField] private SpriteAnimationObject _spriteAnimationObject;

        public static void Open(SpriteAnimationObject spriteAnimationObject)
        {
            SpriteAnimationEditorWindow window = GetWindow<SpriteAnimationEditorWindow>("Sprite Animation Editor");
            window._serializedObject = new SerializedObject(spriteAnimationObject);
            window._spriteAnimationObject = spriteAnimationObject;
            window._selectedProperty = null;
            window._selectedPropertyPath = "";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string spritesPath = Path.Combine("Sprites");
            string checkeredTexturePath = Path.Combine(scriptFolder, spritesPath, "checkered.png");
            _transparentCheckboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(checkeredTexturePath);

            if (_spriteAnimationObject != null)
            {
                _serializedObject = new SerializedObject(_spriteAnimationObject);
                _currentProperty = _serializedObject.FindProperty("SpriteAnimations");
                RestartCoroutine();
            }
        }

        private void OnDisable()
        {
            StopCoroutine();
        }

        private void StartCoroutine()
        {
            _coroutine = EditorCoroutineUtility.StartCoroutine(RefreshSprite_CR(), this);
        }

        private void StopCoroutine()
        {
            if (_coroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void RestartCoroutine()
        {
            StopCoroutine();
            StartCoroutine();
        }

        IEnumerator RefreshSprite_CR()
        {
            _currentFrame = 0;

            while (true)
            {
                if (_currentProperty != null)
                {
                    SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");
                    SerializedProperty fpsProperty = _currentProperty.FindPropertyRelative("FPS");
                    if (framesProperty != null && fpsProperty != null && fpsProperty.intValue > 0 && framesProperty.arraySize > 0)
                    {
                        _currentFrame = (_currentFrame + 1) % framesProperty.arraySize;
                        Repaint();
                        yield return new EditorWaitForSeconds(1f / fpsProperty.intValue);
                    }
                    else
                    {
                        yield return new EditorWaitForSeconds(0.1f);
                    }
                }
                else
                {
                    yield return new EditorWaitForSeconds(0.1f);
                }
            }
        }

        private void OnGUI()
        {
            if (_serializedObject == null)
                return;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            minSize = new Vector2(640, 480);
            _currentProperty = _serializedObject.FindProperty("SpriteAnimations");

            if (_currentProperty == null)
                return;

            EditorGUILayout.BeginHorizontal();

            // === List Items ===
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

            DrawSideBar(_currentProperty);

            EditorGUILayout.EndVertical();
            // ==================

            // === Show Object Details ===
            EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(240), GUILayout.MaxWidth(1000), GUILayout.ExpandHeight(true));
            if (_selectedProperty != null)
            {
                //DrawProperties(_selectedProperty, true);
                DrawSelectedPropertiesPanel();
            }
            else
            {
                EditorGUILayout.LabelField("Select an item from the list", labelStyle);
            }
            EditorGUILayout.EndVertical();
            // ======================

            // === Animation Preview ===
            if (_selectedProperty != null)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
                DrawAnimationPreview();
                EditorGUILayout.EndVertical();
            }

            // =========================

            EditorGUILayout.EndHorizontal();

            Apply();
        }

        private void DrawSelectedPropertiesPanel()
        {
            if (_selectedProperty == null)
                return;

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            _currentProperty = _selectedProperty;


            // === Sprite Animation Data ===
            EditorGUILayout.LabelField("Sprite Animation Data", labelStyle);
            EditorGUILayout.BeginVertical("box");

            DrawField("Name", true);
            DrawField("FPS", true);
            DrawField("Frames", true);
            DrawField("SpriteAnimationType", true);
            // and so on...

            EditorGUILayout.EndVertical();
            // ======================

            EditorGUILayout.Space(15, true);

            // === Sheet Generation ===
            EditorGUILayout.LabelField("Sheet Generation", labelStyle);
            EditorGUILayout.BeginVertical("box");
            _sheetGenerationTexture = (Texture2D)EditorGUILayout.ObjectField("Texture", _sheetGenerationTexture, typeof(Texture2D), false);
            if (_sheetGenerationTexture != null && GUILayout.Button("Generate Sheet"))
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_sheetGenerationTexture));

                Sprite[] sprites = assets.OfType<Sprite>().ToArray();
                SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");
                framesProperty.ClearArray();
                for (int i = 0; i < sprites.Length; i++)
                {
                    framesProperty.InsertArrayElementAtIndex(i);
                    framesProperty.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
                }

                _sheetGenerationTexture = null;
            }
            EditorGUILayout.EndVertical();
            // =========================
        }

        private void DrawAnimationPreview()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            _currentProperty = _selectedProperty;
            EditorGUILayout.LabelField("Sprite Animation Preview", labelStyle);
            Rect boxRect = EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");
            if (framesProperty != null && framesProperty.arraySize - 1 >= _currentFrame)
            {
                SerializedProperty framesPropertyElement = framesProperty.GetArrayElementAtIndex(_currentFrame);
                Sprite sprite = framesPropertyElement.objectReferenceValue as Sprite;

                int textureRectSize = 200;
                Rect textureRect = new(boxRect.x + (boxRect.width / 2f) - textureRectSize / 2f, boxRect.y, textureRectSize, textureRectSize);
                EditorGUI.DrawTextureTransparent(textureRect, _transparentCheckboardTexture, ScaleMode.ScaleToFit);

                if (sprite != null)
                {
                    GUIDrawSprite(textureRect, sprite);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public void GUIDrawSprite(Rect rect, Sprite sprite)
        {
            Rect spriteRect = sprite.rect;
            Texture2D tex = sprite.texture;
            GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
        }

        protected override void OnElementClicked()
        {
            _currentFrame = 0;
            RestartCoroutine();
        }
    }
}
