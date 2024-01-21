using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(RichTextPreviewAttribute))]
    internal class RichTextPreviewDrawer : UnityEditor.PropertyDrawer
    {
        private GUIContent _richTextLabel;
        private static bool Enabled
        {
            get => UnityEditor.EditorPrefs.GetBool("RichTextPreview");
            set => UnityEditor.EditorPrefs.SetBool("RichTextPreview", value);
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == UnityEditor.SerializedPropertyType.String)
            {
                property.serializedObject.Update();

                UnityEditor.EditorGUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.LabelField(label);
                if (GUILayout.Button(Enabled ? "Disable RichText" : "Enable RichText", GUILayout.Width(130)))
                {
                    Enabled = !Enabled;
                }
                UnityEditor.EditorGUILayout.EndHorizontal();
                property.stringValue = UnityEditor.EditorGUILayout.TextArea(property.stringValue, Enabled ? GUIHelper.RichTextArea : GUIHelper.TextArea);
                property.serializedObject.ApplyModifiedProperties();

                _richTextLabel ??= new GUIContent("Enable Rich Text")
                {
                    tooltip = $"{label.tooltip} (toggle to preview rich text formatting)"
                };
            }
            else
            {
                UnityEditor.EditorGUILayout.LabelField(label.text, "Use RichText Preview with string.");
            }
        }
    }
}
