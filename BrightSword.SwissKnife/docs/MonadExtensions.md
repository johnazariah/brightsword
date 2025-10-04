# MonadExtensions

## Purpose
Provides monadic extension methods for conditional and safe invocation patterns on reference types. These helpers simplify null-safe invocation and conditional logic for reference types, and enable LINQ query syntax for simple monad types.

## When to Use
- When you need to safely invoke functions or actions on reference types that may be null.
- When you want to conditionally invoke logic based on predicates, with default results for null or failed conditions.
- When you want to use LINQ query syntax with simple monad types (e.g., Option, Maybe) for improved readability and composability.

## How to Use
Use these methods to safely invoke functions or actions, or to conditionally execute logic based on predicates. Implement <code>Select</code>, <code>SelectMany</code>, and <code>Where</code> for your monad type to enable LINQ syntax.

## Key APIs
- <code>Maybe<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultResult = default)</code>: Safely invokes a function on a reference type if it is not null, otherwise returns a default result.
- <code>Maybe<T>(this T @this, Action<T> action)</code>: Safely invokes an action on a reference type if it is not null.
- <code>When<T, TResult>(this T @this, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default)</code>: Invokes a function if the predicate is true, otherwise returns a default result.
- <code>When<T>(this T @this, Func<T, bool> predicate, Action<T> action)</code>: Invokes an action if the predicate is true.
- <code>Unless<T, TResult>(this T @this, Func<T, bool> predicate, Func<T, TResult> func, TResult defaultResult = default)</code>: Invokes a function if the predicate is false, otherwise returns a default result.
- <code>Unless<T>(this T @this, Func<T, bool> predicate, Action<T> action)</code>: Invokes an action if the predicate is false.

## Examples
```csharp
string s = null;
int len = s.Maybe(str => str.Length, 0); // 0
string s2 = "hello";
s2.Maybe(str => Console.WriteLine(str)); // prints "hello"
string s3 = "abc";
int len2 = s3.When(str => str.Length > 2, str => str.Length, -1); // 3
s3.When(str => str.Length > 2, str => Console.WriteLine(str)); // prints "abc"
int len3 = s3.Unless(str => str.Length > 5, str => str.Length, -1); // 3
s3.Unless(str => str.Length > 5, str => Console.WriteLine(str)); // prints "abc"

// LINQ query syntax with a simple monad type (Option/Maybe)
var result = from x in Maybe.DoSomething()
             from y in Maybe.DoMore(x)
             select x + y;
```

## LINQ Syntax for Monads
By implementing <code>Select</code>, <code>SelectMany</code>, and <code>Where</code> extension methods for your monad type (such as <code>Option</code> or <code>Maybe</code>), you can use LINQ query syntax to chain operations in a readable and composable way. This is especially useful for error handling, null propagation, and functional composition.

## Remarks
These helpers are especially useful for null-safe invocation and conditional logic in fluent or functional code. They improve readability and reduce boilerplate for common patterns, and enable LINQ query syntax for simple monads.
