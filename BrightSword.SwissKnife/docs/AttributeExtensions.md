# AttributeExtensions

## Purpose

Convenience extension methods for discovering and extracting custom attribute instances and values from `Type` and `MemberInfo` using `System.Reflection`.

These wrappers reduce boilerplate by performing casting and null-checks and by returning default values when attributes are not present.

## When to Use

- When you need to read attribute instances or values from types, methods, properties, fields, or other members via reflection.
- When you want compact, null-safe attribute value extraction without repeated `GetCustomAttributes(...).OfType<T>().FirstOrDefault()` code.

## API Reference

- `TAttribute? GetCustomAttribute<TAttribute>(this Type @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute`
  - Returns the first attribute of type `TAttribute` applied to the `Type`, or `null` if not found.

- `TAttribute? GetCustomAttribute<TAttribute>(this MemberInfo @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute`
  - Returns the first attribute of type `TAttribute` applied to the `MemberInfo`, or `null` if not found.

- `TResult GetCustomAttributeValue<TAttribute, TResult>(this Type @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute`
  - Finds the first attribute of type `TAttribute` on the `Type` and returns the projected value via `selector`, or `defaultValue` when the attribute is missing.

- `TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute`
  - Same as above for `MemberInfo`.

## How to Use

- To test presence of an attribute:

```csharp
var has = typeof(MyClass).GetCustomAttribute<ObsoleteAttribute>() != null;
```

- To get an attribute value with a safe fallback:

```csharp
var msg = typeof(MyClass).GetCustomAttributeValue<ObsoleteAttribute, string>(a => a.Message, "");
```

## Examples

```csharp
[Obsolete("Use NewClass instead")]
public class OldClass { }

var attr = typeof(OldClass).GetCustomAttribute<ObsoleteAttribute>();
Console.WriteLine(attr?.Message); // "Use NewClass instead"

var message = typeof(OldClass).GetCustomAttributeValue<ObsoleteAttribute, string>(a => a.Message, "No message");
Console.WriteLine(message); // "Use NewClass instead"
```

## Remarks

- The `flags` parameter is currently not used in the implementation but is retained for API compatibility; passing different binding flags has no effect.
- These helpers return `null` or `default` when attributes are missing — prefer this behavior for concise call sites.
- For performance-sensitive loops, consider caching attribute lookups.

---

Documentation mirrors the helper implementations in `AttributeExtensions.cs` in the source tree.
