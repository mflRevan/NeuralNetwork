using JetBrains.Annotations;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Types
{
    [Serializable]
    public class Prefab : Prefab<GameObject>
    {
    }

    [Serializable]
    public class Prefab<T> where T : Object
    {
        [UsedImplicitly]
        public T value;

        public static implicit operator T(Prefab<T> prefab)
        {
            return prefab.value;
        }

        public T Instantiate()
        {
            return Object.Instantiate(value);
        }

        public T Instantiate(Transform parent)
        {
            return Object.Instantiate(value, parent);
        }

        public T Instantiate(Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(value, position, rotation);
        }
    }
}
