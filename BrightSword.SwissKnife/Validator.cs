using System;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides guard and validation helpers for runtime argument and condition checking.
    /// </summary>
    /// <remarks>
    /// These helpers throw exceptions when conditions or predicates fail, simplifying defensive programming.
    /// </remarks>
    public static class Validator
    {
        /// <summary>
        /// Throws an <see cref="Exception"/> if the condition is false.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The exception message if the condition fails.</param>
        /// <exception cref="Exception">Thrown if <paramref name="condition"/> is false.</exception>
        /// <example>
        /// <code>
        /// Validator.Check(x > 0, "x must be positive");
        /// </code>
        /// </example>
        [Obsolete("Use Debug.Assert, ArgumentNullException, or ArgumentException instead. Modern C# provides built-in guard patterns.")]
        public static void Check(this bool condition, string message = null)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Throws a specific exception type if the condition is false.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The exception message (not used).</param>
        /// <exception cref="TException">Thrown if <paramref name="condition"/> is false.</exception>
        /// <example>
        /// <code>
        /// Validator.Check<ArgumentNullException>(obj != null, "obj cannot be null");
        /// </code>
        /// </example>
        [Obsolete("Use ArgumentNullException, ArgumentException, or custom exception patterns instead.")]
        public static void Check<TException>(this bool condition, string message = null) where TException : Exception, new()
        {
            _ = message;
            if (!condition)
            {
                throw new TException();
            }
        }

        /// <summary>
        /// Throws an <see cref="Exception"/> if the predicate returns false.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="message">The exception message if the predicate fails.</param>
        /// <exception cref="Exception">Thrown if <paramref name="predicate"/> returns false.</exception>
        /// <example>
        /// <code>
        /// Validator.Check(() => x > 0, "x must be positive");
        /// </code>
        /// </example>
        [Obsolete("Use Debug.Assert, ArgumentNullException, or ArgumentException instead. Modern C# provides built-in guard patterns.")]
        public static void Check(this Func<bool> predicate, string message = null)
        {
            if (!predicate())
            {
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Throws a specific exception type if the predicate returns false.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="message">The exception message (not used).</param>
        /// <exception cref="TException">Thrown if <paramref name="predicate"/> returns false.</exception>
        /// <example>
        /// <code>
        /// Validator.Check<ArgumentException>(() => value > 0, "value must be positive");
        /// </code>
        /// </example>
        [Obsolete("Use ArgumentNullException, ArgumentException, or custom exception patterns instead.")]
        public static void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()
        {
            _ = message;
            if (!predicate())
            {
                throw new TException();
            }
        }
    }
}
