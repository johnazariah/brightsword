# Functional

Purpose
- Small functional helpers used across the codebase (e.g., identity, curry, tap helpers, Maybe/Option helpers).

Key APIs
- Identity<T>(T x)
- Tap<T>(T x, Action<T> sideEffect)

Usage
```csharp
var x = Functional.Identity(5);
Functional.Tap(obj, o => Log(o));
```

Notes
- Keep implementations minimal and predictable to be easily inlined by callers.
