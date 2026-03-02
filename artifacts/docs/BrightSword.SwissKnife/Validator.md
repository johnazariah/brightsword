# Validator

## Purpose

Legacy guard helpers implemented in `Validator.cs`. These extension methods provide succinct guard/check semantics but are marked `[Obsolete]` in source. They throw exceptions when conditions or predicates fail.

## When to Use

Prefer modern guard patterns in new code (e.g., `ArgumentNullException.ThrowIfNull`, `ArgumentException`, `Debug.Assert`). Use these only for compatibility with existing code that relies on them.

## API

- `void Check(this bool condition, string message = null)` — Throws `InvalidOperationException` when `condition` is false.
- `void Check<TException>(this bool condition, string message = null) where TException : Exception, new()` — Throws `TException` (attempts to construct with `message` first, falls back to default ctor and then `InvalidOperationException`).
- `void Check(this Func<bool> predicate, string message = null)` — Throws `InvalidOperationException` when predicate returns false.
- `void Check<TException>(this Func<bool> predicate, string message = null) where TException : Exception, new()` — Throws typed exception when predicate fails.

## Examples

```csharp
Validator.Check(value != null, "value required");
Validator.Check<ArgumentNullException>(value != null, "value cannot be null");
Validator.Check(() => value > 0, "must be positive");
```

## Remarks

- These helpers are maintained for legacy compatibility and are intentionally marked `[Obsolete]` to encourage modern alternatives.
- `Check<TException>` uses `Activator.CreateInstance(typeof(TException), message)` when possible; if that fails it falls back to `new TException()` and finally an `InvalidOperationException` with the message.
