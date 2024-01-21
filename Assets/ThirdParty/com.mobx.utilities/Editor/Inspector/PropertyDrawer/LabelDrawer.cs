using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public class LabelDrawer : UnityEditor.PropertyDrawer
    {
        private string _label;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            _label ??= ((LabelAttribute) attribute).Label;
            label.text = _label;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
        }
    }
}
