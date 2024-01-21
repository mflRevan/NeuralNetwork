using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(ReadonlyInspectorAttribute), true)]
    public class ReadonlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var enabled = GUI.enabled;
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            GUI.enabled = enabled;
        }
    }
}