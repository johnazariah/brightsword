# ActionBuilder

## Purpose

Action builders automate the generation of compiled delegates (`Action<TInstance>` or `Action<TLeft, TRight>`) that perform side-effecting operations for each property of an object. They are the core pattern for tasks like pretty-printing, copying, mapping, or validating properties in the Feber library.

## Why Use This Approach?

This builder pattern is designed to **improve runtime performance** for repeated applications of property-based operations. By building and compiling the required expression tree once per type, and caching the resulting delegate, you avoid the overhead of reflection and expression construction on every invocation. The trade-off is a small upfront cost for building and caching the operation, which is quickly amortized when the operation is used many times.

- **Performance:** Compiled delegates execute much faster than repeated reflection or dynamic code generation.
- **Scalability:** The cost of building the operation is paid only once per type; subsequent invocations are as fast as a direct delegate call.
- **Suitability:** This pattern is ideal for scenarios where the same operation will be applied to many instances of the same type (e.g., serialization, mapping, printing, copying).
- **Developer Productivity:** This approach **reduces code-bloat and improves developer productivity** by eliminating repetitive boilerplate code. Instead of writing custom logic to operate on every property of a type, you can automate the generation of these operations, making your codebase cleaner, easier to maintain, and less error-prone.

## Architectural Role

- Composes per-property operation expressions into a single expression block.
- Compiles the block into a cached delegate, ensuring efficient repeated execution.
- Exposes stable `ParameterExpression` instances for lambda compilation.
- Used as the foundation for utilities like pretty-printers, copiers, and mappers.

## Contract
- **Inputs:** Sequence of property-based `Expression` fragments (`OperationExpressions`), one or more `ParameterExpression` instances.
- **Output:** Compiled `Action` delegate.
- **Error modes:** Compilation throws if an expression references undeclared parameters. Builders keep `ParameterExpression` instances stable to avoid this.

## Common Members
- `Action` property (cached) — returns the compiled delegate.
- `BuildAction()` — protected virtual method that composes and compiles the block.

## Usage Examples

### Unary ActionBuilder (Pretty Printer)
```csharp
using System;
using BrightSword.Feber.Core;

public class Person { public string Name { get; set; } public int Age { get; set; } }

private sealed class PrettyPrinterBuilder : ActionBuilder<Person, Person>
{
    protected override Expression PropertyExpression(System.Reflection.PropertyInfo propertyInfo, System.Linq.Expressions.ParameterExpression instanceParameter)
    {
        var member = System.Linq.Expressions.Expression.Property(instanceParameter, propertyInfo);
        var toString = System.Linq.Expressions.Expression.Call(member, typeof(object).GetMethod("ToString"));
        return System.Linq.Expressions.Expression.Call(
            typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
            System.Linq.Expressions.Expression.Call(
                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                System.Linq.Expressions.Expression.Constant(propertyInfo.Name + ": "),
                toString));
    }
}

var printer = new PrettyPrinterBuilder().Action;
printer(new Person { Name = "Ada", Age = 30 });
```

### Binary ActionBuilder (Copier)
```csharp
private sealed class CopierBuilder<T> : ActionBuilder<T, T, T>
{
    protected override Expression PropertyExpression(System.Reflection.PropertyInfo propertyInfo, System.Linq.Expressions.ParameterExpression target, System.Linq.Expressions.ParameterExpression source)
    {
        var sourceProp = System.Linq.Expressions.Expression.Property(source, propertyInfo);
        var targetProp = System.Linq.Expressions.Expression.Property(target, propertyInfo);
        return System.Linq.Expressions.Expression.Assign(targetProp, sourceProp);
    }
}

var copier = new CopierBuilder<Person>().Action;
copier(targetInstance, sourceInstance); // Copies properties
```

## When to Use
- When you need to perform a side effect for each property of an object (printing, copying, mapping, etc.).
- When you want to automate delegate generation for property-based operations.
- When you want to maximize performance for repeated operations on the same type.
- When you want to eliminate repetitive boilerplate and make your codebase more maintainable.

## Customization
- Override `BuildAction()` to change compilation behavior (e.g., to capture additional state, or use `Compile(preferInterpretation:true)` for low-memory scenarios).
- Use the provided `InstanceParameterExpression`, `LeftInstanceParameterExpression`, and `RightInstanceParameterExpression` for lambda parameters.

## Testing and Pitfalls
- **ParameterExpression identity:** Always use the builder's provided parameter expressions. Do not create new ones inside overrides.
- **Delegate caching:** The base classes cache compiled delegates for efficiency. Avoid unnecessary recompilation.
- **Expression validity:** Ensure all referenced parameters are declared in the lambda.

## See Also
- [FunctionBuilder.md](FunctionBuilder.md) — for folding/aggregation patterns.
- [OperationBuilders.md](OperationBuilders.md) — for low-level property scanning and expression generation.

