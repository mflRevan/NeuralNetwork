using MobX.Utilities.Inspector;
using MobX.Utilities.Types;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Platforms
{
    public class SceneBuildSettings : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private List<Optional<string>> sceneLayout = new();

        public void Apply()
        {
#if UNITY_EDITOR
            ApplyToBuildSettings();
#endif
        }

#if UNITY_EDITOR

        [ReadonlyInspector]
        private List<UnityEditor.SceneAsset> _scenes = new();

        [Button]
        private void SaveCurrentBuildSettings()
        {
            sceneLayout.Clear();
            _scenes.Clear();

            foreach (var editorBuildSettingsScene in UnityEditor.EditorBuildSettings.scenes)
            {
                var guid = editorBuildSettingsScene.guid.ToString();
                var enabled = editorBuildSettingsScene.enabled;
                var value = new Optional<string>(guid, enabled);
                sceneLayout.Add(value);
            }

            PopulateSceneAssets();
        }

        [Button]
        private void ApplyToBuildSettings()
        {
            var buffer = new List<UnityEditor.EditorBuildSettingsScene>();

            foreach (var optional in sceneLayout)
            {
                var guid = new UnityEditor.GUID(optional.GetValueDiscrete());
                var enabled = optional.Enabled;
                buffer.Add(new UnityEditor.EditorBuildSettingsScene(guid, enabled));
            }

            UnityEditor.EditorBuildSettings.scenes = buffer.ToArray();
        }

        private void PopulateSceneAssets()
        {
            _scenes.Clear();

            foreach (var layout in sceneLayout)
            {
                var guid = layout.GetValueDiscrete();
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var sceneAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(path);
                _scenes.Add(sceneAsset);
            }
        }

        private void OnValidate()
        {
            PopulateSceneAssets();
        }

#endif
    }
}
