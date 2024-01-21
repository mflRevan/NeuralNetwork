using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Singleton
{
    [CreateAssetMenu]
    public sealed class Singletons : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Data

        public static bool IsLoaded => singleton != null;

#pragma warning disable 414
        private bool _enableEditing;
#pragma warning restore

        [ConditionalShow(nameof(_enableEditing), ReadOnly = true)]
        [ListOptions]
        [SerializeField] private List<Object> registry;

        #endregion


        #region Public

        /// <summary>
        ///     Register a singleton object. The object is then cached persistently and can be resolved with by its type.
        /// </summary>
        public static void Register<T>(T instance) where T : Object
        {
            RegisterInternal(instance);
        }

        /// <summary>
        ///     Get the singleton instance for T. Use <see cref="Exists{T}" /> to check if an instance is registered.
        /// </summary>
        /// <typeparam name="T">The type of the singleton instance to resolve</typeparam>
        /// <returns>The singleton instance of T</returns>
        public static T Resolve<T>() where T : Object
        {
            return ResolveInternal<T>();
        }

        public static bool Exists<T>()
        {
            return ExistsInternal<T>();
        }

        #endregion


        #region Internal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RegisterInternal<T>(T instance) where T : Object
        {
#if UNITY_EDITOR
            async static Task WaitWhile(Func<bool> condition)
            {
                while (condition())
                {
                    await Task.Delay(25);
                }
            }

            static bool IsImport() => UnityEditor.EditorApplication.isCompiling || UnityEditor.EditorApplication.isUpdating;

            if (IsImport())
            {
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                WaitWhile(IsImport).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        return;
                    }
                    Register(instance);
                }, scheduler);
                return;
            }
#endif
            if (IsLoaded is false)
            {
                Debug.LogError("Singleton", $"Singleton Registry is not loaded yet! Cannot register instance for {typeof(T)}", instance);
                return;
            }

            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                var entry = Singleton.registry[i];
                if (entry != null && entry is T)
                {
                    Singleton.registry[i] = instance;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(Singleton);
#endif
                    return;
                }
            }

            Singleton.registry.Add(instance);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(Singleton);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T ResolveInternal<T>() where T : Object
        {
            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                var element = Singleton.registry[i];
                if (element != null && element is T instance)
                {
                    return instance;
                }
            }

            Debug.LogError("Singleton", $"No singleton instance of type {typeof(T)} registered!", Singleton);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ExistsInternal<T>()
        {
            if (Singleton.registry == null)
            {
                Debug.LogError("Singleton", $"Registry is null! Attempted to access singleton for {typeof(T)}");
                return false;
            }

            for (var i = 0; i < Singleton.registry.Count; i++)
            {
                var entry = Singleton.registry[i];

                if (entry == null)
                {
                    continue;
                }

                if (entry.GetType() is T)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Singleton

        private static Singletons singleton;

        public static Singletons Singleton
        {
            get
            {
                // In the editor we load the singleton from the asset database.
#if UNITY_EDITOR
                if (singleton != null)
                {
                    return singleton;
                }

                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(Singletons)}");
                for (var i = 0; i < guids.Length; i++)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    singleton = UnityEditor.AssetDatabase.LoadAssetAtPath<Singletons>(path);
                    if (singleton != null)
                    {
                        break;
                    }
                }
                if (singleton == null)
                {
                    Debug.LogError("Singleton",
                        "Singleton Registry is null! Please create a new Singleton Registry (ScriptableObject)");
                }
#endif
                return singleton;
            }
        }

        #endregion


        #region Serialization

        private void OnEnable()
        {
            singleton = this;
        }

        public void OnAfterDeserialize()
        {
            singleton = this;
        }

        public void OnBeforeSerialize()
        {
        }

        #endregion


        #region Editor

        [Button]
        [SpaceBefore]
        [Tooltip("Remove null objects from the registry")]
        private void ClearInvalid()
        {
            registry.RemoveNull();
        }

        [Button]
        [ConditionalShow(nameof(_enableEditing), false)]
        private void EnableEdit()
        {
            _enableEditing = true;
        }

        [Button]
        [ConditionalShow(nameof(_enableEditing))]
        private void DisableEdit()
        {
            _enableEditing = false;
        }

        #endregion
    }
}
