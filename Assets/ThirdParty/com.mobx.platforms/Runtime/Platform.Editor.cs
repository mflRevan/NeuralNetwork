using MobX.Utilities.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Platforms
{
    public partial class Platform
    {
#if UNITY_EDITOR


        #region Activation

        [Foldout("Activation")]
        [Button("Activate")]
        [ConditionalShow(nameof(IsActive), false, ReadOnly = true)]
        [Annotation("Activation this may switch the active build target causing a long import iteration!")]
        public void Activate()
        {
            PlatformManager.Singleton.Platform = this;
            sceneBuildSettings.Apply();

            var platforms = FindAssetsOfType<Platform>();
            foreach (var platform in platforms)
            {
                platform.RemoveDefines();
            }

            var target = PlatformTypeToBuildTarget(buildPlatform);
            var targetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(target);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            var activeSymbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';');
            var symbols = new List<string>();
            symbols.AddRange(activeSymbols);
            symbols.Add(platformDefine);

            UnityEditor.PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols.ToArray());

            if (target != UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                UnityEditor.EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            }
        }

        private void RemoveDefines()
        {
            var target = PlatformTypeToBuildTarget(buildPlatform);
            var targetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(target);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var activeSymbols = UnityEditor.PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';');
            var symbols = new List<string>();
            symbols.AddRange(activeSymbols);
            symbols.Remove(platformDefine);
            UnityEditor.PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols.ToArray());
        }

        #endregion


        #region Helper

        private static UnityEditor.BuildTarget PlatformTypeToBuildTarget(PlatformType platformType)
        {
            return platformType switch
            {
                PlatformType.Unknown => UnityEditor.BuildTarget.NoTarget,
                PlatformType.StandaloneWindows => UnityEditor.BuildTarget.StandaloneWindows,
                PlatformType.iOS => UnityEditor.BuildTarget.iOS,
                PlatformType.Android => UnityEditor.BuildTarget.Android,
                PlatformType.WebGL => UnityEditor.BuildTarget.WebGL,
                PlatformType.PS4 => UnityEditor.BuildTarget.PS4,
                PlatformType.XboxOne => UnityEditor.BuildTarget.XboxOne,
                PlatformType.tvOS => UnityEditor.BuildTarget.tvOS,
                PlatformType.Switch => UnityEditor.BuildTarget.Switch,
                PlatformType.Stadia => UnityEditor.BuildTarget.Stadia,
                PlatformType.LinuxHeadlessSimulation => UnityEditor.BuildTarget.LinuxHeadlessSimulation,
                PlatformType.GameCoreXboxSeries => UnityEditor.BuildTarget.GameCoreXboxSeries,
                PlatformType.GameCoreXboxOne => UnityEditor.BuildTarget.GameCoreXboxOne,
                PlatformType.PS5 => UnityEditor.BuildTarget.PS5,
                var _ => UnityEditor.BuildTarget.NoTarget
            };
        }

        private static List<T> FindAssetsOfType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        #endregion


#endif
    }
}
