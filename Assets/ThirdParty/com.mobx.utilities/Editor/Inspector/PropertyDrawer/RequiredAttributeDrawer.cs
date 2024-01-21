using MobX.Utilities.Inspector;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(RequiredAttribute), true)]
    public class RequiredAttributeDrawer : UnityEditor.PropertyDrawer
    {
        private float _propertyHeight;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return _propertyHeight;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            var requiredAttribute = (RequiredAttribute) attribute;
            if (property.type == "string")
            {
                if (property.stringValue.IsNullOrWhitespace())
                {
                    var baseHeight = base.GetPropertyHeight(property, label);
                    var helpRect = new Rect(position.x + UnityEditor.EditorGUI.indentLevel * 12, position.y, position.width,
                        baseHeight * 2 - 2);
                    var propRect = new Rect(position.x, position.y + baseHeight * 2, position.width, baseHeight);

                    var message = requiredAttribute.Message ?? $"{property.displayName} is Required!";
                    var messageType = requiredAttribute.MessageLevel;

                    UnityEditor.EditorGUI.HelpBox(helpRect, message, (UnityEditor.MessageType) messageType);
                    UnityEditor.EditorGUI.PropertyField(propRect, property, label);

                    _propertyHeight = baseHeight * 3;
                }
                else
                {
                    _propertyHeight = base.GetPropertyHeight(property, label);
                    UnityEditor.EditorGUI.PropertyField(position, property, label);
                }
            }
            else if (property.objectReferenceValue == null)
            {
                var baseHeight = base.GetPropertyHeight(property, label);
                var helpRect = new Rect(position.x + UnityEditor.EditorGUI.indentLevel * 12, position.y, position.width,
                    baseHeight * 2 - 2);
                var propRect = new Rect(position.x, position.y + baseHeight * 2, position.width, baseHeight);

                var message = requiredAttribute.Message ?? $"{property.displayName} is Required!";
                var messageType = requiredAttribute.MessageLevel;

                UnityEditor.EditorGUI.HelpBox(helpRect, message, (UnityEditor.MessageType) messageType);
                UnityEditor.EditorGUI.PropertyField(propRect, property, label);

                _propertyHeight = baseHeight * 3;
            }
            else
            {
                _propertyHeight = base.GetPropertyHeight(property, label);
                UnityEditor.EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
