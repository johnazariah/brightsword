# FunctionBuilder

Purpose
- Builds Func<T,...> delegates from expression trees and caches the compiled delegates for reuse.

Key APIs
- BuildFunc<T,TReturn>(Expression body, ParameterExpression instance)

Usage
```csharp
var f = FunctionBuilder.BuildFunc<MyType,int>(...);
int r = f(instance);
```

Notes
- Similar caching and compilation patterns to ActionBuilder but returns typed Func delegates.
