#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class SpriteAnimationEditorWindow : ExtendedEditorWindow
    {
        private Texture2D _sheetGenerationTexture;
        private Texture2D _transparentCheckboardTexture;
        [SerializeField] private SpriteAnimationObject _spriteAnimationObject;

        private float _lastTime;
        private float _unscaledDeltaTime;
        private float _deltaTime;
        private float _timeScale = 1f;

        private int _currentFrame = 0;
        private float _animationTime;

        private bool _animationIsRunning;
        private bool _animationComplete;

        private Texture2D _buttonPlayTexture;
        private Texture2D _buttonPauseTexture;
        private Texture2D _buttonNextFrameTexture;
        private Texture2D _buttonPreviousFrameTexture;
        private readonly float _maxItemListColumnWidth = 150;
        private readonly float _maxSpritePreviewColumnWidth = 150;
        private Vector2 _scrollPosition;
        private Texture2D CurrentPauseButtonLabel => _animationIsRunning ? _buttonPauseTexture : _buttonPlayTexture;

        private int _previousFramesPropertySize = 0;

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

            _scrollPosition = Vector2.zero;

            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string spritesPath = Path.Combine("Sprites");

            string checkeredTexturePath = Path.Combine(scriptFolder, spritesPath, "checkered.png");
            _transparentCheckboardTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(checkeredTexturePath);

            string buttonPlayFullPath = Path.Combine(scriptFolder, spritesPath, "buttonplaytexture.png");
            _buttonPlayTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonPlayFullPath);

            string buttonPauseFullPath = Path.Combine(scriptFolder, spritesPath, "buttonpausetexture.png");
            _buttonPauseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonPauseFullPath);

            string buttonNextFrameFullPath = Path.Combine(scriptFolder, spritesPath, "buttonnextframetexture.png");
            _buttonNextFrameTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonNextFrameFullPath);

            string buttonPreviousFrameFullPath = Path.Combine(scriptFolder, spritesPath, "buttonpreviousframetexture.png");
            _buttonPreviousFrameTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonPreviousFrameFullPath);

            if (_spriteAnimationObject != null)
            {
                _serializedObject = new SerializedObject(_spriteAnimationObject);
                _currentProperty = _serializedObject.FindProperty("SpriteAnimations");
            }

            _lastTime = Time.realtimeSinceStartup;
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
        }

        private void OnEditorApplicationUpdate()
        {
            _unscaledDeltaTime = Time.realtimeSinceStartup - _lastTime;
            _deltaTime = (Time.realtimeSinceStartup - _lastTime) * _timeScale;
            _lastTime = Time.realtimeSinceStartup;

            if (_spriteAnimationObject != null)
            {
                if (_selectedProperty == null)
                    return;

                SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");
                SerializedProperty fpsProperty = _selectedProperty.FindPropertyRelative("FPS");
                SerializedProperty animationTypeProperty = _selectedProperty.FindPropertyRelative("SpriteAnimationType");

                if (fpsProperty == null || framesProperty == null || animationTypeProperty == null)
                    return;

                // If frames property array size has changed
                if (_previousFramesPropertySize != framesProperty.arraySize)
                {
                    // If current frame is greater than new frames array size, set it to the array size
                    if(_currentFrame >= framesProperty.arraySize && framesProperty.arraySize > 0)
                    {
                        _currentFrame = framesProperty.arraySize - 1;
                        _animationTime = _currentFrame;
                        _animationIsRunning = false;
                    }

                    _previousFramesPropertySize = framesProperty.arraySize;
                }

                if (_animationIsRunning)
                {
                    _animationTime += _deltaTime * fpsProperty.intValue;

                    var frameDuration = 1f / fpsProperty.intValue;
                    var animationDuration = frameDuration * (framesProperty.arraySize);

                    if (!_animationComplete && _animationTime >= (animationDuration * fpsProperty.intValue))
                    {
                        if (animationTypeProperty.intValue == (int)SpriteAnimationType.Looping)
                        {
                            _animationTime = 0f;
                            _currentFrame = 0;
                        }
                        else
                        {
                            _animationIsRunning = false;
                            _animationTime = _currentFrame;
                            _animationComplete = true;
                        }
                    }

                    _currentFrame = Mathf.Min((int)_animationTime, framesProperty.arraySize);
                }


                Repaint();
            }

            EditorApplication.QueuePlayerLoopUpdate();
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
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(_maxItemListColumnWidth), GUILayout.ExpandHeight(true));
            DrawSideBar(_currentProperty);
            EditorGUILayout.EndVertical();

            // === Show Object Details ===
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(int.MaxValue), GUILayout.ExpandHeight(true));
            if (_selectedProperty != null)
            {
                //DrawProperties(_selectedProperty, true);
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                DrawSelectedPropertiesPanel();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("Select an item from the list", labelStyle);
            }
            EditorGUILayout.EndVertical();

            // === Animation Preview ===
            if (_selectedProperty != null)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(_maxSpritePreviewColumnWidth), GUILayout.ExpandHeight(true));
                DrawAnimationPreview();
                EditorGUILayout.EndVertical();
            }

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
            DrawTextureDisplay(boxRect);
            
            Rect labelRect = GUILayoutUtility.GetRect(new GUIContent($"Current Frame: {_currentFrame}"), labelStyle);
            labelRect.y -= 4;
            EditorGUI.DropShadowLabel(labelRect, $"Current Frame: {_currentFrame}");
            
            EditorGUILayout.Space(200);
            DrawDisplayButtons();

            //EditorGUILayout.Space(200);
            //DrawTimeScaleSlider();

            EditorGUILayout.EndVertical();
        }

        private void DrawTextureDisplay(Rect boxRect)
        {
            int textureRectSize = 200;
            Rect textureRect = new(boxRect.x + (boxRect.width / 2f) - textureRectSize / 2f, boxRect.y, textureRectSize, textureRectSize);
            EditorGUI.DrawTextureTransparent(textureRect, _transparentCheckboardTexture, ScaleMode.ScaleToFit);

            SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");

            if (framesProperty != null && framesProperty.arraySize > 0 && _currentFrame < framesProperty.arraySize)
            {
                SerializedProperty framesPropertyElement = framesProperty.GetArrayElementAtIndex(_currentFrame);
                Sprite sprite = framesPropertyElement.objectReferenceValue as Sprite;

                if (sprite != null)
                {
                    GUIDrawSprite(textureRect, sprite);
                }
            }
        }

        private void DrawTimeScaleSlider()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("Playback Speed", labelStyle);
            _timeScale = EditorGUILayout.Slider(_timeScale, 0f, 2f);
        }

        private void DrawDisplayButtons()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(_buttonPreviousFrameTexture, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");

                _animationIsRunning = false;
                _currentFrame--;

                if (_currentFrame < 0)
                    _currentFrame = framesProperty.arraySize > 0 ? framesProperty.arraySize - 1 : 0;

                _animationTime = _currentFrame;
                _animationComplete = _currentFrame == framesProperty.arraySize - 1;
            }

            if (GUILayout.Button(CurrentPauseButtonLabel, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                _animationIsRunning = !_animationIsRunning;

                if (_animationComplete)
                {
                    _animationTime = 0f;
                    _animationComplete = false;
                }

                _timeScale = _animationIsRunning ? 1f : 0f;
            }

            if (GUILayout.Button(_buttonNextFrameTexture, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
            {
                SerializedProperty framesProperty = _currentProperty.FindPropertyRelative("Frames");

                _animationIsRunning = false;
                _currentFrame++;

                if (_currentFrame > framesProperty.arraySize - 1)
                    _currentFrame = 0;

                _animationTime = _currentFrame;
                _animationComplete = _currentFrame == framesProperty.arraySize - 1;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public void GUIDrawSprite(Rect previewRect, Sprite sprite)
        {
            // Get the sprite's rect and texture
            Rect spriteRect = sprite.rect;
            Texture2D texture = sprite.texture;

            // Calculate the aspect ratio of the sprite
            float spriteAspectRatio = spriteRect.width / spriteRect.height;

            // Calculate the maximum integer scaling factor that fits within the preview rect
            int scaleFactor = Mathf.FloorToInt(Mathf.Min(previewRect.width / spriteRect.width, previewRect.height / spriteRect.height));

            // If scaleFactor is greater than 0, use pixel-perfect scaling
            if (scaleFactor > 0)
            {
                // Calculate the new size of the sprite based on the scale factor
                float displayWidth = scaleFactor * spriteRect.width;
                float displayHeight = scaleFactor * spriteRect.height;

                // Center the scaled sprite within the preview rect
                Rect fitRect = new Rect(
                    previewRect.x + (previewRect.width - displayWidth) / 2f,
                    previewRect.y + (previewRect.height - displayHeight) / 2f,
                    displayWidth,
                    displayHeight
                );

                // Ensure the rect aligns with the pixel grid to avoid cutting off part of the sprite
                fitRect.x = Mathf.Floor(fitRect.x);
                fitRect.y = Mathf.Floor(fitRect.y);
                fitRect.width = Mathf.Floor(fitRect.width);
                fitRect.height = Mathf.Floor(fitRect.height);

                // Draw the sprite's texture using the calculated scaling
                GUI.DrawTextureWithTexCoords(fitRect, texture, new Rect(
                    spriteRect.x / texture.width,
                    spriteRect.y / texture.height,
                    spriteRect.width / texture.width,
                    spriteRect.height / texture.height));
            }
            else
            {
                // Fallback to aspect ratio scaling (non-pixel-perfect) if scaleFactor is 0

                Rect fitRect = previewRect;

                if (spriteAspectRatio > 1) // Sprite is wider than tall
                {
                    float height = previewRect.width / spriteAspectRatio;
                    fitRect = new Rect(previewRect.x, previewRect.y + (previewRect.height - height) / 2f, previewRect.width, height);
                }
                else // Sprite is taller than wide or square
                {
                    float width = previewRect.height * spriteAspectRatio;
                    fitRect = new Rect(previewRect.x + (previewRect.width - width) / 2f, previewRect.y, width, previewRect.height);
                }

                // Draw the sprite's texture within the calculated rect while keeping its aspect ratio
                GUI.DrawTextureWithTexCoords(fitRect, texture, new Rect(
                    spriteRect.x / texture.width,
                    spriteRect.y / texture.height,
                    spriteRect.width / texture.width,
                    spriteRect.height / texture.height));
            }
        }

        protected override void OnElementClicked()
        {
            _currentFrame = 0;
            _animationTime = 0f;
            _animationIsRunning = true;
            _animationComplete = false;
            _timeScale = 1f;
        }
    }
}
#endif