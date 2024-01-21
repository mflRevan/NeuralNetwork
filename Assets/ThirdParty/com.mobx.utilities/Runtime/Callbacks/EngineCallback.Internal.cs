using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    public partial class EngineCallbacks
    {
        #region Update Callbacks

        public static int EngineState { get; private set; }
        public static bool IsQuitting { get; private set; }
        public static bool ManualInitializationCompleted { get; private set; }

        private const int DefaultCapacity = 16;

        private static readonly List<IOnUpdate> updateListener = new(DefaultCapacity);
        private static readonly List<IOnLateUpdate> lateUpdateListener = new(DefaultCapacity);
        private static readonly List<IOnFixedUpdate> fixedUpdateListener = new(DefaultCapacity);

        private static readonly List<UpdateDelegate> updateDelegates = new(DefaultCapacity);
        private static readonly List<LateUpdateDelegate> lateUpdateDelegates = new(DefaultCapacity);
        private static readonly List<FixedUpdateDelegate> fixedUpdateDelegates = new(DefaultCapacity);

        private static readonly List<IOnBeforeFirstSceneLoad> beforeSceneLoadListener = new(DefaultCapacity);
        private static readonly List<IOnQuit> quitListener = new(DefaultCapacity);
        private static readonly List<IOnAfterFirstSceneLoad> afterFirstSceneLoadListener = new();
        private static readonly List<IOnInitializationCompleted> initializationCompletedListener = new();
        private static readonly List<Action> beforeSceneLoadDelegates = new(DefaultCapacity);
        private static readonly List<Action> quitDelegates = new(DefaultCapacity);

        private static bool beforeSceneLoadCompleted;
        private static bool afterSceneLoadCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetupUpdateCallbacks()
        {
            IsQuitting = false;
            RuntimeHook.Create(OnUpdate, OnLateUpdate, OnFixedUpdate, OnQuit);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
#if DEBUG
            foreach (var listener in beforeSceneLoadListener)
            {
                try
                {
                    listener.OnBeforeFirstSceneLoad();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            foreach (var listener in beforeSceneLoadDelegates)
            {
                try
                {
                    listener();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in beforeSceneLoadListener)
            {
                listener.OnBeforeFirstSceneLoad();
            }

            foreach (var listener in beforeSceneLoadDelegates)
            {
                listener();
            }
#endif
            beforeSceneLoadCompleted = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
#if DEBUG
            foreach (var listener in afterFirstSceneLoadListener)
            {
                try
                {
                    listener.OnAfterFirstSceneLoad();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            foreach (var listener in afterFirstSceneLoadListener)
            {
                listener.OnAfterFirstSceneLoad();
            }
#endif
            afterSceneLoadCompleted = true;
        }

        private static void OnQuit()
        {
            IsQuitting = true;
            beforeSceneLoadCompleted = false;
            afterSceneLoadCompleted = false;
#if DEBUG
            for (var index = quitListener.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = quitListener[index];
                    listener.OnQuit();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            for (var index = quitDelegates.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = quitDelegates[index];
                    listener();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            for (var index = quitListener.Count - 1; index >= 0; index--)
            {
                var listener = quitListener[index];
                listener.OnQuit();
            }

            for (var index = quitDelegates.Count - 1; index >= 0; index--)
            {
                var listener = quitDelegates[index];
                listener();
            }
#endif
        }

        private static void OnUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            for (var index = updateListener.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = updateListener[index];
                    listener.OnUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            for (var index = updateDelegates.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = updateDelegates[index];
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            for (var index = updateListener.Count - 1; index >= 0; index--)
            {
                var listener = updateListener[index];
                listener.OnUpdate(deltaTime);
            }

            for (var index = updateDelegates.Count - 1; index >= 0; index--)
            {
                var listener = updateDelegates[index];
                listener(deltaTime);
            }
#endif
        }

        private static void OnLateUpdate()
        {
#if DEBUG
            var deltaTime = Time.deltaTime;
            for (var index = lateUpdateListener.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = lateUpdateListener[index];
                    listener.OnLateUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            for (var index = lateUpdateDelegates.Count - 1; index >= 0; index--)
            {
                try
                {
                    var listener = lateUpdateDelegates[index];
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.deltaTime;
            for (var index = lateUpdateListener.Count - 1; index >= 0; index--)
            {
                var listener = lateUpdateListener[index];
                listener.OnLateUpdate(deltaTime);
            }

            for (var index = lateUpdateDelegates.Count - 1; index >= 0; index--)
            {
                var listener = lateUpdateDelegates[index];
                listener(deltaTime);
            }
#endif
        }

        private static void OnFixedUpdate()
        {
#if DEBUG
            var deltaTime = Time.fixedDeltaTime;
            for (var index = 0; index < fixedUpdateListener.Count; index++)
            {
                try
                {
                    var listener = fixedUpdateListener[index];
                    listener.OnFixedUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            for (var index = 0; index < fixedUpdateDelegates.Count; index++)
            {
                try
                {
                    var listener = fixedUpdateDelegates[index];
                    listener(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
#else
            var deltaTime = Time.fixedDeltaTime;
            for (var index = fixedUpdateListener.Count - 1; index >= 0; index--)
            {
                var listener = fixedUpdateListener[index];
                listener.OnFixedUpdate(deltaTime);
            }

            for (var index = fixedUpdateDelegates.Count - 1; index >= 0; index--)
            {
                var listener = fixedUpdateDelegates[index];
                listener(deltaTime);
            }
#endif
        }

        #endregion
    }
}
