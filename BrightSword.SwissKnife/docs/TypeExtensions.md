# TypeExtensions

Purpose

Helpers that reduce boilerplate when interrogating types via reflection.

Public API (signatures)

- bool IsNullable(this Type type)
- Type? GetElementTypeIfEnumerable(this Type type)
- bool IsEnumerableOfType(this Type type, out Type? elementType)

Examples

```csharp
if (someType.IsNullable()) { /* handle nullable */ }
var itemType = listType.GetElementTypeIfEnumerable();
if (listType.IsEnumerableOfType(out var t)) Console.WriteLine(t.Name);
```

Remarks

These helpers centralize common reflection patterns and improve testability of code that depends on type structure.
