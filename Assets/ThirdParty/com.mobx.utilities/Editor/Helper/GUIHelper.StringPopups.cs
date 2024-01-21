using UnityEngine;

namespace MobX.Utilities.Editor.Helper
{
    public static partial class GUIHelper
    {
        public static int StringPopupMask(Rect rect, int mask, string[] available)
        {
            return UnityEditor.EditorGUI.MaskField(rect, mask, available);
        }

        public static string[] StringPopup(Rect rect, string[] current, string[] available)
        {
            var currentIndex = ArrayExtensions.GetMaskIndex(current, available);
            var mask = UnityEditor.EditorGUI.MaskField(rect, currentIndex, available);
            var result = ArrayExtensions.GetSelectionFromMask(mask, available);
            return result;
        }
    }
}
