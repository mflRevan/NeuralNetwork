using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public class ReadonlyPropertyInspector : InspectorMember
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _target;
        private readonly bool _drawList;
        private readonly bool _inline;
        private readonly UnityEditor.MessageType _messageType;
        private readonly Action<object> _drawer;
        private UnityEditor.Editor _editor;

        public ReadonlyPropertyInspector(PropertyInfo propertyInfo, object target) : base(propertyInfo, target)
        {
            _propertyInfo = propertyInfo;
            _target = target;

            _inline = propertyInfo.HasAttribute<InlineInspectorAttribute>();

            var label = propertyInfo.TryGetCustomAttribute<LabelAttribute>(out LabelAttribute labelAttribute)
                ? labelAttribute.Label
                : _propertyInfo.Name.Humanize(Prefixes);
            var tooltip = propertyInfo.TryGetCustomAttribute<TooltipAttribute>(out TooltipAttribute tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            _drawer = GUIHelper.CreateDrawer(Label, propertyInfo.PropertyType);
        }

        protected override void DrawGUI()
        {
            var value = _propertyInfo.GetValue(_target);
            if (_inline && value is Object targetObject)
            {
                DrawInline(targetObject);
                return;
            }

            var enabled = GUI.enabled;
            GUI.enabled = false;
            _drawer(value);
            GUI.enabled = enabled;
        }

        private void DrawInline(Object targetObject)
        {
            if (targetObject == null)
            {
                _editor = null;
                return;
            }

            if (ReferenceEquals(targetObject, _target))
            {
                return;
            }

            if (_editor != null && _editor.target != targetObject)
            {
                _editor = null;
            }

            _editor ??= UnityEditor.Editor.CreateEditor(targetObject);
            _editor.OnInspectorGUI();
            _editor.serializedObject.ApplyModifiedProperties();
        }
    }

    public class PropertyInspector : InspectorMember
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly object _target;
        private readonly bool _drawList;
        private readonly bool _inline;
        private readonly UnityEditor.MessageType _messageType;
        private readonly Func<object, object> _propertyEditor;
        private UnityEditor.Editor _editor;

        public PropertyInspector(PropertyInfo propertyInfo, object target) : base(propertyInfo, target)
        {
            _propertyInfo = propertyInfo;
            _target = target;

            _inline = propertyInfo.HasAttribute<InlineInspectorAttribute>();

            var label = propertyInfo.TryGetCustomAttribute<LabelAttribute>(out LabelAttribute labelAttribute)
                ? labelAttribute.Label
                : _propertyInfo.Name.Humanize(Prefixes);
            var tooltip = propertyInfo.TryGetCustomAttribute<TooltipAttribute>(out TooltipAttribute tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);

            _propertyEditor = GUIHelper.CreateEditor(Label, propertyInfo.PropertyType);
        }

        protected override void DrawGUI()
        {
            var value = _propertyInfo.GetValue(_target);
            if (_inline && value is Object targetObject)
            {
                DrawInline(targetObject);
                return;
            }
            value = _propertyEditor(value);
            _propertyInfo.SetValue(_target, value);
        }

        private void DrawInline(Object targetObject)
        {
            if (targetObject == null)
            {
                _editor = null;
                return;
            }

            if (ReferenceEquals(targetObject, _target))
            {
                return;
            }

            if (_editor != null && _editor.target != targetObject)
            {
                _editor = null;
            }

            _editor ??= UnityEditor.Editor.CreateEditor(targetObject);
            _editor.OnInspectorGUI();
            _editor.serializedObject.ApplyModifiedProperties();
        }
    }
}
