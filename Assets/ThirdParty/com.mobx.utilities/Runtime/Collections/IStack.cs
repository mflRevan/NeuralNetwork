using System.Collections.Generic;

namespace MobX.Utilities.Collections
{
    public interface IStack<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        void Push(T item);

        T Pop();

        T Peek();

        void PushRange(IEnumerable<T> collection);

        bool TryPeek(out T item);

        bool TryPop(out T item);
    }
}