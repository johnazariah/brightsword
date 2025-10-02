using System;

#nullable disable
namespace BrightSword.SwissKnife;

public class Disposable<T> : IDisposable
{
  private readonly Action<T> _dispose;
  private readonly T _instance;

  public Disposable(T instance, Action<T> dispose)
  {
    this._instance = instance;
    this._dispose = dispose;
  }

  public T Instance => this._instance;

  public void Dispose()
  {
    if (this._dispose == null)
      return;
    this._dispose(this._instance);
  }
}
