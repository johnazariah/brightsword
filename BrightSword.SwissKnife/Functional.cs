using System;
using System.Collections.Concurrent;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides functional programming helpers for memoization and fixed-point combinators.
    /// </summary>
    /// <remarks>
    /// These helpers enable efficient recursive and memoized function definitions in .NET.
    /// </remarks>
    public static class Functional
    {
        /// <summary>
        /// Implements the Y combinator for anonymous recursion.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">A function that takes itself as an argument.</param>
        /// <returns>A recursive function.</returns>
        /// <example>
        /// <code>
        /// // Factorial using Y combinator
        /// var factorial = Functional.Y<int, int>(fact => n => n == 0 ? 1 : n * fact(n - 1));
        /// int result = factorial(5); // 120
        /// </code>
        /// </example>
        public static Func<TArgument, TResult> Y<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            Func<TArgument, TResult> funcFixed = null;
            funcFixed = func(x => funcFixed(x));
            return funcFixed;
        }

        /// <summary>
        /// Memoizes a recursive function using the fixed-point combinator.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">A function that takes itself as an argument.</param>
        /// <returns>A memoized recursive function.</returns>
        /// <example>
        /// <code>
        /// // Fibonacci using memoized fixed-point combinator
        /// var fib = Functional.MemoizeFix<int, int>(f => n => n < 2 ? n : f(n - 1) + f(n - 2));
        /// int result = fib(10); // 55
        /// </code>
        /// </example>
        public static Func<TArgument, TResult> MemoizeFix<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            Func<TArgument, TResult> funcMemoized = null;
            funcMemoized = func(x => funcMemoized(x));
            return funcMemoized.Memoize();
        }

        /// <summary>
        /// Memoizes a recursive function of two arguments using the fixed-point combinator.
        /// </summary>
        /// <typeparam name="TArg1">The first argument type.</typeparam>
        /// <typeparam name="TArg2">The second argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">A function that takes itself as an argument.</param>
        /// <returns>A memoized recursive function of two arguments.</returns>
        /// <example>
        /// <code>
        /// // Example: Memoized function of two arguments
        /// var sum = Functional.MemoizeFix<int, int, int>(f => (a, b) => a + b);
        /// int result = sum(2, 3); // 5
        /// </code>
        /// </example>
        public static Func<TArg1, TArg2, TResult> MemoizeFix<TArg1, TArg2, TResult>(Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)
        {
            Func<TArg1, TArg2, TResult> funcMemoized = null;
            funcMemoized = func((a1, a2) => funcMemoized(a1, a2));
            return funcMemoized.Memoize();
        }

        /// <summary>
        /// Memoizes a recursive function of three arguments using the fixed-point combinator.
        /// </summary>
        /// <typeparam name="TArg1">The first argument type.</typeparam>
        /// <typeparam name="TArg2">The second argument type.</typeparam>
        /// <typeparam name="TArg3">The third argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">A function that takes itself as an argument.</param>
        /// <returns>A memoized recursive function of three arguments.</returns>
        /// <example>
        /// <code>
        /// // Example: Memoized function of three arguments
        /// var sum3 = Functional.MemoizeFix<int, int, int, int>(f => (a, b, c) => a + b + c);
        /// int result = sum3(1, 2, 3); // 6
        /// </code>
        /// </example>
        public static Func<TArg1, TArg2, TArg3, TResult> MemoizeFix<TArg1, TArg2, TArg3, TResult>(Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)
        {
            Func<TArg1, TArg2, TArg3, TResult> funcMemoized = null;
            funcMemoized = func((a1, a2, a3) => funcMemoized(a1, a2, a3));
            return funcMemoized.Memoize();
        }

        /// <summary>
        /// Private helper for tracing the invocation of a function. Emits a Trace message on invocation.
        /// </summary>
        /// <typeparam name="TArgument">The argument type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">The function to trace.</param>
        private static Func<TArgument, TResult> Trace<TArgument, TResult>(this Func<TArgument, TResult> func)
                => x =>
                {
                    System.Diagnostics.Trace.WriteLine($"{func}({x}) called");
                    return func(x);
                };

        private static Func<TArgument, TResult> Memoize<TArgument, TResult>(this Func<TArgument, TResult> func)
        {
            var map = new ConcurrentDictionary<TArgument, TResult>();
            return x => map.GetOrAdd(x, func.Trace());
        }

        private static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
            var map = new ConcurrentDictionary<(TArg1, TArg2), TResult>();
            return (a1, a2) => map.GetOrAdd((a1, a2), _ => func(_.Item1, _.Item2));
        }

        private static Func<TArg1, TArg2, TArg3, TResult> Memoize<TArg1, TArg2, TArg3, TResult>(this Func<TArg1, TArg2, TArg3, TResult> func)
        {
            var map = new ConcurrentDictionary<(TArg1, TArg2, TArg3), TResult>();
            return (a1, a2, a3) => map.GetOrAdd((a1, a2, a3), _ => func(_.Item1, _.Item2, _.Item3));
        }
    }
}
