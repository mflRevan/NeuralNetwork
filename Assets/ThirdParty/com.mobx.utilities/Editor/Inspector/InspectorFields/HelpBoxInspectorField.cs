using System.Reflection;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public class HelpBoxInspectorMember : InspectorMember
    {
        private readonly string _message;
        private readonly UnityEditor.MessageType _messageType;

        public HelpBoxInspectorMember(string message, UnityEditor.MessageType messageType, MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _message = message;
            _messageType = messageType;
        }

        protected override void DrawGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox(_message, _messageType);
        }
    }
}
