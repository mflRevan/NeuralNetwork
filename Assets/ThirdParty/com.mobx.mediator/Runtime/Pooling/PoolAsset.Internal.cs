using Cysharp.Threading.Tasks;
using MobX.Utilities;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Types;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace MobX.Mediator.Pooling
{
    public partial class PoolAsset<T>
    {
        #region Runtime Callbacks

        public void OnAfterFirstSceneLoad()
        {
#if UNITY_EDITOR
            if (warmupOnBeginPlay)
            {
                Load();
            }
#endif
        }

        public void OnQuit()
        {
            Dispose();
        }

        #endregion


        #region Warup And Refresh

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadInternal()
        {
            if (State != PoolState.Unloaded)
            {
                return;
            }
            State = PoolState.Loading;

            var buffer = ListPool<T>.Get();

            if (Parent == null)
            {
                Parent = PoolTransform.Create(this);
            }

            if (multiple && selectionMode == SelectionMode.RoundRobin)
            {
                _prefabIndex = Loop.Create(prefabs);
            }

            for (var i = 0; i < initialPoolSize; i++)
            {
                buffer.Add(GetInternal(true));
            }

            foreach (var element in buffer)
            {
                ReleaseInternal(element);
            }

            ListPool<T>.Release(buffer);
            State = PoolState.Loaded;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RefreshInternal()
        {
            var instance = GetInternal(true);
            Release(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DisposeInternal()
        {
            ClearInternal();
            State = PoolState.Unloaded;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearInternal()
        {
            AssertIsPlaying();
            foreach (var instance in _pool)
            {
                OnDestroyInstance(instance);
            }
            _pool.Clear();
            CountAll = 0;
            State = PoolState.Unloaded;
        }

        #endregion


        #region Get And Release

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetInternal(bool discrete)
        {
            AssertIsPlaying();

            if (State == PoolState.Unloaded)
            {
                Load();
            }

            T instance;
            if (_pool.Count == 0)
            {
                if (_activeItems.Count >= MaxPoolSize)
                {
                    instance = _activeItems[0];
                    _activeItems.RemoveAt(0);
                    OnReleaseInstance(instance);
                }
                else
                {
                    instance = CreateInstance();
                    ++CountAll;
                }
            }
            else
            {
                switch (selectionMode)
                {
                    case SelectionMode.RoundRobin:
                        var index = _pool.Count - 1;
                        instance = _pool[index];
                        _pool.RemoveAt(index);
                        break;
                    case SelectionMode.Random:
                        instance = _pool.RandomItemRemove();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (instance == null)
            {
                Debug.LogError("Pooling", $"Requested Instance from {this} was null!", this);
                instance = CreateInstance();
                ++CountAll;
            }

            OnGetInstance(instance);
            if (!discrete && autoRelease)
            {
                UniTask.Delay(TimeSpan.FromSeconds(lifeSpanInSeconds))
                    .ContinueWith(() => { ReleaseInternal(instance); });
            }

            _activeItems.Add(instance);
            UpdateInspector();
            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseInternal(T instance)
        {
            AssertIsPlaying();
            if (EngineCallbacks.IsQuitting)
            {
                return;
            }
            if (instance == null)
            {
                Debug.LogError("Pooling", "Released Instance was null!");
                return;
            }

            if (_pool.Contains(instance))
            {
                Debug.Log("Pooling", $"Released Instance [{instance}] was already released to the pool!", instance);
                return;
            }

            OnReleaseInstance(instance);
            _activeItems.Remove(instance);

            if (CountInactive < MaxPoolSize)
            {
                _pool.Add(instance);
            }
            else
            {
                Debug.Log("Pooling",
                    $"Pool [{name}] reached its max allowed capacity! Destroying released instance: [{instance}] ",
                    LogOption.NoStacktrace);
                OnDestroyInstance(instance);
            }

            UpdateInspector();
        }

        #endregion


        #region Assertions And Obsolete

        [Obsolete("This method is Obsolete and will throw an InvalidOperationException")]
        public PooledObject<T> Get(out T instance)
        {
            throw new InvalidOperationException("Invalid method call!");
        }

        #endregion


        #region Editor

        [Conditional("UNITY_EDITOR")]
        private static void AssertIsPlaying()
        {
            Assert.IsTrue(Application.isPlaying, "Application Is Not Playing!");
        }

        [Conditional("UNITY_EDITOR")]
        private void UpdateInspector()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        #endregion
    }
}
