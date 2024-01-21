using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using System;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(FlagsToggleAttribute))]
    public class FlagsToggleDrawer : UnityEditor.PropertyDrawer
    {
        private Type _underLying;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != UnityEditor.SerializedPropertyType.Enum)
            {
                GUIHelper.DrawMessageBox("Property must be an enum!");
                return;
            }

            _underLying ??= property.GetUnderlyingType() ?? throw new Exception();
            property.enumValueFlag = GUIHelper.DrawFlagsEnumAsToggle(property.enumValueFlag, _underLying, true);
        }
    }
}
