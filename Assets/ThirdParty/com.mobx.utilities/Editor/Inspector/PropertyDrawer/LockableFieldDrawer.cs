using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(LockableFieldAttribute))]
    internal class LockableFieldDrawer : UnityEditor.PropertyDrawer
    {
        private bool _writable;
        private const int BUTTON_WIDTH = 50;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var enabled = GUI.enabled;
            GUI.enabled = _writable;
            var textPos = new Rect(position.x, position.y, position.width - BUTTON_WIDTH, position.height);
            UnityEditor.EditorGUI.PropertyField(textPos, property, label);
            GUI.enabled = enabled;

            var buttonPos = new Rect(position.x + position.width - BUTTON_WIDTH, position.y, BUTTON_WIDTH,
                position.height);
            if (GUI.Button(buttonPos, _writable ? "Lock" : "Edit"))
            {
                _writable = !_writable;
            }
        }
    }
}
