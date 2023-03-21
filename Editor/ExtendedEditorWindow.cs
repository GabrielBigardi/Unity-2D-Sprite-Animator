using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GabrielBigardi.SpriteAnimator
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject _serializedObject;
        protected SerializedProperty _currentProperty;

        protected string _selectedPropertyPath;
        protected SerializedProperty _selectedProperty;

        private Texture2D _buttonUpTexture;
        private Texture2D _buttonDownTexture;
        private Texture2D _buttonNegativeTexture;

        protected virtual void OnEnable()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string scriptFolder = Path.GetDirectoryName(scriptPath);
            string spritesPath = Path.Combine("Sprites");

            string buttonUpFullPath = Path.Combine(scriptFolder, spritesPath, "buttonuptexture.png");
            _buttonUpTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonUpFullPath);

            string buttonDownFullPath = Path.Combine(scriptFolder, spritesPath, "buttondowntexture.png");
            _buttonDownTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonDownFullPath);

            string buttonNegativeTexture = Path.Combine(scriptFolder, spritesPath, "buttonnegativetexture.png");
            _buttonNegativeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(buttonNegativeTexture);
        }

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        protected void DrawSideBar(SerializedProperty prop)
        {
            if (prop == null)
                return;

            if (GUILayout.Button("Add New Item"))
            {
                GUI.FocusControl("");
                prop.arraySize++;
                _selectedPropertyPath = prop.GetArrayElementAtIndex(prop.arraySize - 1).propertyPath;
            }

            foreach (SerializedProperty p in prop)
            {
                EditorGUILayout.BeginHorizontal();
                var path = p.propertyPath;
                int pos = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
                if (GUILayout.Button(ObjectNames.NicifyVariableName($"{pos} : {p.displayName}")))
                {
                    GUI.FocusControl("");
                    _selectedPropertyPath = p.propertyPath;
                    OnElementClicked();
                }

                if (GUILayout.Button(_buttonUpTexture, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    GUI.FocusControl("");
                    if (pos == 0)
                    {
                        prop.MoveArrayElement(pos, prop.arraySize - 1);
                    }
                    else
                    {
                        prop.MoveArrayElement(pos, pos - 1);
                    }
                }

                if (GUILayout.Button(_buttonDownTexture, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    GUI.FocusControl("");
                    if (pos == prop.arraySize - 1)
                    {
                        prop.MoveArrayElement(pos, 0);
                    }
                    else
                    {
                        prop.MoveArrayElement(pos, pos + 1);
                    }
                }

                if (GUILayout.Button(_buttonNegativeTexture, GUILayout.MinWidth(20), GUILayout.MinHeight(20), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                {
                    GUI.FocusControl("");
                    prop.DeleteArrayElementAtIndex(pos);
                }

                if (!string.IsNullOrEmpty(_selectedPropertyPath))
                {
                    _selectedProperty = _serializedObject.FindProperty(_selectedPropertyPath);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        protected void DrawField(string propName, bool relative)
        {
            if (relative && _currentProperty != null)
            {
                EditorGUILayout.PropertyField(_currentProperty.FindPropertyRelative(propName), true);
            }
            else if (_serializedObject != null)
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty(propName), true);
            }
        }

        protected void Apply() => _serializedObject.ApplyModifiedProperties();

        protected virtual void OnElementClicked() { }
    }
}