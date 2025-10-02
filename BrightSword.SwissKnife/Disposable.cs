using System;

namespace BrightSword.SwissKnife;

public class Disposable<T> : IDisposable
{
    private readonly Action<T> _dispose;
    private readonly T _instance;

    public Disposable(T instance, Action<T> dispose)
    {
        _instance = instance;
        _dispose = dispose;
    }

    public T Instance => _instance;

    public void Dispose() => _dispose?.Invoke(_instance);
}
