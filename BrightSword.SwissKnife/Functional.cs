using System;
using System.Collections.Concurrent;

namespace BrightSword.SwissKnife;

public static class Functional
{
    public static Func<TArgument, TResult> Y<TArgument, TResult>(
      Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
    {
        Func<TArgument, TResult> funcFixed = (Func<TArgument, TResult>)null;
        funcFixed = func((Func<TArgument, TResult>)(_ => funcFixed(_)));
        return funcFixed;
    }

    private static Func<TArgument, TResult> Trace<TArgument, TResult>(
      this Func<TArgument, TResult> func)
    {
        return (Func<TArgument, TResult>)(_ =>
        {
            System.Diagnostics.Trace.WriteLine($"{func}({_}) called");
            return func(_);
        });
    }

    private static Func<TArgument, TResult> Memoize<TArgument, TResult>(
      this Func<TArgument, TResult> func)
    {
        ConcurrentDictionary<TArgument, TResult> _map = new ConcurrentDictionary<TArgument, TResult>();
        return (Func<TArgument, TResult>)(_ => _map.GetOrAdd(_, func.Trace<TArgument, TResult>()));
    }

    public static Func<TArgument, TResult> MemoizeFix<TArgument, TResult>(
      Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)
    {
        Func<TArgument, TResult> funcMemoized = (Func<TArgument, TResult>)null;
        funcMemoized = func((Func<TArgument, TResult>)(_ => funcMemoized(_)));
        funcMemoized = funcMemoized.Memoize<TArgument, TResult>();
        return funcMemoized;
    }

    private static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(
      this Func<TArg1, TArg2, TResult> func)
    {
        ConcurrentDictionary<Tuple<TArg1, TArg2>, TResult> _map = new ConcurrentDictionary<Tuple<TArg1, TArg2>, TResult>();
        return (Func<TArg1, TArg2, TResult>)((_a1, _a2) => _map.GetOrAdd(new Tuple<TArg1, TArg2>(_a1, _a2), (Func<Tuple<TArg1, TArg2>, TResult>)(_ => func(_.Item1, _.Item2))));
    }

    public static Func<TArg1, TArg2, TResult> MemoizeFix<TArg1, TArg2, TResult>(
      Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)
    {
        Func<TArg1, TArg2, TResult> funcMemoized = (Func<TArg1, TArg2, TResult>)null;
        funcMemoized = func((Func<TArg1, TArg2, TResult>)((_a1, _a2) => funcMemoized(_a1, _a2)));
        funcMemoized = funcMemoized.Memoize<TArg1, TArg2, TResult>();
        return funcMemoized;
    }

    private static Func<TArg1, TArg2, TArg3, TResult> Memoize<TArg1, TArg2, TArg3, TResult>(
      this Func<TArg1, TArg2, TArg3, TResult> func)
    {
        ConcurrentDictionary<Tuple<TArg1, TArg2, TArg3>, TResult> _map = new ConcurrentDictionary<Tuple<TArg1, TArg2, TArg3>, TResult>();
        return (Func<TArg1, TArg2, TArg3, TResult>)((_a1, _a2, _a3) => _map.GetOrAdd(new Tuple<TArg1, TArg2, TArg3>(_a1, _a2, _a3), (Func<Tuple<TArg1, TArg2, TArg3>, TResult>)(_ => func(_.Item1, _.Item2, _.Item3))));
    }

    public static Func<TArg1, TArg2, TArg3, TResult> MemoizeFix<TArg1, TArg2, TArg3, TResult>(
      Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)
    {
        Func<TArg1, TArg2, TArg3, TResult> funcMemoized = (Func<TArg1, TArg2, TArg3, TResult>)null;
        funcMemoized = func((Func<TArg1, TArg2, TArg3, TResult>)((_a1, _a2, _a3) => funcMemoized(_a1, _a2, _a3)));
        funcMemoized = funcMemoized.Memoize<TArg1, TArg2, TArg3, TResult>();
        return funcMemoized;
    }
}
