using System;
using UnityEngine.AddressableAssets;

namespace MobX.Utilities.Addressables
{
    [Serializable]
    public class AssetReferenceScene
#if UNITY_EDITOR
        : AssetReferenceT<UnityEditor.SceneAsset>
#else
    : AssetReference
#endif
    {
        public AssetReferenceScene(string guid) : base(guid)
        {
        }
    }
}