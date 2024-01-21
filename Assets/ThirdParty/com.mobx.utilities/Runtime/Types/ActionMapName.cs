using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MobX.Utilities.Types
{
    [Serializable]
    public struct ActionMapName
    {
        [SerializeField]
        private string actionMapName;

        public static implicit operator string(ActionMapName actionMap)
        {
            return actionMap.actionMapName;
        }

        public static implicit operator ActionMapName(string name)
        {
            return new ActionMapName(name);
        }

        public ActionMapName(string name)
        {
            actionMapName = name;
        }

        public override string ToString()
        {
            return actionMapName;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ActionMapName))]
    public class ActionMapNamePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private List<string> _actionMapNames;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (_actionMapNames == null)
            {
                PopulateActionMapNames();
            }

            var relativeProperty = property.FindPropertyRelative("actionMapName");

            var currentIndex =
                Mathf.Max(_actionMapNames!.IndexOf(relativeProperty.stringValue), 0);
            var newIndex = UnityEditor.EditorGUI.Popup(position, label.text, currentIndex, _actionMapNames.ToArray());

            relativeProperty.stringValue = _actionMapNames[newIndex];
        }

        private void PopulateActionMapNames()
        {
            _actionMapNames = new List<string>();

            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(InputActionAsset)}");

            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);

                if (asset != null)
                {
                    foreach (var actionMap in asset.actionMaps)
                    {
                        _actionMapNames.Add(actionMap.name);
                    }
                }
            }
        }
    }
#endif
}