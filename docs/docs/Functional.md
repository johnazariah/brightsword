# Functional

## Purpose

Provides functional programming helpers implemented in `Functional.cs`: an implementation of the Y combinator for anonymous recursion and a family of `MemoizeFix` helpers that combine fixed-point combinators with memoization. These helpers let you define recursive functions (including anonymous ones) and optionally memoize them to dramatically improve performance for expensive or overlapping recursive computations.

## When to Use

- When you want to define recursion without naming a method (anonymous recursion) — e.g. inline or local recursive definitions.
- When a recursive algorithm has overlapping subproblems (Fibonacci, dynamic programming style recurrences) and memoization will reduce recomputation.
- When you want a compact, functional-style way to express recursive logic for algorithms used in tests, prototypes, or libraries.

Do NOT use these helpers as a drop-in cache for unbounded inputs in long-running processes without considering memory usage and key equality semantics.

## How It Works (High Level)

- `Y<TArgument, TResult>` implements the Y combinator pattern. You provide a function that accepts a recursive function and returns the actual implementation; `Y` returns the recursive function itself.

- `MemoizeFix<...>` variants accept the same fixed-point style function but wrap the produced recursive function with a memoization layer. The memoization uses `System.Collections.Concurrent.ConcurrentDictionary` keyed by the function arguments (tuples for multi-argument variants) to cache results.

The memoized functions are thread-safe (using `ConcurrentDictionary`) and safe for concurrent callers.

## API Reference

- `Func<TArgument, TResult> Functional.Y<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)`

- `Func<TArgument, TResult> Functional.MemoizeFix<TArgument, TResult>(Func<Func<TArgument, TResult>, Func<TArgument, TResult>> func)`

- `Func<TArg1, TArg2, TResult> Functional.MemoizeFix<TArg1, TArg2, TResult>(Func<Func<TArg1, TArg2, TResult>, Func<TArg1, TArg2, TResult>> func)`

- `Func<TArg1, TArg2, TArg3, TResult> Functional.MemoizeFix<TArg1, TArg2, TArg3, TResult>(Func<Func<TArg1, TArg2, TArg3, TResult>, Func<TArg1, TArg2, TArg3, TResult>> func)`

Note: All functions above return a compiled delegate that performs the desired recursion. The `MemoizeFix` variants return delegates that cache computed results for given input argument combinations.

## Examples

### 1) Factorial using the Y combinator

```csharp
// Factorial (anonymous recursion)
var factorial = Functional.Y<int, int>(self => n => n <= 1 ? 1 : n * self(n - 1));
Console.WriteLine(factorial(5)); // 120
```

Explanation: The function passed to `Y` takes a `self` function and returns the actual implementation. `Y` wires `self` to refer to the resulting function so recursion works without requiring a named method.

### 2) Fibonacci (naive) vs memoized

Naive recursive fibonacci (exponential time):

```csharp
var fibNaive = Functional.Y<int, long>(self => n => n < 2 ? n : self(n - 1) + self(n - 2));
Console.WriteLine(fibNaive(30)); // slow for larger n
```

Memoized fibonacci (polynomial/time-linear-ish):

```csharp
var fibMemo = Functional.MemoizeFix<int, long>(self => n => n < 2 ? n : self(n - 1) + self(n - 2));
Console.WriteLine(fibMemo(100)); // fast and uses caching
```

### 3) Memoized function with two arguments (example: simple combinatorics)

```csharp
var choose = Functional.MemoizeFix<int, int, long>(chooseFunc => (n, k) =>
    k == 0 || k == n ? 1 : chooseFunc(n - 1, k - 1) + chooseFunc(n - 1, k));

Console.WriteLine(choose(30, 15));
```

### 4) Ackermann (demonstrates deep recursion)

```csharp
var ackermann = Functional.Y<(int m, int n), int>(ack => t =>
    t.m == 0 ? t.n + 1 :
    t.n == 0 ? ack((t.m - 1, 1)) :
    ack((t.m - 1, ack((t.m, t.n - 1)))));

Console.WriteLine(ackermann((2, 3))); // 9
```

## Remarks and Implementation Notes

- Memoization store: `MemoizeFix` uses `ConcurrentDictionary` keyed by the function arguments. For multi-argument overloads a tuple (e.g. `(TArg1, TArg2)`) is used as key.

- Key equality: The quality of memoization depends on correct, stable equality semantics for the key types. For value types and tuples of value types the default equality is fine. For reference types, cached results depend on the reference identity or the type's `Equals`/`GetHashCode` implementations.

- Memory growth: The cache grows for each distinct input key. For unbounded input spaces or long-running programs you may accumulate a large cache. If you need eviction or bounded caches, implement a custom memoization layer or use a caching library.

- Thread-safety: The provided memoization is thread-safe thanks to `ConcurrentDictionary`. Concurrent calls for the same key may cause the value factory to run multiple times briefly (depending on the `GetOrAdd` usage), but `ConcurrentDictionary` ensures eventual single cached value.

- Stack depth and tail recursion: These helpers do not transform recursion into iteration. Deep recursion may cause `StackOverflowException`. Consider rewriting very deep recursions into iterative algorithms.

- Debugging: When using anonymous recursion, stack traces will still contain method frames for the compiled delegate; debugging may be less direct than named methods.

## Performance Considerations

- First call cost: Building the recursive delegate with `Y` is negligible. For memoized functions, the overhead of dictionary lookups is modest compared to recomputing expensive recursive calls.

- When memoization yields benefit: Use `MemoizeFix` when the recursion exhibits overlapping subproblems (e.g., Fibonacci, dynamic programming). For pure divide-and-conquer algorithms that split into mostly independent subproblems (e.g., quicksort partitioning), memoization usually provides no benefit and increases memory use.

## Pitfalls and Gotchas

- Avoid memoizing functions with mutable or large object arguments unless their equality/hash semantics are stable and intended.

- Beware of closures capturing large objects that keep them alive longer due to cache entries.

- If your arguments are not value types or tuples, consider projecting them to a stable cache key (e.g., an immutable identifier) before memoizing.

## Testing and Examples in Project

There are unit tests that demonstrate usage patterns for `Functional` helpers. Look at `BrightSword.SwissKnife.Tests` to find reference examples and property-based tests.

## Suggested Enhancements (if needed)

- Add bounded/evicting caches for `MemoizeFix` (LRU or time-based eviction).
- Accept a custom `IEqualityComparer<TKey>` or key selector to control key equality semantics.
- Provide a way to clear the memoization cache when inputs are known to be transient.

---

This document reflects the implementation in `Functional.cs`. Keep examples synchronized with source examples and unit tests when modifying the implementation.