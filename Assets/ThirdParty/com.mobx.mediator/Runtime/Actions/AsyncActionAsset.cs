using Cysharp.Threading.Tasks;
using MobX.Utilities.Inspector;

namespace MobX.Mediator.Actions
{
    public abstract class AsyncActionAsset : MediatorAsset
    {
        [Button]
        public abstract UniTask PerformAsync();
    }

    public abstract class AsyncActionAsset<T> : MediatorAsset
    {
        [Button]
        public abstract UniTask PerformAsync(T value);
    }

    public abstract class AsyncActionAsset<T1, T2> : MediatorAsset
    {
        [Button]
        public abstract UniTask PerformAsync(T1 value1, T2 value2);
    }

    public abstract class AsyncActionAsset<T1, T2, T3> : MediatorAsset
    {
        [Button]
        public abstract UniTask PerformAsync(T1 value1, T2 value2, T3 value3);
    }

    public abstract class AsyncActionAsset<T1, T2, T3, T4> : MediatorAsset
    {
        [Button]
        public abstract UniTask PerformAsync(T1 value1, T2 value2, T3 value3, T4 value4);
    }
}