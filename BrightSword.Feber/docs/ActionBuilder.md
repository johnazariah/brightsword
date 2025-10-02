# ActionBuilder

Purpose
- Provides helpers to construct and cache compiled Action<T...> delegates from expression trees composed by the library.

Key APIs
- BuildAction<T>(Expression body, ParameterExpression instance): compiles the expression block into a cached Action<T> delegate.

Usage
```csharp
var builder = new ActionBuilder<MyType>(...);
var action = builder.Compile();
action(instance);
```

Notes
- Builders cache compiled delegates to reduce runtime compilation overhead.
