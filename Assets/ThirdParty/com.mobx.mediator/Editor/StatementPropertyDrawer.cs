using MobX.Mediator.Conditions;
using MobX.Utilities.Editor.Helper;
using UnityEngine;

namespace Mobx.Mediator.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(Statement))]
    public class StatementPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.SerializedProperty typeProperty = property.FindPropertyRelative("checkMethod");
            UnityEditor.SerializedProperty conditionsProperty = property.FindPropertyRelative("conditions");

            GUIHelper.BeginBox();
            UnityEditor.EditorGUILayout.LabelField(label);
            UnityEditor.EditorGUILayout.PropertyField(typeProperty);
            GUIHelper.IncreaseIndent();
            UnityEditor.EditorGUILayout.PropertyField(conditionsProperty);
            GUIHelper.DecreaseIndent();
            GUIHelper.EndBox();
        }
    }
}
