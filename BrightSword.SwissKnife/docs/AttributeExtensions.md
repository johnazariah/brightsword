# AttributeExtensions

Purpose
- Provides helper extension methods for working with attributes on types, members and parameters.

Key APIs
- GetCustomAttribute<T>(this MemberInfo member, bool inherit = true): returns a single attribute or null.
- GetCustomAttributes<T>(this MemberInfo member, bool inherit = true): returns all attributes of type T.

Usage
```csharp
var attr = typeof(MyClass).GetCustomAttribute<MyAttribute>();
foreach(var a in typeof(MyClass).GetCustomAttributes<MyAttribute>()) {
  // use a
}
```

Notes
- These helpers are thin, null-safe wrappers around System.Reflection and are intended to improve readability.
