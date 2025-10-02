# Disposable

Purpose
- Lightweight IDisposable helpers to simplify creation of dispose actions and wrapping using statements.

Key APIs
- Disposable.Create(Action disposeAction) — returns an IDisposable that runs the action.
- Using pattern helpers to run disposable logic inline.

Usage
```csharp
using(Disposable.Create(() => Cleanup())) {
  // do work
}
```

Notes
- These helpers reduce boilerplate for simple disposable actions and are useful in tests and short-lived scopes.
