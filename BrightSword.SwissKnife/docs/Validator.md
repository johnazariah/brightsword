# Validator

## Purpose
Provides guard and validation helpers for runtime argument and condition checking. These helpers throw exceptions when conditions or predicates fail, simplifying defensive programming.

## When to Use
- When you need to validate arguments, parameters, or object state at runtime.
- When you want to centralize guard patterns and consistent exception messages.

## How to Use
Use these methods to check conditions and throw exceptions if validation fails. You can specify the exception type or use the default.

## Key APIs
- <code>Check(this bool condition, string message = null)</code>: Throws an Exception if the condition is false.
- <code>Check<TException>(this bool condition, string message = null)</code>: Throws a specific exception type if the condition is false.
- <code>Check(this Func<bool> predicate, string message = null)</code>: Throws an Exception if the predicate returns false.
- <code>Check<TException>(this Func<bool> predicate, string message = null)</code>: Throws a specific exception type if the predicate returns false.

## Examples
```csharp
Validator.Check(x > 0, "x must be positive");
Validator.Check<ArgumentNullException>(obj != null, "obj cannot be null");
Validator.Check(() => value > 0, "value must be positive");
Validator.Check<ArgumentException>(() => value > 0, "value must be positive");
```

## Remarks
Centralizes guard patterns and consistent exception messages used across the codebase. These helpers are useful for defensive programming and runtime validation.
