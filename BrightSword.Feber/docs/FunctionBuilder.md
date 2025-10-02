# FunctionBuilder

Purpose

Function builders fold per-property expressions into a single result value using a seed and conjunction (fold) operation. They are perfect for computing aggregates across properties: sums, logical conjunctions, counts, or string concatenation.

Contract
- Inputs: `OperationExpressions` (sequence of Expression representing each property operation), a Seed value (constant expression) and a Conjunction function `Func<Expression, Expression, Expression>`.
- Output: a compiled `Func<...>` delegate that accepts instance parameters and returns the folded `TResult`.

Common members
- `Function` property (cached) — returns the compiled delegate.
- `BuildFunction()` — protected virtual method that constructs the body by `OperationExpressions.Aggregate(Expression.Constant(Seed), Conjunction)` and then compiles a lambda.


Comprehensive example — binary EqualityComparer
```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

public class Person { public string Name { get; set; } public int Age { get; set; } }

// A binary FunctionBuilder that produces Func<TLeft,TRight,bool> by folding per-property comparisons.
private sealed class EqualityComparerBuilder<TProto, TLeft, TRight> : FunctionBuilder<TProto, TLeft, TRight, bool>
{
	// Start with "true" and AND each per-property equality result
	protected override bool Seed => true;

	protected override Func<Expression, Expression, Expression> Conjunction => Expression.AndAlso;

	// Build an expression that compares the property on the left and right instances
	protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression leftInstanceParameter, ParameterExpression rightInstanceParameter)
	{
		var left = Expression.Property(leftInstanceParameter, propertyInfo);
		var right = Expression.Property(rightInstanceParameter, propertyInfo);
		return Expression.Equal(left, right);
	}
}

// Usage
var comparer = new EqualityComparerBuilder<Person, Person, Person>().Function;
var a = new Person { Name = "Ada", Age = 30 };
var b = new Person { Name = "Ada", Age = 30 };
bool allEqual = comparer(a, b); // true
```

Comprehensive example — NullChecker (any property is null)
```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

public class Person { public string Name { get; set; } public int? Age { get; set; } }

// Unary FunctionBuilder that returns true if any of the inspected properties are null.
private sealed class NullChecker<TProto, TInstance> : FunctionBuilder<TProto, TInstance, bool>
{
	// Seed is false (no nulls seen yet); we OR each per-property "is null" check
	protected override bool Seed => false;

	protected override Func<Expression, Expression, Expression> Conjunction => Expression.OrElse;

	// Build an expression that yields (Expression.Property(instance, p) == null)
	protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
	{
		var prop = Expression.Property(instanceParameter, propertyInfo);
		// If property is a value type, compare to its nullable default via Convert
		if (propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(propertyInfo.PropertyType) is null)
		{
			// Non-nullable value types cannot be null -> result is false for this property
			return Expression.Constant(false);
		}

		var nullConstant = Expression.Constant(null, propertyInfo.PropertyType);
		return Expression.Equal(prop, nullConstant);
	}
}

// Usage
var nullChecker = new NullChecker<Person, Person>().Function;
var p1 = new Person { Name = "Ada", Age = 30 };
var p2 = new Person { Name = null, Age = 25 };
Console.WriteLine(nullChecker(p1)); // False
Console.WriteLine(nullChecker(p2)); // True (Name is null)
```

Testing and edge-cases
- Ensure `Seed` is appropriate for the type and `Conjunction` produces a type-compatible result. Use `Expression.Convert` if necessary to coerce types.
- For large numbers of properties or expensive expressions, consider performance implications of expression compilation and caching.

See also
- Concrete unit test demonstrating the NullChecker pattern: `BrightSword.Feber.Tests/NullCheckBuilderTests.cs` (contains a `NullCheckBuilder<T>` test exercising `FunctionBuilder<T,T,bool>` with property-based inputs).

