# TypeExtensions

## Purpose
Helpers that reduce boilerplate when interrogating types via reflection.

## When to Use
- When you need to inspect type structure, properties, or generic arguments via reflection.
- When you want to centralize and reduce boilerplate for type interrogation logic.

## How to Use
Use these methods to determine if a type is nullable, to get the element type of an enumerable, or to check if an enumerable is of a specific type.

## Key APIs
- `bool IsNullable(this Type type)`: Determines if a type is a nullable type.
- `Type? GetElementTypeIfEnumerable(this Type type)`: Gets the element type if the type is an enumerable.
- `bool IsEnumerableOfType(this Type type, out Type? elementType)`: Checks if the type is an enumerable of a specific type.

## Examples
```csharp
if (someType.IsNullable()) { /* handle nullable */ }

var itemType = listType.GetElementTypeIfEnumerable();
if (itemType != null) { /* handle item type */ }

if (listType.IsEnumerableOfType(out var t)) Console.WriteLine(t.Name);
```

## Remarks
These helpers centralize common reflection patterns and improve testability of code that depends on type structure. They are especially useful in libraries and frameworks that need to inspect or manipulate types dynamically.
