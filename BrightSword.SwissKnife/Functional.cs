using System;
using System.Collections.Concurrent;

namespace BrightSword.SwissKnife
{
    public static class Functional
    {
        public static Func<TArgument, TResult> Y<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            Func<TArgument, TResult> funcFixed = null!;
            funcFixed = func(x => funcFixed(x));
            return funcFixed;
        }

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

        public static Func<TArgument, TResult> MemoizeFix<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
        {
            Func<TArgument, TResult> funcMemoized = null!;
            funcMemoized = func(x => funcMemoized(x));
            return funcMemoized.Memoize();
        }

        private static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
            var map = new ConcurrentDictionary<(TArg1, TArg2), TResult>();
            return (a1, a2) => map.GetOrAdd((a1, a2), _ => func(_.Item1, _.Item2));
        }

        public static Func<TArg1, TArg2, TResult> MemoizeFix<TArg1, TArg2, TResult>(Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)
        {
            Func<TArg1, TArg2, TResult> funcMemoized = null!;
            funcMemoized = func((a1, a2) => funcMemoized(a1, a2));
            return funcMemoized.Memoize();
        }

        private static Func<TArg1, TArg2, TArg3, TResult> Memoize<TArg1, TArg2, TArg3, TResult>(this Func<TArg1, TArg2, TArg3, TResult> func)
        {
            var map = new ConcurrentDictionary<(TArg1, TArg2, TArg3), TResult>();
            return (a1, a2, a3) => map.GetOrAdd((a1, a2, a3), _ => func(_.Item1, _.Item2, _.Item3));
        }

        public static Func<TArg1, TArg2, TArg3, TResult> MemoizeFix<TArg1, TArg2, TArg3, TResult>(Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)
        {
            Func<TArg1, TArg2, TArg3, TResult> funcMemoized = null!;
            funcMemoized = func((a1, a2, a3) => funcMemoized(a1, a2, a3));
            return funcMemoized.Memoize();
        }
    }
}
