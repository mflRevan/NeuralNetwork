using MobX.Utilities.Collections;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Platforms
{
    [Serializable]
    public struct PlatformVariable<T>
    {
        [SerializeField] private T defaultValue;
        [SerializeField] private Map<PlatformType, T> platformOverrides;

        public T Value => GetValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetValue()
        {
            return platformOverrides.TryGetValue(Platform.Current.PlatformType, out var value) ? value : defaultValue;
        }

        public static implicit operator T(PlatformVariable<T> variable)
        {
            return variable.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
