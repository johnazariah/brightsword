// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.Validator
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System;

#nullable disable
namespace BrightSword.SwissKnife;

public static class Validator
{
  public static void Check(this bool condition, string message = null)
  {
    if (!condition)
      throw new Exception(message);
  }

  public static void Check<TException>(this bool condition, string message = null) where TException : Exception, new()
  {
    if (!condition)
      throw (object) new TException();
  }

  public static void Check(this Func<bool> predicate, string message = null)
  {
    if (!predicate())
      throw new Exception(message);
  }

  public static void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()
  {
    if (!predicate())
      throw (object) new TException();
  }
}
