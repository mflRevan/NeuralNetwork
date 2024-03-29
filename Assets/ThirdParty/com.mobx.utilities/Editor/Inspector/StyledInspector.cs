using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.Inspector
{
    public class StyledInspector<T> : UnityEditor.Editor where T : Object
    {
        #region Properties & Fields

        /*
         * Generic Targets
         */

        protected T Target { get; private set; }
        protected T[] Targets { get; private set; }

        /*
         * Foldout Cache
         */

        protected FoldoutHandler Foldout { get; private set; }

        /*
         * Fields
         */

        private string _editorPrefsKey;

        #endregion


        //--------------------------------------------------------------------------------------------------------------

        protected void SetDefaultFoldoutState(FoldoutData data, bool stateValue)
        {
            Foldout.DefaultFoldoutStates.TryAdd(data.Title, stateValue);
        }

        protected static void PopulateSerializedProperties(StyledInspector<T> inspector)
        {
            const BindingFlags Flags =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            var fields = inspector.GetType().GetFields(Flags);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(UnityEditor.SerializedProperty))
                {
                    fieldInfo.SetValue(inspector, inspector.FindProperty(fieldInfo.Name));
                }
            }
        }

        protected UnityEditor.SerializedProperty FindProperty(string member)
        {
            var parsedMemberName = member.StartsWith('_') ? member.Remove(0, 1) : member;
            return serializedObject.FindProperty(parsedMemberName);
        }

        protected virtual void OnEnable()
        {
            Target = target as T;
            Targets = targets.ConvertTo<T>();

            if (Target == null)
            {
                return;
            }

            _editorPrefsKey = $"{Target!.name}{Target.GetType().FullName}";
            LoadStateData(_editorPrefsKey);
        }

        protected virtual void OnDisable()
        {
            SaveStateData(_editorPrefsKey);
        }

        protected virtual void LoadStateData(string editorPrefsKey)
        {
            Foldout = new FoldoutHandler(editorPrefsKey);
            if (Environment.StackTrace.Contains("RedrawFromNative"))
            {
            }
        }

        protected virtual void SaveStateData(string editorPrefsKey)
        {
            if (Environment.StackTrace.Contains("RedrawFromNative"))
            {
                return;
            }
            Foldout?.SaveState();
        }
    }
}