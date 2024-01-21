using MobX.Mediator.Events;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Mediator.Values
{
    /// <summary>
    ///     Scriptable object holding a value that can be accessed and set during runtime.
    /// </summary>
    public abstract class SerializedValueAsset<TValue> : ValueAsset<TValue>, IOnExitEditMode, IOnEnterEditMode
    {
        [SerializeField] private TValue value;

        [ReadonlyInspector]
#pragma warning disable 414
        [SerializeField] private TValue cached;

        private readonly Broadcast<TValue> _changedEvent = new();

        /// <summary>
        ///     Event, called every time the value changed.
        /// </summary>
        public sealed override event Action<TValue> Changed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            add => _changedEvent.Add(value);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            remove => _changedEvent.Remove(value);
        }

        /// <summary>
        ///     Get or set the underlying value.
        /// </summary>
        public sealed override TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                _changedEvent.Raise(value);
            }
        }

        public override void SetValue(TValue newValue)
        {
            Value = newValue;
        }

        public override TValue GetValue()
        {
            return Value;
        }

        public static implicit operator TValue(SerializedValueAsset<TValue> serializedValueAsset)
        {
            return serializedValueAsset.value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if !UNITY_EDITOR
            // Set the cached value to a default value (null for reference types) to release references.
            cached = default;
#endif
        }

        public void OnExitEditMode()
        {
#if UNITY_EDITOR
            UpdateCached();
#endif
        }

        public void OnEnterEditMode()
        {
#if UNITY_EDITOR
            ResetValue();
#endif
        }

#if UNITY_EDITOR
        [SpaceBefore]
        [Button(ButtonType.Center)]
        [Tooltip("Reset the current value to the cached value")]
        private void ResetValue()
        {
            SetValue(cached);
        }

        private void UpdateCached()
        {
            cached = Value;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                cached = Value;
            }
            else
            {
                _changedEvent.Raise(value);
            }
        }
#endif
    }
}
