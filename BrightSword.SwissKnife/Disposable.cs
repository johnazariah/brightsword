using System;

namespace BrightSword.SwissKnife
{
    public sealed class Disposable<T> : IDisposable
    {
        private readonly Action<T> _dispose;

        public T Instance { get; }

        public Disposable(T instance, Action<T> dispose)
        {
            Instance = instance;
            _dispose = dispose ?? (_ => { });
        }

        public void Dispose()
        {
            _dispose?.Invoke(Instance);
            GC.SuppressFinalize(this);
        }
    }
}
