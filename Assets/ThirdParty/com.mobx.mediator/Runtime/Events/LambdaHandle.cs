using System;

namespace MobX.Mediator.Events
{
    public readonly struct LambdaHandle : IDisposable
    {
        private readonly Action _dispose;

        public LambdaHandle(Action dispose)
        {
            this._dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}