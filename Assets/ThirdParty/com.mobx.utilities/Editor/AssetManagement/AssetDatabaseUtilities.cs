using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.AssetManagement
{
    public class AssetDatabaseUtilities : MonoBehaviour
    {
        public static string[] GUIDsToPaths(string[] guids)
        {
            var paths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return paths;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2>() where T1 : Object where T2 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3>() where T1 : Object where T2 : Object where T3 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3, T4>() where T1 : Object where T2 : Object where T3 : Object where T4 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            list.AddRange(FindAssetsOfType<T4>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3, T4, T5>() where T1 : Object where T2 : Object where T3 : Object where T4 : Object where T5 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            list.AddRange(FindAssetsOfType<T4>());
            list.AddRange(FindAssetsOfType<T5>());
            return list;
        }

        [Pure]
        public static List<T> FindAssetsOfType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        [Pure]
        public static string[] GetAssetPathsOfType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            var paths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return paths;
        }

        /*
         * Non Generic
         */

        [Pure]
        public static List<ScriptableObject> FindAssetsOfType(Type objectType)
        {
            var assets = new List<ScriptableObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{objectType}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        [Pure]
        public static List<Component> FindPrefabsOfType(Type componentType)
        {
            var assets = new List<Component>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponents(componentType, out Component[] components))
                {
                    assets.AddRange(components);
                }
            }

            return assets;
        }

        public static string[] FindAllScenePaths()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");
            var scenePaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                scenePaths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return scenePaths;
        }

        [Pure]
        public static string[] GetAssetPaths(IList<Object> assets)
        {
            var result = new string[assets.Count];
            for (var i = 0; i < assets.Count; i++)
            {
                result[i] = UnityEditor.AssetDatabase.GetAssetPath(assets[i]);
            }
            return result;
        }

        [Pure]
        public static string[] GetAssetPaths(ICollection<Object> assets)
        {
            var result = new string[assets.Count];
            var index = 0;
            foreach (Object asset in assets)
            {
                result[index] = UnityEditor.AssetDatabase.GetAssetPath(asset);
                index++;
            }
            return result;
        }
    }
}
