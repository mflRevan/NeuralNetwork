using MobX.Utilities;
using MobX.Utilities.Editor.AssetManagement;
using MobX.Utilities.Editor.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Platforms.Editor
{
    [UnityEditor.CustomEditor(typeof(PlatformManager))]
    public class PlatformManagerInspector : UnityEditor.Editor
    {
        private readonly List<Platform> _platforms = new();
        private Action _activationDelegate;

        private void OnEnable()
        {
            _platforms.Clear();
            _platforms.AddRange(AssetDatabaseUtilities.FindAssetsOfType<Platform>());
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUIHelper.Space();
            GUIHelper.BoldLabel("Platforms");
            _activationDelegate = null;
            GUIHelper.BeginBox();
            foreach (var platform in _platforms)
            {
                DrawPlatform(platform);
            }
            GUIHelper.EndBox();
            GUIHelper.Space();
            _activationDelegate?.Invoke();
        }

        private void DrawPlatform(Platform platform)
        {
            var position = GUIHelper.GetControlRect();
            var labelWidth = GUIHelper.GetLabelWidth();

            var nameRect = position.WithWidth(labelWidth);
            var fieldRect = position.WithOffset(labelWidth, width: -labelWidth);
            var elementWidth = fieldRect.width / 2f;
            var buildTargetRect = fieldRect.WithWidth(elementWidth);
            var buttonRect = buildTargetRect.WithOffset(elementWidth);

            GUI.Label(nameRect, platform.name.Replace("Platform", ""));
            GUI.Label(buildTargetRect, platform.PlatformType.ToString());

            GUIHelper.BeginEnabledOverride(!platform.IsActive);
            if (GUI.Button(buttonRect, "Activate"))
            {
                _activationDelegate = platform.Activate;
            }
            GUIHelper.EndEnabledOverride();
        }
    }
}
