#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Callbacks
{
    public sealed partial class EngineCallbacks : UnityEditor.AssetModificationProcessor
    {
        #region Asset Handling

        private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath,
            UnityEditor.RemoveAssetOptions options)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (asset is not ICallbackInterface)
            {
                return UnityEditor.AssetDeleteResult.DidNotDelete;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitPlayMode exitPlayModeCallback)
            {
                exitPlayModeListener.Remove(exitPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterPlayMode enterPlayModeCallback)
            {
                enterPlayModeListener.Remove(enterPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitEditMode exitEditModeCallback)
            {
                exitEditModeListener.Remove(exitEditModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterEditMode enterEditModeCallback)
            {
                enterEditModeListener.Remove(enterEditModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnAfterFirstSceneLoad afterLoadCallback)
            {
                afterFirstSceneLoadListener.Remove(afterLoadCallback);
            }

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion


        #region Fields

        private static readonly List<Action> exitPlayModeDelegate = new();
        private static readonly List<Action> enterPlayModeDelegate = new();
        private static readonly List<Action> exitEditModeDelegate = new();
        private static readonly List<Action> enterEditModeDelegate = new();

        private static readonly List<IOnExitPlayMode> exitPlayModeListener = new();
        private static readonly List<IOnEnterPlayMode> enterPlayModeListener = new();
        private static readonly List<IOnExitEditMode> exitEditModeListener = new();
        private static readonly List<IOnEnterEditMode> enterEditModeListener = new();

        #endregion


        #region Internal Subscribtions

        private static void AddOnExitPlayInternal(IOnExitPlayMode listener)
        {
            exitPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitPlayInternal(IOnExitPlayMode listener)
        {
            exitPlayModeListener.Remove(listener);
        }

        private static void AddOnEnterPlayInternal(IOnEnterPlayMode listener)
        {
            enterPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterPlayInternal(IOnEnterPlayMode listener)
        {
            enterPlayModeListener.Remove(listener);
        }

        private static void AddOnExitEditInternal(IOnExitEditMode listener)
        {
            exitEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitEditInternal(IOnExitEditMode listener)
        {
            exitEditModeListener.Remove(listener);
        }

        private static void AddOnEnterEditInternal(IOnEnterEditMode listener)
        {
            enterEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterEditInternal(IOnEnterEditMode listener)
        {
            enterEditModeListener.Remove(listener);
        }

        #endregion


        #region Play Mode State Changed

        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            EngineState = (int) state;
            switch (state)
            {
                case UnityEditor.PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode();
                    ManualInitializationCompleted = false;
                    break;
                case UnityEditor.PlayModeStateChange.ExitingEditMode:
                    OnExitEditMode();

                    break;
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode();

                    break;
                case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode();
                    ManualInitializationCompleted = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void OnExitPlayMode()
        {
            for (var i = 0; i < exitPlayModeListener.Count; i++)
            {
                if (exitPlayModeListener[i] == null)
                {
                    continue;
                }
                exitPlayModeListener[i].OnExitPlayMode();
            }

            for (var i = 0; i < exitPlayModeDelegate.Count; i++)
            {
                if (exitPlayModeDelegate[i] == null)
                {
                    continue;
                }
                exitPlayModeDelegate[i]();
            }
        }

        private static void OnEnterPlayMode()
        {
            for (var i = 0; i < enterPlayModeListener.Count; i++)
            {
                if (enterPlayModeListener[i] == null)
                {
                    continue;
                }
                enterPlayModeListener[i].OnEnterPlayMode();
            }

            for (var i = 0; i < enterPlayModeDelegate.Count; i++)
            {
                if (enterPlayModeDelegate[i] == null)
                {
                    continue;
                }
                enterPlayModeDelegate[i]();
            }
        }

        private static void OnExitEditMode()
        {
            for (var i = 0; i < exitEditModeListener.Count; i++)
            {
                if (exitEditModeListener[i] == null)
                {
                    continue;
                }
                exitEditModeListener[i].OnExitEditMode();
            }

            for (var i = 0; i < exitEditModeDelegate.Count; i++)
            {
                if (exitEditModeDelegate[i] == null)
                {
                    continue;
                }
                exitEditModeDelegate[i]();
            }
        }

        private static void OnEnterEditMode()
        {
            for (var i = 0; i < enterEditModeListener.Count; i++)
            {
                if (enterEditModeListener[i] == null)
                {
                    continue;
                }
                enterEditModeListener[i].OnEnterEditMode();
            }

            for (var i = 0; i < enterEditModeDelegate.Count; i++)
            {
                if (enterEditModeDelegate[i] == null)
                {
                    continue;
                }
                enterEditModeDelegate[i]();
            }
        }

        #endregion
    }
}
#endif