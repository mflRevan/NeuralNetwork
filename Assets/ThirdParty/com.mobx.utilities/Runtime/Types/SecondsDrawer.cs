#if UNITY_EDITOR
// Custom property drawer for Seconds
using UnityEngine;

namespace MobX.Utilities.Types
{
    [UnityEditor.CustomPropertyDrawer(typeof(Seconds))]
    public class SecondsDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), label);
        }
    }
}

#endif
