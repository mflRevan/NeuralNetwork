using System.Collections.Generic;
using UnityEngine.Pool;

namespace MobX.Utilities.Pooling
{
    public class StackPool<T>
    {
        private static readonly ObjectPool<Stack<T>> pool
            = new ObjectPool<Stack<T>>(() => new Stack<T>(), actionOnRelease: l => l.Clear());

        public static Stack<T> Get()
        {
            return pool.Get();
        }

        public static void Release(Stack<T> toRelease)
        {
            pool.Release(toRelease);
        }
    }
}
