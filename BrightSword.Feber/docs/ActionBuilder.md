# ActionBuilder

Purpose

Action builders compose per-property operation expressions into a single expression block and compile that block into an Action-style delegate. The core pattern is used when you need to execute a side-effecting operation for each property (for example: copying properties, pretty-printing, or assigning values from another instance).

Contract (tiny)
- Inputs: a sequence of Expression fragments (see `OperationExpressions`) and one or more ParameterExpression instances representing the delegate parameters.
- Output: a compiled Action delegate (e.g., `Action<TInstance>` or `Action<TLeft,TRight>`).
- Error modes: compilation throws if an Expression references ParameterExpressions not declared on the lambda. Builders keep ParameterExpression instances stable to avoid this.

Common members
- `Action` property (cached) — returns a compiled delegate built from `OperationExpressions`.
- `BuildAction()` — protected virtual method that composes `OperationExpressions` into an Expression.Block and calls `Expression.Lambda<...>(...).Compile()`.

Minimal example (copyable)
```csharp
using System;
using BrightSword.Feber.Core;

public class Person { public string Name { get; set; } public int Age { get; set; } }

// Implement a small pretty-printer using ActionBuilder
private sealed class PrettyPrinterBuilder : ActionBuilder<Person, Person>
{
	protected override Expression PropertyExpression(System.Reflection.PropertyInfo propertyInfo, System.Linq.Expressions.ParameterExpression instanceParameter)
	{
		var member = System.Linq.Expressions.Expression.Property(instanceParameter, propertyInfo);
		var write = System.Linq.Expressions.Expression.Call(typeof(Console), "WriteLine", Type.EmptyTypes, System.Linq.Expressions.Expression.Constant(propertyInfo.Name + ": {0}"), member);
		return write;
	}
}

var builder = new PrettyPrinterBuilder();
builder.Action(new Person { Name = "Ada", Age = 30 });
```

Testing and edge-cases
- Keep the builder's ParameterExpression instances stable. The base classes in this repo expose `InstanceParameterExpression` / `LeftInstanceParameterExpression` / `RightInstanceParameterExpression` so override implementations should use those, not create new ParameterExpression objects.
- Avoid expression fragments that reference undeclared variables. If you need temporary variables inside the block, create them as Block variables and include them in the block explicitly.

When to customize
- Override `BuildAction()` to change compilation behavior (for example to produce a delegate that captures additional state, or to use `Compile(preferInterpretation:true)` in low-memory scenarios).

