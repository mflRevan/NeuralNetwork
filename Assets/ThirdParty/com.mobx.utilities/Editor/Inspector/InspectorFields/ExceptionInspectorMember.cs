using MobX.Utilities.Editor.Helper;
using System;
using System.Reflection;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public class ExceptionInspectorMember : InspectorMember
    {
        private readonly Exception _exception;
        private bool _foldout;

        public ExceptionInspectorMember(Exception exception, MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _exception = exception;
        }

        protected override void DrawGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox(_exception.Message, UnityEditor.MessageType.Error);
            _foldout = UnityEditor.EditorGUILayout.Foldout(_foldout, "Stacktrace", true);
            if (_foldout)
            {
                GUIHelper.BeginBox();
                GUILayout.TextArea(_exception.StackTrace, new GUIStyle("CN Message"));
                GUIHelper.EndBox();
            }
        }
    }
}
