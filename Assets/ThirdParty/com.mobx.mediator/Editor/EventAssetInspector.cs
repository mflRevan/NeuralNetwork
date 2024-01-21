using MobX.Mediator.Events;
using MobX.Utilities.Editor.Inspector;
using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mobx.Mediator.Editor
{
    [UnityEditor.CustomEditor(typeof(EventAssetBase), true)]
    public class EventAssetInspector : OverrideInspector<EventAssetBase>
    {
        private const string EventFieldName = "Event";
        private const string CountProperty = "Count";
        private const string ListenerFieldName = "_listener";

        private bool _showListener;
        private ParameterInfo[] _parameterInfos;

        private Func<int> _count;
        private Func<Delegate[]> _listener;
        private Action<Delegate> _remove;

        private Vector2 _scrollPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            var eventField = GetFieldIncludeBaseTypes(target.GetType(), EventFieldName);
            var eventValue = eventField.GetValue(target);

            var receiverType = eventValue.GetType().BaseType!;
            var propertyInfo = receiverType.GetProperty(CountProperty, BindingFlags.Public | BindingFlags.Instance);
            var listenerField =
                receiverType.GetField(ListenerFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            _count = () => (int) propertyInfo!.GetValue(eventValue);
            _listener = () => listenerField!.GetValue(eventValue) as Delegate[];
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Foldout["Listener"])
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                DrawListenerGUI();
                GUILayout.EndScrollView();
            }
        }

        private void DrawListenerGUI()
        {
            var listeners = _listener();
            var count = _count();

            for (var index = 0; index < count; index++)
            {
                var listener = listeners[index];
                DrawListener(listener);
            }
        }

        private void DrawListener(Delegate listener)
        {
            if (listener == null)
            {
                UnityEditor.EditorGUILayout.LabelField("Listener: NULL");
                UnityEditor.EditorGUILayout.LabelField("Target:   NULL");
                return;
            }

            GUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField($"Listener: {listener.Method}");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                _remove(listener);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField($"Target: {listener.Target}");
            GUILayout.FlexibleSpace();
            if (listener.Target is Object unityObject && unityObject)
            {
                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    UnityEditor.Selection.activeObject = unityObject;
                    UnityEditor.EditorGUIUtility.PingObject(unityObject);
                }

                if (GUILayout.Button("Ping", GUILayout.Width(70)))
                {
                    UnityEditor.EditorGUIUtility.PingObject(unityObject);
                }
            }

            GUILayout.EndHorizontal();
        }


        #region Reflection

        private static FieldInfo GetFieldIncludeBaseTypes(Type type, string fieldName, BindingFlags flags =
            BindingFlags.Static |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.FlattenHierarchy)
        {
            FieldInfo fieldInfo = null;
            var targetType = type;

            while (fieldInfo == null)
            {
                fieldInfo = targetType.GetField(fieldName, flags);
                targetType = targetType.BaseType;

                if (targetType == null)
                {
                    return null;
                }
            }

            return fieldInfo;
        }

        #endregion
    }
}