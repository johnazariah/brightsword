# CoerceExtensions

Purpose
- Helpers to coerce or convert values between types with sensible fallbacks and safe parsing semantics.

Key APIs
- CoerceTo<T>(this object obj, T defaultValue = default) — attempt to convert and return default on failure.
- ToIntSafe(this string s, int fallback)

Usage
```csharp
int x = someObj.CoerceTo<int>(0);
int parsed = "123".ToIntSafe(0);
```

Notes
- These extensions simplify defensive parsing and conversion; they do not throw on parse errors and return fallback values instead.

Implementation notes
- Prefer TryParse patterns and culture-invariant parsing where appropriate.
