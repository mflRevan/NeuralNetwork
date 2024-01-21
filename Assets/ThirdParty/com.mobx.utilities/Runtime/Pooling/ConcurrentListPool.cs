using System.Collections.Generic;

namespace MobX.Utilities.Pooling
{
    public class ConcurrentListPool<T> : ConcurrentCollectionPool<List<T>, T>
    {
    }
}