using System;

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
      throw new TException();
  }

  public static void Check(this Func<bool> predicate, string message = null)
  {
    if (!predicate())
      throw new Exception(message);
  }

  public static void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()
  {
    if (!predicate())
      throw new TException();
  }
}
