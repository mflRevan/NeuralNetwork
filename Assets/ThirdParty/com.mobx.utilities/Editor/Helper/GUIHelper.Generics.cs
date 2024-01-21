using UnityEngine;

namespace MobX.Utilities.Editor.Helper
{
    public static partial class GUIHelper
    {
        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects) where T : Object
        {
            return (T) UnityEditor.EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
        }
    }
}
