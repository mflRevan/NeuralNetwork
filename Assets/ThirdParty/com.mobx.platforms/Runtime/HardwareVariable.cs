using MobX.Utilities.Collections;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Platforms
{
    [Serializable]
    public struct HardwareVariable<T>
    {
        [SerializeField] private T defaultValue;
        [SerializeField] private Map<HardwareType, T> platformOverrides;

        public T Value => GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetValue()
        {
            return platformOverrides.TryGetValue(Platform.Current.HardwareType, out var value) ? value : defaultValue;
        }

        public static implicit operator T(HardwareVariable<T> variable)
        {
            return variable.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
