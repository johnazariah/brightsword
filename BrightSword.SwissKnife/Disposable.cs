using System;

namespace BrightSword.SwissKnife
{
    public class Disposable<T>(T instance, Action<T> dispose) : IDisposable
    {
        private readonly Action<T> _dispose = dispose;

        public T Instance { get; } = instance;

        public void Dispose()
        {
            _dispose?.Invoke(Instance);
            GC.SuppressFinalize(this);
        }
    }
}
