using JetBrains.Annotations;
using System;
using System.Diagnostics;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public partial class EngineCallbacks
    {
        #region Initialization

        static EngineCallbacks()
        {
            Application.quitting += () => IsQuitting = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
#endif
        }

        #endregion


        #region Callbacks

        public static void AddCallbacks<T>(T listener) where T : class
        {
            if (listener is not ICallbackInterface)
            {
                return;
            }

            if (listener is IOnUpdate onUpdate)
            {
                AddUpdateListener(onUpdate);
            }

            if (listener is IOnLateUpdate onLateUpdate)
            {
                AddLateUpdateListener(onLateUpdate);
            }

            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                AddFixedUpdateListener(onFixedUpdate);
            }

            if (listener is IOnQuit onQuit)
            {
                AddOnQuitListener(onQuit);
            }

            if (listener is IOnBeforeFirstSceneLoad onBeforeFirstSceneLoad)
            {
                AddBeforeFirstSceneLoadListener(onBeforeFirstSceneLoad);
            }

            if (listener is IOnAfterFirstSceneLoad onAfterFirstSceneLoad)
            {
                AddAfterFirstSceneLoadListener(onAfterFirstSceneLoad);
            }

            if (listener is IOnInitializationCompleted onInitializationCompleted)
            {
                AddInitializationCompletedListener(onInitializationCompleted);
            }

#if UNITY_EDITOR
            AddOnExitPlayInternal(listener as IOnExitPlayMode);
            AddOnEnterPlayInternal(listener as IOnEnterPlayMode);
            AddOnExitEditInternal(listener as IOnExitEditMode);
            AddOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        public static void RemoveCallbacks<T>(T listener) where T : class
        {
            if (listener is not ICallbackInterface)
            {
                return;
            }

            if (listener is IOnUpdate onUpdate)
            {
                RemoveUpdateListener(onUpdate);
            }

            if (listener is IOnLateUpdate onLateUpdate)
            {
                RemoveLateUpdateListener(onLateUpdate);
            }

            if (listener is IOnFixedUpdate onFixedUpdate)
            {
                RemoveFixedUpdateListener(onFixedUpdate);
            }

            if (listener is IOnQuit onQuit)
            {
                RemoveQuitListener(onQuit);
            }

            if (listener is IOnBeforeFirstSceneLoad onBeforeFirstSceneLoad)
            {
                RemoveBeforeFirstSceneLoadListener(onBeforeFirstSceneLoad);
            }

            if (listener is IOnAfterFirstSceneLoad onAfterFirstSceneLoad)
            {
                RemoveAfterFirstSceneLoadListener(onAfterFirstSceneLoad);
            }

            if (listener is IOnInitializationCompleted onInitializationCompleted)
            {
                RemoveInitializationCompletedListener(onInitializationCompleted);
            }

#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener as IOnExitPlayMode);
            RemoveOnEnterPlayInternal(listener as IOnEnterPlayMode);
            RemoveOnExitEditInternal(listener as IOnExitEditMode);
            RemoveOnEnterEditInternal(listener as IOnEnterEditMode);
#endif
        }

        #endregion


        #region Before First Scene Loaded

        [PublicAPI]
        public static void AddBeforeFirstSceneLoadListener<T>(T listener) where T : class, IOnBeforeFirstSceneLoad
        {
            if (beforeSceneLoadCompleted)
            {
                listener.OnBeforeFirstSceneLoad();
            }

            beforeSceneLoadListener.AddUnique(listener, true);
        }

        [PublicAPI]
        public static void RemoveBeforeFirstSceneLoadListener<T>(T listener) where T : class, IOnBeforeFirstSceneLoad
        {
            beforeSceneLoadListener.Remove(listener);
        }

        #endregion


        #region After First Scene Loaded

        [PublicAPI]
        public static void AddAfterFirstSceneLoadListener<T>(T listener) where T : class, IOnAfterFirstSceneLoad
        {
            if (afterSceneLoadCompleted)
            {
                listener.OnAfterFirstSceneLoad();
            }
            afterFirstSceneLoadListener.AddUnique(listener, true);
        }

        [PublicAPI]
        public static void RemoveAfterFirstSceneLoadListener<T>(T listener) where T : class, IOnAfterFirstSceneLoad
        {
            afterFirstSceneLoadListener.Remove(listener);
        }

        #endregion


        #region On Quit

        [PublicAPI]
        public static void AddOnQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitListener.AddUnique(listener, true);
        }

        [PublicAPI]
        public static void RemoveQuitListener<T>(T listener) where T : class, IOnQuit
        {
            quitListener.Remove(listener);
        }

        #endregion


        #region After Initialization

        public static void NotifyInitializationCompleted()
        {
            if (ManualInitializationCompleted)
            {
                Debug.Log("Engine Callbacks", $"{nameof(NotifyInitializationCompleted)} has already been invoked!");
                return;
            }

            ManualInitializationCompleted = true;

            foreach (var onInitializationCompleted in initializationCompletedListener)
            {
                try
                {
                    onInitializationCompleted.OnInitializationCompleted();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        [PublicAPI]
        public static void AddInitializationCompletedListener<T>(T listener) where T : class, IOnInitializationCompleted
        {
            if (ManualInitializationCompleted)
            {
                listener.OnInitializationCompleted();
            }
            initializationCompletedListener.AddUnique(listener, true);
        }

        [PublicAPI]
        public static void RemoveInitializationCompletedListener<T>(T listener)
            where T : class, IOnInitializationCompleted
        {
            initializationCompletedListener.Remove(listener);
        }

        #endregion


        #region Update Callbacks

        public static void AddUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateListener.AddUnique(listener, true);
        }

        public static void AddLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateListener.AddUnique(listener, true);
        }

        public static void AddFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateListener.AddUnique(listener, true);
        }

        public static void RemoveUpdateListener<T>(T listener) where T : class, IOnUpdate
        {
            updateListener.Remove(listener);
        }

        public static void RemoveLateUpdateListener<T>(T listener) where T : class, IOnLateUpdate
        {
            lateUpdateListener.Remove(listener);
        }

        public static void RemoveFixedUpdateListener<T>(T listener) where T : class, IOnFixedUpdate
        {
            fixedUpdateListener.Remove(listener);
        }

        #endregion


        #region Add State Callbacks

        [Conditional("UNITY_EDITOR")]
        public static void AddExitPlayModeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            AddOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterPlayModeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            AddOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            AddOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            AddOnEnterEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitPlaymodeListener<T>(T listener) where T : class, IOnExitPlayMode
        {
#if UNITY_EDITOR
            RemoveOnExitPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterPlaymodeListener<T>(T listener) where T : class, IOnEnterPlayMode
        {
#if UNITY_EDITOR
            RemoveOnEnterPlayInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveExitEditModeListener<T>(T listener) where T : class, IOnExitEditMode
        {
#if UNITY_EDITOR
            RemoveOnExitEditInternal(listener);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RemoveEnterEditModeListener<T>(T listener) where T : class, IOnEnterEditMode
        {
#if UNITY_EDITOR
            RemoveOnEnterEditInternal(listener);
#endif
        }

        #endregion
    }
}