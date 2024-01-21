using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalValuePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private bool _isArray;
        private UnityEditor.SerializedProperty _arrayProperty;
        private UnityEditor.SerializedProperty _enabledProperty;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (_isArray && _enabledProperty.boolValue)
            {
                return _arrayProperty.GetPropertyHeight() + GUIHelper.LineHeight();
            }
            return base.GetPropertyHeight(property, label);
        }

        /// <summary>
        ///     <para>Override this method to make your own IMGUI based GUI for the property.</para>
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            UnityEditor.SerializedProperty enabledProperty = property.FindPropertyRelative("enabled");
            UnityEditor.SerializedProperty valueProperty = property.FindPropertyRelative("value");

            _isArray = valueProperty.isArray && valueProperty.type != "string";
            _arrayProperty = valueProperty;
            _enabledProperty = enabledProperty;

            if (_isArray)
            {
                DrawMultiLine(position, enabledProperty, valueProperty, label);
            }
            else
            {
                DrawSingleLine(position, enabledProperty, valueProperty, label);
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        private static void DrawMultiLine(Rect position, UnityEditor.SerializedProperty enabledProperty,
            UnityEditor.SerializedProperty valueProperty, GUIContent label)
        {
            enabledProperty.boolValue = UnityEditor.EditorGUI.Toggle(position, label, enabledProperty.boolValue);

            if (enabledProperty.boolValue)
            {
                var offset = GUIHelper.LineHeight();
                Rect rect = position.WithOffset(y: offset, height: -offset);
                UnityEditor.EditorGUI.PropertyField(rect, valueProperty, new GUIContent("Content"));
            }
        }

        private static void DrawSingleLine(Rect position, UnityEditor.SerializedProperty enabledProperty,
            UnityEditor.SerializedProperty valueProperty, GUIContent label)
        {
            var labelWidth = UnityEditor.EditorGUIUtility.labelWidth - 12;
            Rect boolRect = position.WithWidth(labelWidth);
            var valueRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);
            enabledProperty.boolValue = UnityEditor.EditorGUI.ToggleLeft(boolRect, label, enabledProperty.boolValue);
            enabledProperty.serializedObject.ApplyModifiedProperties();
            GUIHelper.BeginEnabledOverride(enabledProperty.boolValue);
            UnityEditor.EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
            GUIHelper.EndEnabledOverride();
        }
    }
}
