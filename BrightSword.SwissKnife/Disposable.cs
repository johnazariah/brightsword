using System;

namespace BrightSword.SwissKnife
{
    public sealed class Disposable<T>(T instance, Action<T> dispose) : IDisposable
    {
        private readonly Action<T> _dispose = dispose ?? (_ => { });

        public T Instance { get; } = instance;

        public void Dispose()
        {
            _dispose(Instance);
            GC.SuppressFinalize(this);
        }
    }
}
