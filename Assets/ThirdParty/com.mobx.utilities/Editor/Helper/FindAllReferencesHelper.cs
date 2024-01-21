using System.IO;
using UnityEngine;

namespace MobX.Utilities.Editor.Helper
{
    [UnityEditor.InitializeOnLoadAttribute]
    public class FindAllReferencesHelper : UnityEditor.AssetPostprocessor
    {
        private static readonly string cachePath;

        static FindAllReferencesHelper()
        {
            cachePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/FindAllReferences"));
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
                BuildCaches();
                UnityEngine.Debug.Log("[FindAllReferences] Caches Path: " + cachePath);
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                var ext = Path.GetExtension(path);
                if (ext is ".prefab" or ".unity" or ".mat" or ".controller" or ".asset")
                {
                    var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                    var dependencies = UnityEditor.AssetDatabase.GetDependencies(path);
                    CreateCache(guid, dependencies);
                }
            }

            foreach (var path in deletedAssets)
            {
                var cache = Path.Combine(cachePath, Path.GetFileName(path));
                if (File.Exists(cache))
                {
                    File.Delete(cache);
                }
            }
        }

        public static string[] GetDependencies(string guid)
        {
            if (LoadCache(guid, out var dependencies))
            {
                return dependencies;
            }

            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            dependencies = UnityEditor.AssetDatabase.GetDependencies(assetPath, false);

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            CreateCache(guid, dependencies);

            return dependencies;
        }

        private static void BuildCaches()
        {
            UnityEditor.EditorUtility.DisplayProgressBar("[FindAllReferences] - Building caches ...", "To path: " + cachePath, 0f);

            var filter = "t:Prefab t:Scene t:ScriptableObject t:Material t:AnimatorController";
            var guids = UnityEditor.AssetDatabase.FindAssets(filter);

            for (var i = 0; i < guids.Length; i++)
            {
                CreateCache(guids[i]);
                UnityEditor.EditorUtility.DisplayProgressBar("[FindAllReferences] - Building caches ...", "To path: " + cachePath, 1f * i / guids.Length);
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

        public static bool LoadCache(string guid, out string[] dependencies)
        {
            var cachePath = Path.Combine(FindAllReferencesHelper.cachePath, guid);
            if (!File.Exists(cachePath))
            {
                dependencies = null;
                return false;
            }

            var sr = new StreamReader(cachePath);
            dependencies = sr.ReadLine()?.Split('|');
            sr.Close();

            return true;
        }

        public static void CreateCache(string guid)
        {
            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            CreateCache(guid, UnityEditor.AssetDatabase.GetDependencies(assetPath, false));
        }

        public static void CreateCache(string guid, string[] dependencies)
        {
            var sw = new StreamWriter(Path.Combine(cachePath, guid));
            for (var i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = UnityEditor.AssetDatabase.AssetPathToGUID(dependencies[i]);
            }
            sw.WriteLine(string.Join("|", dependencies));
            sw.Close();
        }
    }
}
