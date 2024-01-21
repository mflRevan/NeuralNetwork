using MobX.Utilities.Inspector;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private const string SceneListItem = "{0} ({1})";
        private const string ScenePattern = @".+\/(.+)\.unity";
        private const string TypeWarningMessage = "{0} must be an int or a string";
        private const string BuildSettingsWarningMessage = "No scenes in the build settings";

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            var validPropertyType = property.propertyType == UnityEditor.SerializedPropertyType.String || property.propertyType == UnityEditor.SerializedPropertyType.Integer;
            var anySceneInBuildSettings = GetScenes().Length > 0;

            return validPropertyType && anySceneInBuildSettings
                ? base.GetPropertyHeight(property, label)
                : 0;
        }

        public override void OnGUI(Rect rect, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(rect, label, property);

            var scenes = GetScenes();
            var anySceneInBuildSettings = scenes.Length > 0;
            if (!anySceneInBuildSettings)
            {
                UnityEditor.EditorGUILayout.HelpBox(BuildSettingsWarningMessage, UnityEditor.MessageType.Warning);
                return;
            }

            var sceneOptions = GetSceneOptions(scenes);
            switch (property.propertyType)
            {
                case UnityEditor.SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, scenes, sceneOptions);
                    break;
                case UnityEditor.SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, sceneOptions);
                    break;
                default:
                    var message = string.Format(TypeWarningMessage, property.name);
                    UnityEditor.EditorGUILayout.HelpBox(message, UnityEditor.MessageType.Warning);
                    break;
            }

            UnityEditor.EditorGUI.EndProperty();
        }

        private string[] GetScenes()
        {
            return UnityEditor.EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Regex.Match(scene.path, ScenePattern).Groups[1].Value)
                .ToArray();
        }

        private string[] GetSceneOptions(string[] scenes)
        {
            return scenes.Select((s, i) => string.Format(SceneListItem, s, i)).ToArray();
        }

        private static void DrawPropertyForString(Rect rect, UnityEditor.SerializedProperty property, GUIContent label, string[] scenes, string[] sceneOptions)
        {
            var index = IndexOf(scenes, property.stringValue);
            var newIndex = UnityEditor.EditorGUI.Popup(rect, label.text, index, sceneOptions);
            var newScene = scenes[newIndex];

            if (!property.stringValue.Equals(newScene, StringComparison.Ordinal))
            {
                property.stringValue = scenes[newIndex];
            }
        }

        private static void DrawPropertyForInt(Rect rect, UnityEditor.SerializedProperty property, GUIContent label, string[] sceneOptions)
        {
            var index = property.intValue;
            var newIndex = UnityEditor.EditorGUI.Popup(rect, label.text, index, sceneOptions);

            if (property.intValue != newIndex)
            {
                property.intValue = newIndex;
            }
        }

        private static int IndexOf(string[] scenes, string scene)
        {
            var index = Array.IndexOf(scenes, scene);
            return Mathf.Clamp(index, 0, scenes.Length - 1);
        }
    }
}
