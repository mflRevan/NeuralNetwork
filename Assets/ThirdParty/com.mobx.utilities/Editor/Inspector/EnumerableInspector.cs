using MobX.Utilities.Editor.Helper;
using System;
using System.Collections;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    public interface ICollectionInspector
    {
        public void DrawGUI();
    }

    public class EnumerableGUI : ICollectionInspector
    {
        private readonly IEnumerable _enumerable;
        private readonly Action<object> _elementDrawer;

        public EnumerableGUI(IEnumerable enumerable, Type elementType)
        {
            _enumerable = enumerable;
            _elementDrawer = GUIHelper.CreateDrawer(new GUIContent("Element"), elementType);
        }

        public void DrawGUI()
        {
            GUIHelper.BeginBox();
            var count = 0;
            foreach (var item in _enumerable)
            {
                _elementDrawer(item);
                count++;
            }

            if (count <= 0)
            {
                UnityEditor.EditorGUILayout.LabelField("Collection is empty");
            }
            GUIHelper.EndBox();
        }
    }

    public class DictionaryInspector : ICollectionInspector
    {
        private IDictionary _dictionary;

        public void DrawGUI()
        {
        }
    }
}
