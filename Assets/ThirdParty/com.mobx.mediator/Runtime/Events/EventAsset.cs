using JetBrains.Annotations;
using MobX.Utilities.Inspector;
using System;

namespace MobX.Mediator.Events
{
    public abstract class EventAsset : EventAssetBase, IReceiver
    {
        private protected readonly IBroadcast Event = new Broadcast();

        public void Add([NotNull] Action listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique([NotNull] Action listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action listener)
        {
            return Event.Contains(listener);
        }

        [Button]
        [Foldout("Debug")]
        public void Clear()
        {
            Event.Clear();
        }

        [Button]
        [Foldout("Debug")]
        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        [Button]
        [Foldout("Debug")]
        public void Raise()
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise();
        }

        [ReadonlyInspector]
        [Foldout("Debug")]
        public int Count => Event.Count;
    }

    public abstract class EventAsset<T> : EventAssetBase, IReceiver<T>
    {
        public event Action<T> Invoked
        {
            add => Add(value);
            remove => Remove(value);
        }

        private protected readonly IBroadcast<T> Event = new Broadcast<T>();

        public void Add([NotNull] Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique([NotNull] Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T> listener)
        {
            return Event.Contains(listener);
        }

        [Button]
        [Foldout("Debug")]
        public void Clear()
        {
            Event.Clear();
        }

        [Button]
        [Foldout("Debug")]
        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        [Button]
        [Foldout("Debug")]
        public void Raise(T value)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value);
        }

        [ReadonlyInspector]
        [Foldout("Debug")]
        public int Count => Event.Count;
    }

    public abstract class EventAsset<T1, T2> : EventAssetBase, IReceiver<T1, T2>
    {
        private protected readonly IBroadcast<T1, T2> Event = new Broadcast<T1, T2>();

        public void Add([NotNull] Action<T1, T2> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique([NotNull] Action<T1, T2> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2> listener)
        {
            return Event.Contains(listener);
        }

        [Button]
        [Foldout("Debug")]
        public void Clear()
        {
            Event.Clear();
        }

        [Button]
        [Foldout("Debug")]
        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        [Button]
        [Foldout("Debug")]
        public void Raise(T1 value1, T2 value2)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2);
        }

        [ReadonlyInspector]
        [Foldout("Debug")]
        public int Count => Event.Count;
    }

    public abstract class EventAsset<T1, T2, T3> : EventAssetBase, IReceiver<T1, T2, T3>
    {
        private protected readonly IBroadcast<T1, T2, T3> Event = new Broadcast<T1, T2, T3>();

        public void Add([NotNull] Action<T1, T2, T3> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique([NotNull] Action<T1, T2, T3> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2, T3> listener)
        {
            return Event.Contains(listener);
        }

        [Button]
        [Foldout("Debug")]
        public void Clear()
        {
            Event.Clear();
        }

        [Button]
        [Foldout("Debug")]
        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        [Button]
        [Foldout("Debug")]
        public void Raise(T1 value1, T2 value2, T3 value3)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2, value3);
        }

        [ReadonlyInspector]
        [Foldout("Debug")]
        public int Count => Event.Count;
    }

    public abstract class EventAsset<T1, T2, T3, T4> : EventAssetBase, IReceiver<T1, T2, T3, T4>
    {
        private protected readonly IBroadcast<T1, T2, T3, T4> Event = new Broadcast<T1, T2, T3, T4>();

        public void Add([NotNull] Action<T1, T2, T3, T4> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Add(listener);
        }

        public bool AddUnique([NotNull] Action<T1, T2, T3, T4> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.AddUnique(listener);
        }

        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return false;
            }
#endif
            return Event.Remove(listener);
        }

        public bool Contains(Action<T1, T2, T3, T4> listener)
        {
            return Event.Contains(listener);
        }

        [Button]
        [Foldout("Debug")]
        public void Clear()
        {
            Event.Clear();
        }

        [Button]
        [Foldout("Debug")]
        public void ClearInvalid()
        {
            Event.ClearInvalid();
        }

        [Button]
        [Foldout("Debug")]
        public void Raise(T1 value1, T2 value2, T3 value3, T4 value4)
        {
#if UNITY_EDITOR
            if (IsIllegalCall(this))
            {
                return;
            }
#endif
            Event.Raise(value1, value2, value3, value4);
        }

        [ReadonlyInspector]
        [Foldout("Debug")]
        public int Count => Event.Count;
    }
}