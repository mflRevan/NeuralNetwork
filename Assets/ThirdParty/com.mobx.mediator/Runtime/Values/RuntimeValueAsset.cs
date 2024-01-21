using MobX.Mediator.Events;
using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Mediator.Values
{
    public abstract class RuntimeValueAsset<TValue> : ValueAsset<TValue>, IOnExitEditMode, IOnEnterEditMode
    {
        [ReadonlyInspector]
        [NonSerialized] private TValue _value;

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
            get => _value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (EqualityComparer<TValue>.Default.Equals(value, _value))
                {
                    return;
                }

                _value = value;
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

        public static implicit operator TValue(RuntimeValueAsset<TValue> serializedValueAsset)
        {
            return serializedValueAsset._value;
        }

        protected RuntimeValueAsset()
        {
            EngineCallbacks.AddExitEditModeListener(this);
            EngineCallbacks.AddEnterEditModeListener(this);
        }

        public void OnExitEditMode()
        {
            Value = default(TValue);
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
            Value = default(TValue);
        }
#endif
    }
}