using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MobX.Utilities.Addressables
{
    /// <summary>
    /// Creates an AssetReference that is restricted to having a specific Component.
    /// * This is the class that inherits from AssetReference.  It is generic and does not specify which Components it might care about.  A concrete child of this class is required for serialization to work.* At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At edit-time it validates that the asset set on it is a GameObject with the required Component.
    /// * At runtime it can load/instantiate the GameObject, then return the desired component.  API matches base class (LoadAssetAsync & InstantiateAsync).
    /// </summary>
    /// <typeparam name="TComponent"> The component type.</typeparam>
    [Serializable]
    public class AssetReferenceComponent<TComponent> : AssetReference where TComponent : Component
    {
        public AssetReferenceComponent(string guid) : base(guid)
        {
        }

        private async Task<TComponent> LoadAssetAsyncTaskInternal(Task<GameObject> target)
        {
            var obj = await target;
            return obj.GetComponent<TComponent>();
        }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.InstantiateAsync(position, Quaternion.identity, parent), GameObjectReady);
        }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.InstantiateAsync(parent, instantiateInWorldSpace), GameObjectReady);
        }

        public AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.LoadAssetAsync<GameObject>(), GameObjectReady);
        }

        private AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            var comp = arg.Result.GetComponent<TComponent>();
            return UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateCompletedOperation<TComponent>(comp, string.Empty);
        }

        public override bool ValidateAsset(UnityEngine.Object obj)
        {
            var go = obj as GameObject;
            return go != null && go.GetComponent<TComponent>() != null;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
        }

        public void ReleaseInstance(AsyncOperationHandle<TComponent> op)
        {
            // Release the instance
            var component = op.Result as Component;
            if (component != null)
            {
                UnityEngine.AddressableAssets.Addressables.ReleaseInstance(component.gameObject);
            }

            // Release the handle
            UnityEngine.AddressableAssets.Addressables.Release(op);
        }
    }
}