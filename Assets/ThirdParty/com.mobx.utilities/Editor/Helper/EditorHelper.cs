using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.Helper
{
    [UnityEditor.InitializeOnLoadAttribute]
    public static class EditorHelper
    {
        private static readonly MethodInfo activeFolderPathMethod = typeof(UnityEditor.ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

        public static string GetCurrentAssetDirectory()
        {
            return activeFolderPathMethod.Invoke(null, Array.Empty<object>()).ToString();
        }

        private static UnityEditor.Compilation.Assembly[] UnityAssemblies { get; }

        static EditorHelper()
        {
            UnityAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
        }

        public static bool IsEditorAssembly(this Assembly assembly)
        {
            UnityEditor.Compilation.Assembly[] editorAssemblies = UnityAssemblies;

            for (var i = 0; i < editorAssemblies.Length; i++)
            {
                UnityEditor.Compilation.Assembly unityAssembly = editorAssemblies[i];

                if (unityAssembly.name != assembly.GetName().Name)
                {
                    continue;
                }

                if (unityAssembly.flags.HasFlag(UnityEditor.Compilation.AssemblyFlags.EditorAssembly))
                {
                    return true;
                }
            }

            return false;
        }

        /*
         *  Asset Database
         */

        public static bool IsLocatedInResources(this Object asset)
        {
            return UnityEditor.AssetDatabase.GetAssetPath(asset).Contains("/Resources");
        }

        public static bool TryFindPrefabsOfType<TComponent>(out List<TComponent> prefabs) where TComponent : Component
        {
            prefabs = FindPrefabsOfType<TComponent>();
            return prefabs.IsNotNullOrEmpty();
        }

        public static List<TComponent> FindPrefabsOfType<TComponent>() where TComponent : Component
        {
            var assets = new List<TComponent>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject.IsNotNull() && gameObject.TryGetComponents<TComponent>(out TComponent[] components))
                {
                    assets.AddRange(components);
                }
            }

            return assets;
        }

        public static List<GameObject> FindPrefabsOfTypeAsGameObjects<TComponent>() where TComponent : Component
        {
            var assets = new List<GameObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponent<TComponent>(out _))
                {
                    assets.Add(gameObject);
                }
            }

            return assets;
        }

        public static List<Component> FindPrefabsWithGenericInterface(Type interfaceType)
        {
            var assets = new List<Component>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                Component[] components = gameObject.GetComponents<Component>();
                for (var j = 0; j < components.Length; j++)
                {
                    Component component = components[j];
                    if (!component)
                    {
                        continue;
                    }

                    if (components[j].GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                    {
                        assets.Add(components[j]);
                    }
                }
            }

            return assets;
        }

        public static List<GameObject> FindPrefabsWithGenericInterfaceAsGameObject(Type interfaceType)
        {
            var assets = new List<GameObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                Component[] components = gameObject.GetComponents<Component>();

                for (var j = 0; j < components.Length; j++)
                {
                    Component component = components[j];
                    if (!component)
                    {
                        continue;
                    }

                    if (components[j].GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                    {
                        assets.Add(gameObject);
                    }
                }
            }

            return assets;
        }

        public static bool TryFindAssetsOfType<T>(out List<T> assets) where T : Object
        {
            assets = FindAssetsOfType<T>();
            return assets.IsNotNullOrEmpty();
        }

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

        public static List<T> FindAssetsOfTypeWithInterface<T>(Type interfaceType) where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && asset.GetType().HasInterface(interfaceType))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<T> FindAssetsOfTypeWithGenericInterface<T>(Type interfaceType) where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && asset.GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<Object> FindAssetsOfType(Type type)
        {
            var assets = new List<Object>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{type}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static ScriptableObject CreateScriptableObjectAsset(Type type)
        {
            if (type.IsGenericType)
            {
                UnityEngine.Debug.LogWarning("Cannot create an asset for a generic type!");
                return null;
            }

            var so = ScriptableObject.CreateInstance(type);
            var filePath = GetCurrentAssetDirectory();
            var fileName = type.Name;
            var completePath =
                $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}.asset";
            var index = 1;
            while (File.Exists(completePath))
            {
                completePath =
                    $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}{index++.ToString()}.asset";
            }

            UnityEditor.AssetDatabase.CreateAsset(so, completePath);

            return so;
        }

        /*
         * Delete
         */

        public static void DeleteAsset(Object asset)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            UnityEditor.AssetDatabase.DeleteAsset(path);
        }

        /*
         * Selection
         */

        public static void SelectObject(Object target)
        {
            UnityEditor.Selection.activeObject = target;
            UnityEditor.EditorGUIUtility.PingObject(target);
        }
    }
}
