# CoerceExtensions

## Purpose

Provides type coercion and conversion helpers implemented in `CoerceExtensions.cs`. These helpers attempt to convert `object` values to specified target types using a series of parsing strategies and fallbacks.

## When to Use

- When you need a tolerant conversion from loosely-typed values (e.g., deserialized JSON, configuration values) into primitive types or enums.
- When you want a conversion that returns defaults or fallbacks rather than throwing exceptions in common parse-failure scenarios.

Be cautious: these helpers are marked `[Obsolete]` in source with suggestions to prefer `TryParse`/`Convert.ChangeType` and explicit conversion logic for modern code.

## API Reference

- `object CoerceType(this object value, Type targetType, object defaultValue)`
  - Attempts to convert `value` to `targetType`, returning `defaultValue` or a type-appropriate default when conversion fails.

- `bool CoerceType<T>(this object value, Type targetType, out object returnValue, Func<Type, object, T> parseFunc, object defaultValue)`
  - Attempt conversion using a custom parse function specialized for `T`.

- `bool CoerceType(this object value, Type targetType, out object returnValue, Func<Type, bool> checkFunc, Func<Type, object, object> parseFunc, object defaultValue)`
  - Attempt conversion when `checkFunc` returns true for the target type, using `parseFunc` to produce `returnValue`. On exception, falls back to `defaultValue` or a type default.

## How It Works

`CoerceType` implements layered parsing:
- Special-cases for `bool` with tolerant parsing accepting `"y"`/`"n"`.
- Enum parsing using `Enum.Parse` (case-insensitive).
- Sequential attempts to parse as common primitive types (bool, decimal, long, int, short, byte, char, double, float, DateTime) using `Parse` methods with invariant culture.
- Fallback to `Convert.ChangeType` when specific parses fail.
- If all attempts fail, returns the original value or `defaultValue` depending on overload behavior.

## Examples

```csharp
object s = "42";
var val = s.CoerceType(typeof(int), 0); // returns int 42

object b = "y";
var boolVal = b.CoerceType(typeof(bool), false); // true

object e = "Sunday";
var day = e.CoerceType(typeof(DayOfWeek), DayOfWeek.Monday); // DayOfWeek.Sunday
```

## Remarks

- The code is annotated `[Obsolete]` — prefer explicit `TryParse` calls and `Convert.ChangeType` where possible for clarity and maintainability.
- Culture: parsing uses `CultureInfo.InvariantCulture` for numeric and DateTime parsing to avoid culture-dependent surprises.
- Enum parsing is case-insensitive.
- `CoerceType` may return original `value` if conversions fail; review call sites to ensure this behavior is acceptable.

---

This document mirrors the implementation in `CoerceExtensions.cs` and highlights trade-offs and modern alternatives.
