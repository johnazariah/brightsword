# ActionBuilder

Purpose

Utility for creating Action<> delegates from a set of expression fragments. The builder composes expression trees and compiles them to efficient delegates, caching compiled delegates where appropriate.

Public API (conceptual)

- ActionBuilder<TInstance> (builder type)
- Action<TInstance> Build(ActionBuilder<TInstance> builder) // compile or get cached delegate

Examples

```csharp
// Build an action that writes a property value to console
var builder = new ActionBuilder<MyType>();
builder.AddExpression(x => Console.WriteLine(x.SomeProperty));
var action = builder.Compile();
action(instance);
```

Remarks

ActionBuilder is optimized for scenarios where the same operation must be executed repeatedly with minimal overhead; compiled delegates are cached to avoid repeated compilation cost.
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
