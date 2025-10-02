# FunctionBuilder

## Purpose

Function builders automate the generation of compiled delegates (`Func<TInstance, TResult>` or `Func<TLeft, TRight, TResult>`) that fold per-property expressions into a single result value. They are ideal for computing aggregates, logical conjunctions, counts, or string concatenations across properties.

## Why Use This Approach?

This builder pattern is designed to **improve runtime performance** for repeated applications of property-based computations. By building and compiling the required expression tree once per type, and caching the resulting delegate, you avoid the overhead of reflection and expression construction on every invocation. The trade-off is a small upfront cost for building and caching the operation, which is quickly amortized when the operation is used many times.

- **Performance:** Compiled delegates execute much faster than repeated reflection or dynamic code generation.
- **Scalability:** The cost of building the operation is paid only once per type; subsequent invocations are as fast as a direct delegate call.
- **Suitability:** This pattern is ideal for scenarios where the same computation will be applied to many instances of the same type (e.g., aggregation, validation, comparison).
- **Developer Productivity:** This approach **reduces code-bloat and improves developer productivity** by eliminating repetitive boilerplate code. Instead of writing custom logic to compute across every property of a type, you can automate the generation of these computations, making your codebase cleaner, easier to maintain, and less error-prone.

## Architectural Role
- Composes per-property operation expressions into a single folded result using a seed and conjunction function.
- Compiles the folded expression into a cached delegate for efficient repeated execution.
- Used as the foundation for utilities like equality comparers, null checkers, and aggregators.

## Contract
- **Inputs:** Sequence of property-based `Expression` fragments (`OperationExpressions`), a seed value, and a conjunction function (`Func<Expression, Expression, Expression>`).
- **Output:** Compiled `Func` delegate.
- **Error modes:** Ensure type compatibility between seed, property expressions, and conjunction results.

## Common Members
- `Function` property (cached) — returns the compiled delegate.
- `BuildFunction()` — protected virtual method that folds and compiles the block.

## Usage Examples

### Binary FunctionBuilder (Equality Comparer)
```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

public class Person { public string Name { get; set; } public int Age { get; set; } }

private sealed class EqualityComparerBuilder<TProto, TLeft, TRight> : FunctionBuilder<TProto, TLeft, TRight, bool>
{
    protected override bool Seed => true;
    protected override Func<Expression, Expression, Expression> Conjunction => Expression.AndAlso;
    protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression left, ParameterExpression right)
    {
        var leftProp = Expression.Property(left, propertyInfo);
        var rightProp = Expression.Property(right, propertyInfo);
        return Expression.Equal(leftProp, rightProp);
    }
}

var comparer = new EqualityComparerBuilder<Person, Person, Person>().Function;
bool allEqual = comparer(a, b); // true if all properties are equal
```

### Unary FunctionBuilder (Null Checker)
```csharp
private sealed class NullChecker<TProto, TInstance> : FunctionBuilder<TProto, TInstance, bool>
{
    protected override bool Seed => false;
    protected override Func<Expression, Expression, Expression> Conjunction => Expression.OrElse;
    protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
    {
        var prop = Expression.Property(instanceParameter, propertyInfo);
        if (propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) is null)
            return Expression.Constant(false);
        var nullConstant = Expression.Constant(null, propertyInfo.PropertyType);
        return Expression.Equal(prop, nullConstant);
    }
}

var nullChecker = new NullChecker<Person, Person>().Function;
Console.WriteLine(nullChecker(p1)); // True if any property is null
```

## When to Use
- When you need to compute an aggregate or folded result across properties (sum, count, logical AND/OR, etc.).
- When you want to automate delegate generation for property-based computations.
- When you want to maximize performance for repeated computations on the same type.
- When you want to eliminate repetitive boilerplate and make your codebase more maintainable.

## Customization
- Override `BuildFunction()` to change folding or compilation behavior.
- Ensure `Seed` and `Conjunction` are type-compatible with property expressions.

## Testing and Pitfalls
- **Type compatibility:** Use `Expression.Convert` if necessary to coerce types.
- **Delegate caching:** The base classes cache compiled delegates for efficiency.
- **Performance:** For large types, consider the cost of expression compilation and delegate invocation.

## See Also
- [ActionBuilder.md](ActionBuilder.md) — for side-effecting property operations.
- [OperationBuilders.md](OperationBuilders.md) — for low-level property scanning and expression generation.

