// Decompiled with JetBrains decompiler
// Type: BrightSword.Crucible.ExceptionHelper
// Assembly: BrightSword.Crucible, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 719B4D8D-7577-4C5C-8115-083F94C056BF
// Assembly location: C:\Users\johnaz\AppData\Local\Temp\Depejin\bc05fa93a2\lib\net40\BrightSword.Crucible.dll

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
                throw new InvalidOperationException($"An exception of type '{typeof(TException).Name}' was expected but not thrown");
            }
            catch (Exception ex)
            {
                if (ex is TException exception)
                {
                    if (string.IsNullOrWhiteSpace(expectedExceptionMessage) || exception.Message.Equals(expectedExceptionMessage, StringComparison.OrdinalIgnoreCase))
                    {
                        return exception;
                    }

                    throw new InvalidOperationException($"An exception of type '{typeof(TException).Name}' with message '{expectedExceptionMessage}' was expected. The exception was thrown but the exception message was '{exception.Message}'.");
                }

                throw new InvalidOperationException($"An exception of type '{typeof(TException).Name}' was expected, but an exception of type '{ex.GetType().Name}', with message '{ex.Message}' was thrown instead.");
            }
            // unreachable but required for compile-time return
            // ReSharper disable once HeuristicUnreachableCode
            // return default;
  }
}
