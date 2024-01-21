using UnityEngine;

namespace MobX.Mediator.Pooling
{
    /// <summary>
    ///     Generic <see cref="GameObject" /> object pool.
    /// </summary>
    public class GameObjectPool : PoolAsset<GameObject>
    {
        protected override void OnReleaseInstance(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(Parent);
        }

        protected override void OnGetInstance(GameObject instance)
        {
            instance.SetActive(true);
        }
    }
}
