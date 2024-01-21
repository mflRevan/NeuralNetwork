using MobX.Utilities.Editor.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.AssetManagement
{
    public class LabelWindow : UnityEditor.EditorWindow
    {
        private GUIToggle _includeNested;
        private Vector2 _scrollPosition;
        private AssetData[] _filteredSelection = Array.Empty<AssetData>();


        #region Type Definition

        private readonly struct AssetData
        {
            public readonly Object Asset;
            public readonly string[] Labels;
            public readonly string Path;
            public readonly string LabelRepresentation;
            public readonly AssetData[] SubAssets;

            public AssetData(Object asset, bool includeFolder)
            {
                Asset = asset;
                Labels = UnityEditor.AssetDatabase.GetLabels(asset);
                LabelRepresentation = Labels.CombineToString(", ");
                Path = UnityEditor.AssetDatabase.GetAssetPath(asset);

                if (includeFolder)
                {
                    List<AssetData> buffer = ListPool<AssetData>.Get();
                    if (UnityEditor.AssetDatabase.IsValidFolder(Path))
                    {
                        var files = UnityEditor.AssetDatabase.FindAssets("t:Object", new[]
                        {
                            Path
                        });
                        foreach (var file in files)
                        {
                            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(file);
                            Object subAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                            buffer.Add(new AssetData(subAsset, true));
                        }
                    }

                    SubAssets = buffer.ToArray();
                    ListPool<AssetData>.Release(buffer);
                    return;
                }

                SubAssets = Array.Empty<AssetData>();
            }
        }

        #endregion


        [UnityEditor.MenuItem("Tools/Asset Labels")]
        private static void Open()
        {
            GetWindow<LabelWindow>("Asset Labels");
        }

        private void OnEnable()
        {
            _includeNested = new GUIToggle("Include Nested", true);
            _includeNested.Changed += _ => UpdateSelection();
            _includeNested.LoadValue(nameof(_includeNested));
            UnityEditor.Selection.selectionChanged += UpdateSelection;
            UpdateSelection();
        }

        private void OnDisable()
        {
            _includeNested.Dispose();
            _includeNested.SaveValue(nameof(_includeNested));
            UnityEditor.Selection.selectionChanged -= UpdateSelection;
        }

        private void UpdateSelection()
        {
            List<AssetData> buffer = ListPool<AssetData>.Get();
            foreach (Object asset in UnityEditor.Selection.objects)
            {
                if (UnityEditor.AssetDatabase.Contains(asset) is false)
                {
                    continue;
                }

                buffer.Add(new AssetData(asset, _includeNested));
            }

            _filteredSelection = buffer.ToArray();
            ListPool<AssetData>.Release(buffer);
            Repaint();
        }

        private string _labelString;

        private void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            DrawAssetData(_filteredSelection);
            GUILayout.EndScrollView();
            GUIHelper.DrawLine();
            _includeNested.Draw();
            _labelString = UnityEditor.EditorGUILayout.TextField(_labelString);
            if (GUILayout.Button("Add"))
            {
                AddLabel(_labelString, _filteredSelection);
                UpdateSelection();
            }
        }

        private static void AddLabel(string label, AssetData[] assets)
        {
            foreach (AssetData assetData in assets)
            {
                var labels = assetData.Labels;
                ArrayUtility.Add(ref labels, label);
                UnityEditor.AssetDatabase.SetLabels(assetData.Asset, labels);

                AddLabel(label, assetData.SubAssets);
            }
        }

        private static void DrawAssetData(AssetData[] assets)
        {
            foreach (AssetData assetData in assets)
            {
                Object asset = assetData.Asset;
                var assetName = asset.name;
                var labels = assetData.LabelRepresentation;

                GUILayout.BeginHorizontal();
                UnityEditor.EditorGUILayout.LabelField(assetName);
                UnityEditor.EditorGUILayout.LabelField(labels);
                GUILayout.EndHorizontal();

                DrawSubAssets(assetData.SubAssets);
            }

            static void DrawSubAssets(AssetData[] subAssets)
            {
                GUIHelper.IncreaseIndent();
                DrawAssetData(subAssets);
                GUIHelper.DecreaseIndent();
            }
        }
    }
}
