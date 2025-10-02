# MonadExtensions

Purpose
- LINQ-friendly monadic extension helpers (Select, SelectMany, Where) for custom monad-like types used across the repo.

Key APIs
- Bind / SelectMany implementations for monadic chaining
- Return/Result helpers

Usage
```csharp
var result = from x in Maybe.DoSomething()
             from y in Maybe.DoMore(x)
             select x + y;
```

Notes
- These functions allow using LINQ query syntax with small monad types (Option / Maybe) to improve readability.
