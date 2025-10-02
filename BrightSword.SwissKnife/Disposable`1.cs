// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.Disposable`1
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

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
