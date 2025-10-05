# TypeExtensions

## Purpose

Utilities for working with `System.Type` and reflection, implemented in `TypeExtensions.cs`. These helpers provide friendly type names and allow enumeration of members including those inherited via interfaces.

## When to Use

- When you need human-friendly type names (especially for generics) for display or logging.
- When you need to enumerate properties/methods/events of an interface including inherited interfaces.

## API Reference

- `string PrintableName(this Type @this)` — Returns a readable name for the type; for generic types it includes type parameters using recursive `PrintableName` calls.

- `string Name(this Type @this)` — Backwards-compatible alias for `PrintableName` used in older code/tests.

- `string RenameToConcreteType(this Type @this)` — Heuristic to map interface names to concrete-like names (trims leading `I` if followed by uppercase letter) while preserving generic notation.

- `IEnumerable<PropertyInfo> GetAllProperties(this Type @this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)` — Returns properties including those inherited through interfaces; for non-interfaces it removes `DeclaredOnly` to include inherited properties.

- `IEnumerable<MethodInfo> GetAllMethods(this Type @this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)` — Same as above for methods.

- `IEnumerable<EventInfo> GetAllEvents(this Type @this, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)` — Same as above for events.

## Examples

```csharp
var name = typeof(Dictionary<string, int>).PrintableName(); // "Dictionary<String, Int32>"
var n = typeof(IList<int>).RenameToConcreteType(); // "List<Int32>" or "IList<Int32>" based on heuristic

foreach (var p in typeof(IMyInterface).GetAllProperties()) Console.WriteLine(p.Name);
```

## Implementation Notes

- `PrintableName` strips the generic arity suffix (the `\``) from generic type definitions and recursively formats type arguments.
- Interface member enumeration prevents cycles by tracking processed interface types in a `HashSet<Type>` when recursing.
- For non-interface types, the helper ensures inherited members are included by unsetting `DeclaredOnly` from the incoming binding flags.

## Remarks

- These helpers are intended to be small and predictable. They do not attempt to produce C#-exact type syntax (e.g., they use type `Name` rather than `FullName`).
- Use `PrintableName` for logs, diagnostics, or user-facing type descriptions.

---

This document matches the source in `TypeExtensions.cs`.
