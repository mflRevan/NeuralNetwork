using MobX.Mediator.Values;
using MobX.Utilities.Editor.Helper;
using UnityEngine;

namespace Mobx.Mediator.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(Variable<>), true)]
    [UnityEditor.CustomPropertyDrawer(typeof(ReadonlyVariable<>), true)]
    public class ValuePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private static readonly GUIContent modeLabel = new("Pass By", "Set the value directly or set a reference to a value object.");

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            UnityEditor.SerializedProperty byReferenceProperty = property.FindPropertyRelative("byReference");
            UnityEditor.SerializedProperty referenceProperty = property.FindPropertyRelative("reference");
            UnityEditor.SerializedProperty valueProperty = property.FindPropertyRelative("value");

            UnityEditor.EditorGUILayout.PropertyField(byReferenceProperty.boolValue ? referenceProperty : valueProperty, label);

            var index = byReferenceProperty.boolValue ? 1 : 0;
            index = UnityEditor.EditorGUILayout.Popup(modeLabel, index, new[]
            {
                "Value", "Reference"
            });
            byReferenceProperty.boolValue = index == 1;

            GUIHelper.Space(1);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
