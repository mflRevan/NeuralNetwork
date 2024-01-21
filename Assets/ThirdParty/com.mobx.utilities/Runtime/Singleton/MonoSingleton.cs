using UnityEngine;

namespace MobX.Utilities.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Singleton { get; private set; }
        public static bool IsSingletonInitialized { get; private set; } = false;

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Singleton", $"More that one instance of {typeof(T).Name} found!");
                return;
            }

            Singleton = (T) this;
            IsSingletonInitialized = true;
        }

        private void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
                IsSingletonInitialized = false;
            }
        }
    }
}
