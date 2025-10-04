using System;

namespace BrightSword.Crucible
{
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
                return ex is TException exception
                    ? string.IsNullOrWhiteSpace(expectedExceptionMessage) || exception.Message.Equals(expectedExceptionMessage, StringComparison.OrdinalIgnoreCase)
                        ? exception
                        : throw new InvalidOperationException($"An exception of type '{typeof(TException).Name}' with message '{expectedExceptionMessage}' was expected. The exception was thrown but the exception message was '{exception.Message}'.")
                    : throw new InvalidOperationException($"An exception of type '{typeof(TException).Name}' was expected, but an exception of type '{ex.GetType().Name}', with message '{ex.Message}' was thrown instead.");
            }
        }
    }
}
