using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using MobX.Utilities.Singleton;
using UnityEngine;

namespace MobX.Platforms
{
    public class PlatformManager : SingletonAsset<PlatformManager>, IOnInitializationCompleted
    {
        [InlineInspector]
        [SerializeField] private Platform platform;

        internal Platform Platform
        {
            get => platform;
            set
            {
                platform = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        public void OnInitializationCompleted()
        {
            platform.Initialize();
        }
    }
}
