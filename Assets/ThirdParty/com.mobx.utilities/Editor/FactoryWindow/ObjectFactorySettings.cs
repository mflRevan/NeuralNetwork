using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Editor.Inspector;
using System;
using System.Linq;
using UnityEngine;

namespace MobX.Utilities.Editor.FactoryWindow
{
    [Flags]
    public enum SearchOptions
    {
        None = 0,
        BaseTypes = 1,
        AssemblyName = 2,
        CreateAttributePath = 4
    }

    public class ObjectFactorySettings : UnityEditor.EditorWindow
    {
        #region Data

        /*
         * Internal
         */

        internal static event Action<bool> SettingsChanged;

        /*
         * Private
         */

        private static bool isOpen;
        private UnityEditor.SerializedObject _serializedObject;
        private UnityEditor.SerializedProperty _serializedPropertyPrefixes;
        private UnityEditor.SerializedProperty _serializedPropertyNames;
        [SerializeField] private string[] ignoredAssemblyPrefixes = Array.Empty<string>();
        [SerializeField] private string[] ignoredNames = Array.Empty<string>();
        /*
         * Properties
         */

        private FoldoutHandler Foldout { get; set; }

        internal static string[] GetIgnoredAssemblyPrefixes()
        {
            var data = UnityEditor.EditorPrefs.GetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}", string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredAssemblyPrefixes(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            UnityEditor.EditorPrefs.SetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}", data);
        }

        internal static string[] GetIgnoredNames()
        {
            var data = UnityEditor.EditorPrefs.GetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredNames)}", string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredNames(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            UnityEditor.EditorPrefs.SetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredNames)}", data);
        }

        internal static bool EnableMultiAssetCreation
        {
            get => UnityEditor.EditorPrefs.GetBool($"{nameof(ObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}", false);
            set => UnityEditor.EditorPrefs.SetBool($"{nameof(ObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}", value);
        }

        internal static SearchOptions SearchOptions
        {
            get => (SearchOptions) UnityEditor.EditorPrefs.GetInt($"{nameof(ObjectFactoryWindow)}{nameof(SearchOptions)}", (int) (SearchOptions.AssemblyName | SearchOptions.CreateAttributePath | SearchOptions.BaseTypes));
            set => UnityEditor.EditorPrefs.SetInt($"{nameof(ObjectFactoryWindow)}{nameof(SearchOptions)}", (int) value);
        }

        #endregion


        #region Setup

        internal static void OpenWindow()
        {
            if (!isOpen)
            {
                ObjectFactorySettings window = GetWindow<ObjectFactorySettings>("Settings");
                window.Show(false);
            }
        }

        private void OnEnable()
        {
            isOpen = true;
            Foldout = new FoldoutHandler(nameof(ObjectFactorySettings));

            ignoredAssemblyPrefixes = GetIgnoredAssemblyPrefixes();
            ignoredNames = GetIgnoredNames();

            _serializedObject = new UnityEditor.SerializedObject(this);
            _serializedPropertyPrefixes = _serializedObject.FindProperty(nameof(ignoredAssemblyPrefixes));
            _serializedPropertyNames = _serializedObject.FindProperty(nameof(ignoredNames));

            ObjectFactoryWindow.WindowClosed += Close;
        }

        private void OnDisable()
        {
            Save();
            isOpen = false;
            Foldout.SaveState();
            ObjectFactoryWindow.WindowClosed -= Close;
        }

        private void Save()
        {
            SetIgnoredAssemblyPrefixes(ignoredAssemblyPrefixes);
            SetIgnoredNames(ignoredNames);
            SettingsChanged?.Invoke(true);
        }

        #endregion


        #region GUI

        private void OnGUI()
        {
            var changed = false;
            UnityEditor.EditorGUI.indentLevel--;

            if (Foldout["Search Options"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIHelper.Space();
                SearchOptions newSearchOptions = GUIHelper.DrawFlagsEnumAsToggle(SearchOptions, true);
                if (newSearchOptions != SearchOptions)
                {
                    changed = true;
                }
                SearchOptions = newSearchOptions;
                GUIHelper.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            if (Foldout["Misc"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIHelper.Space();

                var enableMulti = EnableMultiAssetCreation;
                var newEnableMulti = UnityEditor.EditorGUILayout.Toggle("Multi Asset Creation", enableMulti);
                if (newEnableMulti != enableMulti)
                {
                    changed = true;
                }
                EnableMultiAssetCreation = newEnableMulti;

                GUIHelper.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            if (Foldout["Assemblies"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIHelper.Space();

                _serializedObject.Update();
                UnityEditor.EditorGUILayout.PropertyField(_serializedPropertyPrefixes);
                UnityEditor.EditorGUILayout.PropertyField(_serializedPropertyNames);
                _serializedObject.ApplyModifiedProperties();
                GUIHelper.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            GUILayout.FlexibleSpace();
            GUIHelper.DrawLine();
            GUIHelper.Space();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save", GUILayout.Width(140)))
            {
                Save();
            }
            if (GUILayout.Button("Save & Close", GUILayout.Width(140)))
            {
                Save();
                Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUIHelper.Space();

            if (changed)
            {
                SettingsChanged?.Invoke(false);
            }
        }

        #endregion
    }
}
