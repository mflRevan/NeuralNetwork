using MobX.Utilities.Inspector;

namespace MobX.Mediator.Actions
{
    public abstract class ActionAsset : MediatorAsset
    {
        [Button]
        public abstract void Perform();
    }

    public abstract class ActionAsset<T> : MediatorAsset
    {
        [Button]
        public abstract void Perform(T value);
    }

    public abstract class ActionAsset<T1, T2> : MediatorAsset
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2);
    }

    public abstract class ActionAsset<T1, T2, T3> : MediatorAsset
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2, T3 value3);
    }

    public abstract class ActionAsset<T1, T2, T3, T4> : MediatorAsset
    {
        [Button]
        public abstract void Perform(T1 value1, T2 value2, T3 value3, T4 value4);
    }
}