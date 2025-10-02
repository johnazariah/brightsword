using System;

namespace BrightSword.SwissKnife
{
    public static class Validator
    {
#pragma warning disable CA2201 // Use specific exception types
        public static void Check(this bool condition, string message = null)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }

#pragma warning disable RCS1163 // Unused parameter
        public static void Check<TException>(this bool condition, string message = null) where TException : Exception, new()
        {
            if (!condition)
            {
                throw new TException();
            }
        }
#pragma warning restore RCS1163

        public static void Check(this Func<bool> predicate, string message = null)
        {
            if (!predicate())
            {
                throw new Exception(message);
            }
        }

#pragma warning disable RCS1163 // Unused parameter
        public static void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()
        {
            if (!predicate())
            {
                throw new TException();
            }
        }
#pragma warning restore RCS1163
    }
}
