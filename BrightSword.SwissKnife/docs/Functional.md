# Functional

## Purpose
Provides functional programming helpers for memoization and fixed-point combinators. These helpers enable efficient recursive and memoized function definitions in .NET.

## When to Use
- When you need to define recursive functions without named methods (Y combinator).
- When you want to memoize recursive functions for performance.

## How to Use
Use these methods to create recursive or memoized functions, including support for multiple arguments.

## Key APIs
- <code>Y<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)</code>: Implements the Y combinator for anonymous recursion.
- <code>MemoizeFix<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)</code>: Memoizes a recursive function using the fixed-point combinator.
- <code>MemoizeFix<TArg1, TArg2, TResult>(Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)</code>: Memoizes a recursive function of two arguments.
- <code>MemoizeFix<TArg1, TArg2, TArg3, TResult>(Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)</code>: Memoizes a recursive function of three arguments.

## Examples
```csharp
// Factorial using Y combinator
var factorial = Functional.Y<int, int>(fact => n => n == 0 ? 1 : n * fact(n - 1));
int result = factorial(5); // 120

// Fibonacci using memoized fixed-point combinator
var fib = Functional.MemoizeFix<int, int>(f => n => n < 2 ? n : f(n - 1) + f(n - 2));
int result2 = fib(10); // 55

// Ackermann function using Y combinator
var ackermann = Functional.Y<(int m, int n), int>(ack => t =>
    t.m == 0 ? t.n + 1 :
    t.n == 0 ? ack((t.m - 1, 1)) :
    ack((t.m - 1, ack((t.m, t.n - 1))))));
int ackResult = ackermann((2, 3)); // 9

// Memoized binomial coefficient (n choose k)
var binom = Functional.MemoizeFix<int, int, int>((choose) => (n, k) =>
    k == 0 || k == n ? 1 : choose(n - 1, k - 1) + choose(n - 1, k));
int c = binom(5, 2); // 10

// Memoized recursive function with three arguments
var tribonacci = Functional.MemoizeFix<int, int, int, int>((trib) => (a, b, c) =>
    c == 0 ? 0 : c == 1 ? 1 : c == 2 ? 1 : trib(a, b, c - 1) + trib(a, b, c - 2) + trib(a, b, c - 3));
int t = tribonacci(0, 1, 10); // 149
```

## Remarks
These functions are intentionally small so they can be inlined and mixed with imperative code without performance surprises. They are useful for advanced functional programming scenarios in .NET. The Y combinator enables anonymous recursion, while MemoizeFix provides efficient memoization for recursive functions, including those with multiple arguments.
