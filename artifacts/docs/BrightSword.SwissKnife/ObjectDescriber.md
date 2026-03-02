# ObjectDescriber

## Purpose

Small expression-based helpers implemented in `ObjectDescriber.cs`. Provides `GetName(...)` overloads that extract member or method names from expression trees.

## API

- `string GetName(Expression<Action> e)`
- `string GetName<TResult>(Expression<Func<TResult>> selector)`
- `string GetName<TArg, TResult>(Expression<Func<TArg, TResult>> selector)`
- `string GetName<TArg>(Expression<Action<TArg>> selector)`

## Examples

```csharp
class Person { public string Name { get; set; } public void Update() { } }

var propName = ObjectDescriber.GetName<Person, string>(x => x.Name); // "Name"
var methodName = ObjectDescriber.GetName(() => new Person().Update()); // "Update"
```

## Remarks

- The helper expects simple member or method call expressions and throws `NotSupportedException` when passed expressions it cannot analyze.
- Useful in logging, test assertions, and scenarios where code-based member name retrieval avoids string literals.
