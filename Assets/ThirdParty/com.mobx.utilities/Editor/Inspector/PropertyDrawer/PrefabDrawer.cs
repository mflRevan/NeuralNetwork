using MobX.Utilities.Types;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.PropertyDrawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(Prefab<>), true)]
    internal class PrefabDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            UnityEditor.SerializedProperty prefabProperty = property.FindPropertyRelative("value");

            if (prefabProperty.objectReferenceValue is GameObject obj && !obj.IsPrefab())
            {
                UnityEngine.Debug.LogWarning("Only prefabs are allowed!");
                prefabProperty.objectReferenceValue = null;
            }
            UnityEditor.EditorGUI.PropertyField(position, prefabProperty, label);

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
