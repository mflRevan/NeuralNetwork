namespace MobX.Mediator.Pooling
{
    public abstract class IPoolAsset : MediatorAsset
    {
        /// <summary>
        ///     Preload assets to prevent potential frame drops.
        /// </summary>
        public abstract void Load();

        /// <summary>
        ///     Clear the elements of the pool.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Recreate the elements of the pool.
        /// </summary>
        public abstract void Refresh();
    }
}