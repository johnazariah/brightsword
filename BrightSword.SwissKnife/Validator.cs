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
        /// (This overload preserves legacy behavior.)
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The exception message if the condition fails.</param>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="condition"/> is false.</exception>
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
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws a specific exception type if the condition is false.
        /// If an instance of <typeparamref name="TException"/> cannot be created with the
        /// provided message, an instance created with the default constructor will be used
        /// or an <see cref="InvalidOperationException"/> will be thrown as a fallback.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The exception message to use when creating the exception.</param>
        /// <exception cref="TException">Thrown if <paramref name="condition"/> is false and an instance can be created.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a <typeparamref name="TException"/> instance could not be created.</exception>
        /// <example>
        /// <code>
        /// Validator.Check<ArgumentNullException>(obj != null, "obj cannot be null");
        /// </code>
        /// </example>
        [Obsolete("Use ArgumentNullException, ArgumentException, or custom exception patterns instead.")]
        public static void Check<TException>(this bool condition, string message = null) where TException : Exception, new()
        {
            if (!condition)
            {
                var ex = CreateExceptionInstance<TException>(message);
                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="Exception"/> if the predicate returns false.
        /// (This overload preserves legacy behavior.)
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="message">The exception message if the predicate fails.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="predicate"/> returns false.</exception>
        /// <example>
        /// <code>
        /// Validator.Check(() => x > 0, "x must be positive");
        /// </code>
        /// </example>
        [Obsolete("Use Debug.Assert, ArgumentNullException, or ArgumentException instead. Modern C# provides built-in guard patterns.")]
        public static void Check(this Func<bool> predicate, string message = null)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            if (!predicate())
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throws a specific exception type if the predicate returns false.
        /// If an instance of <typeparamref name="TException"/> cannot be created with the
        /// provided message, an instance created with the default constructor will be used
        /// or an <see cref="InvalidOperationException"/> will be thrown as a fallback.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="message">The exception message to use when creating the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
        /// <exception cref="TException">Thrown if <paramref name="predicate"/> returns false and an instance can be created.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a <typeparamref name="TException"/> instance could not be created.</exception>
        /// <example>
        /// <code>
        /// Validator.Check<ArgumentException>(() => value > 0, "value must be positive");
        /// </code>
        /// </example>
        [Obsolete("Use ArgumentNullException, ArgumentException, or custom exception patterns instead.")]
        public static void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()
        {
            ArgumentNullException.ThrowIfNull(predicate);
            if (!predicate())
            {
                var ex = CreateExceptionInstance<TException>(message);
                throw ex;
            }
        }

        private static Exception CreateExceptionInstance<TException>(string message) where TException : Exception, new()
        {
            if (message != null)
            {
                try
                {
                    var instance = Activator.CreateInstance(typeof(TException), message) as Exception;
                    if (instance != null)
                    {
                        return instance;
                    }
                }
                catch
                {
                    // fall through to default construction
                }
            }

            try
            {
                return new TException();
            }
            catch
            {
                return new InvalidOperationException(message);
            }
        }
    }
}
