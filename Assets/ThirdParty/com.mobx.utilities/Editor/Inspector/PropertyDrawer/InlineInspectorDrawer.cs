using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(InlineInspectorAttribute))]
    public class InlineInspectorDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEditor.Editor _inspector;
        private InlineInspectorAttribute _inspectorAttribute;
        private static readonly Dictionary<string, bool> foldoutStatuses = new();

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return -2;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            _inspectorAttribute ??= (InlineInspectorAttribute) attribute;
            if (_inspectorAttribute is { Required: true } && property.objectReferenceValue == null)
            {
                var message = $"{property.displayName} is Required!";
                UnityEditor.EditorGUILayout.HelpBox(message, UnityEditor.MessageType.Error);
            }

            UnityEditor.EditorGUILayout.PropertyField(property, label);

            if (property.propertyType != UnityEditor.SerializedPropertyType.ObjectReference)
            {
                return;
            }

            if (property.objectReferenceValue == null)
            {
                _inspector = null;
                return;
            }

            if (ReferenceEquals(property.objectReferenceValue, property.serializedObject.targetObject))
            {
                return;
            }

            foldoutStatuses.TryGetValue(property.propertyPath, out var foldoutState);

            Rect rect = GUIHelper.GetLastRect();

            if (UnityEditor.EditorGUI.indentLevel > 0)
            {
                rect = rect.WithOffset(-13);
            }

            foldoutState = UnityEditor.EditorGUI.Foldout(rect, foldoutState, "", true);
            foldoutStatuses[property.propertyPath] = foldoutState;

            if (foldoutState)
            {
                GUIHelper.IncreaseIndent();
                GUIHelper.HideMonoScript = true;
                GetOrCreateInspector(property).OnInspectorGUI();
                GUIHelper.HideMonoScript = false;
                GUIHelper.DecreaseIndent();
            }
        }

        private UnityEditor.Editor GetOrCreateInspector(UnityEditor.SerializedProperty property)
        {
            if (_inspector && _inspector.target != property.objectReferenceValue)
            {
                _inspector = null;
            }
            _inspector ??= UnityEditor.Editor.CreateEditor(property.objectReferenceValue);

            return _inspector;
        }
    }
}
