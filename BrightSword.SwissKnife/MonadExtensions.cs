using System;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides monadic extension methods for conditional and safe invocation patterns on reference types.
    /// </summary>
    /// <remarks>
    /// These helpers simplify null-safe invocation and conditional logic for reference types.
    /// </remarks>
    public static class MonadExtensions
    {
        /// <summary>
        /// Safely invokes a function on a reference type if it is not null, otherwise returns a default result.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="func">The function to invoke if <paramref name="@this"/> is not null.</param>
        /// <param name="defaultResult">The result to return if <paramref name="@this"/> is null.</param>
        /// <returns>The result of <paramref name="func"/> or <paramref name="defaultResult"/>.</returns>
        /// <example>
        /// <code>
        /// string s = null;
        /// int len = s.Maybe(str => str.Length, 0); // 0
        /// </code>
        /// </example>
        [Obsolete("Use ?. (null-conditional), pattern matching, or switch expressions in modern C#. Maybe is superseded by language features.")]
        public static TResult Maybe<T, TResult>(
            this T @this,
            Func<T, TResult> func,
            TResult defaultResult = default)
            where T : class => @this is null ? defaultResult : func(@this);

        /// <summary>
        /// Safely invokes an action on a reference type if it is not null.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="action">The action to invoke if <paramref name="@this"/> is not null.</param>
        /// <returns>The original instance.</returns>
        /// <example>
        /// <code>
        /// string s = "hello";
        /// s.Maybe(str => Console.WriteLine(str)); // prints "hello"
        /// </code>
        /// </example>
        [Obsolete("Use ?. (null-conditional) or pattern matching in modern C#. Maybe is superseded by language features.")]
        public static T Maybe<T>(this T @this, Action<T> action) where T : class
        {
            if (@this is not null)
            {
                action(@this);
            }
            return @this;
        }

        /// <summary>
        /// Invokes a function if the predicate is true, otherwise returns a default result.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="predicate">Predicate to test the instance.</param>
        /// <param name="func">Function to invoke if predicate is true.</param>
        /// <param name="defaultResult">Result to return if predicate is false or instance is null.</param>
        /// <returns>The result of <paramref name="func"/> or <paramref name="defaultResult"/>.</returns>
        /// <example>
        /// <code>
        /// string s = "abc";
        /// int len = s.When(str => str.Length > 2, str => str.Length, -1); // 3
        /// </code>
        /// </example>
        [Obsolete("Use pattern matching or switch expressions in modern C#. When is superseded by language features.")]
        public static TResult When<T, TResult>(this T @this, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default) where T : class
            => @this.Maybe(_ => predicate(_) ? func(_) : defaultResult, defaultResult);

        /// <summary>
        /// Invokes an action if the predicate is true.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="predicate">Predicate to test the instance.</param>
        /// <param name="action">Action to invoke if predicate is true.</param>
        /// <returns>The original instance.</returns>
        /// <example>
        /// <code>
        /// string s = "abc";
        /// s.When(str => str.Length > 2, str => Console.WriteLine(str)); // prints "abc"
        /// </code>
        /// </example>
        [Obsolete("Use pattern matching or switch expressions in modern C#. When is superseded by language features.")]
        public static T When<T>(this T @this, Func<T, bool> predicate, Action<T> action) where T : class
            => @this.Maybe(_ =>
            {
                if (predicate(_))
                {
                    action(_);
                }

                return _;
            });

        /// <summary>
        /// Invokes a function if the predicate is false, otherwise returns a default result.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="predicate">Predicate to test the instance.</param>
        /// <param name="func">Function to invoke if predicate is false.</param>
        /// <param name="defaultResult">Result to return if predicate is true or instance is null.</param>
        /// <returns>The result of <paramref name="func"/> or <paramref name="defaultResult"/>.</returns>
        /// <example>
        /// <code>
        /// string s = "abc";
        /// int len = s.Unless(str => str.Length > 5, str => str.Length, -1); // 3
        /// </code>
        /// </example>
        [Obsolete("Use pattern matching or switch expressions in modern C#. Unless is superseded by language features.")]
        public static TResult Unless<T, TResult>(this T @this, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default) where T : class
            => @this.Maybe(_ => !predicate(_) ? func(_) : defaultResult, defaultResult);

        /// <summary>
        /// Invokes an action if the predicate is false.
        /// </summary>
        /// <typeparam name="T">The reference type.</typeparam>
        /// <param name="@this">The instance to operate on.</param>
        /// <param name="predicate">Predicate to test the instance.</param>
        /// <param name="action">Action to invoke if predicate is false.</param>
        /// <returns>The original instance.</returns>
        /// <example>
        /// <code>
        /// string s = "abc";
        /// s.Unless(str => str.Length > 5, str => Console.WriteLine(str)); // prints "abc"
        /// </code>
        /// </example>
        [Obsolete("Use pattern matching or switch expressions in modern C#. Unless is superseded by language features.")]
        public static T Unless<T>(this T @this, Func<T, bool> predicate, Action<T> action) where T : class
            => @this.Maybe(_ =>
            {
                if (!predicate(_))
                {
                    action(_);
                }

                return _;
            });
    }
}
