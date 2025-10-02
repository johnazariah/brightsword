# Functional

Purpose

Small, focused functional utilities used to improve expression clarity across the codebase.

Public API

- T Identity<T>(T x)
- T Tap<T>(T x, Action<T> sideEffect)
- Func<T2,T1,TR> Curry/Uncurry helpers (where present)

Examples

```csharp
var five = Functional.Identity(5);
var obj = Functional.Tap(myObj, o => Logger.Debug(o));

// Curry example (if available in this helper set)
var cur = Functional.Curry((int x,int y) => x + y);
var add5 = cur(5);
int sum = add5(3); // 8
```

Remarks

These functions are intentionally small so they can be inlined and mixed with imperative code without performance surprises.
