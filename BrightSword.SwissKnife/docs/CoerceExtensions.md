# CoerceExtensions

## Purpose
Provides extension methods for type coercion and conversion with fallback/default handling. These helpers simplify safe type conversion, including enums and primitives, with custom parsing logic.

## When to Use
- When you need to safely convert values between types, with sensible fallbacks and safe parsing semantics.
- When you want to avoid exceptions on parse errors and return fallback values instead.

## How to Use
Use these methods to coerce or convert values between types, specifying default values for failure cases.

## Key APIs
- <code>CoerceType(this object value, Type targetType, object defaultValue)</code>: Attempts to coerce an object to the specified target type, using default value if conversion fails.
- <code>CoerceType<T>(this object value, Type targetType, out object returnValue, Func<Type, object, T> parseFunc, object defaultValue)</code>: Attempts to coerce an object to the specified target type using a custom parse function.
- <code>CoerceType(this object value, Type targetType, out object returnValue, Func<Type, bool> checkFunc, Func<Type, object, object> parseFunc, object defaultValue)</code>: Attempts to coerce an object to the specified target type using custom check and parse functions.

## Examples
```csharp
object val = "42";
int result = (int)val.CoerceType(typeof(int), 0); // 42
object val2 = "true";
bool success = val2.CoerceType<bool>(typeof(bool), out var result2, (_, v) => bool.Parse(v.ToString()), false);
object val3 = "42";
bool success2 = val3.CoerceType(typeof(int), out var result3, t => t == typeof(int), (t, v) => int.Parse(v.ToString()), 0);
```

## Remarks
These extensions simplify defensive parsing and conversion. They do not throw on parse errors and return fallback values instead. Prefer TryParse patterns and culture-invariant parsing where appropriate.
