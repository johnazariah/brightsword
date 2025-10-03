// Decompiled with JetBrains decompiler
// Type: BrightSword.Crucible.ExceptionHelper
// Assembly: BrightSword.Crucible, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 719B4D8D-7577-4C5C-8115-083F94C056BF
// Assembly location: C:\Users\johnaz\AppData\Local\Temp\Depejin\bc05fa93a2\lib\net40\BrightSword.Crucible.dll

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

#nullable disable
namespace BrightSword.Crucible;

public static class ExceptionHelper
{
  public static TException ExpectException<TException>(
    this Action action,
    string expectedExceptionMessage = null)
    where TException : Exception
  {
    try
    {
      action();
      Assert.Fail("An exception of type '{0}' was expected but not thrown", new object[1]
      {
        (object) typeof (TException).Name
      });
    }
    catch (Exception ex)
    {
      if (ex is TException exception)
      {
        if (string.IsNullOrWhiteSpace(expectedExceptionMessage) || exception.Message.Equals(expectedExceptionMessage, StringComparison.OrdinalIgnoreCase))
        {
          return exception;
        }

        Assert.Fail(
          "An exception of type '{0}' with message '{1}' was expected. The exception was thrown but the exception message was '{2}'.",
          typeof(TException).Name,
          expectedExceptionMessage,
          exception.Message);
      }

      Assert.Fail(
        "An exception of type '{0}' was expected, but an exception of type '{1}', with message '{2}' was thrown instead.",
        typeof(TException).Name,
        ex.GetType().Name,
        ex.Message);
    }
    return default;
  }
}
