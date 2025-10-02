// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.MonadExtensions
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System;

#nullable disable
namespace BrightSword.SwissKnife;

public static class MonadExtensions
{
  public static TResult Maybe<T, TResult>(
    this T _this,
    Func<T, TResult> func,
    TResult defaultResult = null)
    where T : class
  {
    return (object) _this == null ? defaultResult : func(_this);
  }

  public static T Maybe<T>(this T _this, Action<T> action) where T : class
  {
    if ((object) _this != null)
      action(_this);
    return _this;
  }

  public static TResult When<T, TResult>(
    this T _this,
    Func<T, bool> predicate,
    Func<T, TResult> func,
    TResult defaultResult = null)
    where T : class
  {
    return _this.Maybe<T, TResult>((Func<T, TResult>) (_ => predicate(_) ? func(_) : defaultResult), defaultResult);
  }

  public static T When<T>(this T _this, Func<T, bool> predicate, Action<T> action) where T : class
  {
    return _this.Maybe<T, T>((Func<T, T>) (_ =>
    {
      if (predicate(_))
        action(_);
      return _;
    }));
  }

  public static TResult Unless<T, TResult>(
    this T _this,
    Func<T, bool> predicate,
    Func<T, TResult> func,
    TResult defaultResult = null)
    where T : class
  {
    return _this.Maybe<T, TResult>((Func<T, TResult>) (_ => !predicate(_) ? func(_) : defaultResult), defaultResult);
  }

  public static T Unless<T>(this T _this, Func<T, bool> predicate, Action<T> action) where T : class
  {
    return _this.Maybe<T, T>((Func<T, T>) (_ =>
    {
      if (!predicate(_))
        action(_);
      return _;
    }));
  }
}
