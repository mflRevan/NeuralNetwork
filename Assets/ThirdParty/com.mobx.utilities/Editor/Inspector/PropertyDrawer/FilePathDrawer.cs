﻿using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(DrawFilePathAttribute))]
    internal class FilePathDrawer : UnityEditor.PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == UnityEditor.SerializedPropertyType.String)
            {
                var pathAttribute = (DrawFilePathAttribute) attribute;
                var buttonWidth = pathAttribute.ButtonWidth;
                var directEditing = !pathAttribute.Readonly;

                property.serializedObject.Update();
                var path = property.stringValue;
                Rect contentRect = UnityEditor.EditorGUI.PrefixLabel(position, label);

                Rect textRect = contentRect.WithOffset(0, 0, -(buttonWidth + 3));
                Rect buttonRect = textRect.WithOffset(textRect.width + 2).WithWidth(buttonWidth);

                var enabled = GUI.enabled;
                GUI.enabled = enabled && directEditing;

                GUIHelper.BeginIndentOverride(0);
                path = UnityEditor.EditorGUI.TextField(textRect, path);
                GUIHelper.EndIndentOverride();

                GUI.enabled = enabled;

                var style = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 17,
                    fixedHeight = 19,
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(2, 0, 0, 1)
                };

                if (GUI.Button(buttonRect, "⊙", style))
                {
                    var newPath = UnityEditor.EditorUtility.OpenFilePanel("Select File", path.IsNotNullOrWhitespace() ? path : Application.dataPath, pathAttribute.FileExtension);
                    path = newPath.IsNotNullOrWhitespace() ? newPath : path;
                }

                property.stringValue = path;
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                UnityEditor.EditorGUI.LabelField(position, label.text, $"Use {nameof(DrawFilePathAttribute)} with string.");
            }
        }
    }
}
