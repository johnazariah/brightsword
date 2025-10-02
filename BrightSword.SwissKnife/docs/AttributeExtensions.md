# AttributeExtensions

Purpose

Helpers to simplify retrieving custom attributes via reflection. These extensions are null-safe and provide convenient generic overloads.

Public API (examples)

- T? GetCustomAttribute<T>(this MemberInfo member, bool inherit = true)
- IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member, bool inherit = true)

Examples

```csharp
// Get a single attribute from a type
var attrib = typeof(MyClass).GetCustomAttribute<MyAttribute>();

// Get all matching attributes from a member
var attrs = typeof(MyClass).GetCustomAttributes<MyAttribute>(inherit: false);
foreach (var a in attrs) Console.WriteLine(a.SomeProperty);
```

Remarks

These are convenience wrappers over System.Reflection that reduce casting/boilerplate when working with attributes. They return empty sequences or null where appropriate to keep call sites simple.
