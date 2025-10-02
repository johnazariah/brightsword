# OperationBuilders

Purpose

Operation builders are the low-level primitives used by the Feber library to generate per-property Expression fragments. Higher-level builders (ActionBuilder/FunctionBuilder) consume `OperationExpressions` and compile complete delegates.

Core types & behaviors
- `OperationBuilderBase<TProto>`
	- Scans `typeof(TProto)` for public instance properties using `GetAllProperties` (SwissKnife helper).
	- Exposes `FilteredProperties` and `OperationExpressions` which map properties to Expressions via `BuildPropertyExpression`.

- `UnaryOperationBuilderBase<TProto, TInstance>`
	- Exposes a stable `InstanceParameterExpression` (static field) so that compiled lambdas reuse the same ParameterExpression instance.
	- Implementors override `PropertyExpression(PropertyInfo, ParameterExpression)` to return a per-property Expression referencing the provided `instanceParameter`.

- `BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>`
	- Supplies `LeftInstanceParameterExpression` and `RightInstanceParameterExpression` and expects implementors to override `PropertyExpression(PropertyInfo, ParameterExpression, ParameterExpression)`.

Worked example: null-check builder (complete)
```csharp
using System;
using System.Linq.Expressions;
using System.Reflection;

public sealed class NullCheckBuilder<T> : UnaryOperationBuilderBase<T, T>
{
		protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
		{
				var propertyAccess = Expression.Property(instanceParameter, propertyInfo);
				var nullConstant = Expression.Constant(null, propertyInfo.PropertyType);
				return Expression.Equal(propertyAccess, nullConstant);
		}

		public Expression BuildAll() => Expression.Block(OperationExpressions);
}

// Usage
// var builder = new NullCheckBuilder<MyType>();
// var block = builder.BuildAll();
// var lam = Expression.Lambda<Action<MyType>>(block, builder.InstanceParameterExpression);
// var action = lam.Compile();

Testing guidance and pitfalls
- ParameterExpression identity: The expression compiler requires that the ParameterExpression referenced in the body matches the lambda parameter instance. Use the provided `InstanceParameterExpression` / `LeftInstanceParameterExpression` / `RightInstanceParameterExpression` instead of creating new parameters inside overrides.
- Avoid sharing ParameterExpression instances across independent builder instances unless intended; the base classes supply static fields for common cases to keep things simple.
- If tests need to compile the final block, obtain the same ParameterExpression instance used by the builder (via the protected property) or compile per-property lambdas rather than the full block.

Cross-links
- See `ActionBuilder.md` for examples that compose `OperationExpressions` into side-effecting `Action` delegates.
- See `FunctionBuilder.md` for examples that fold `OperationExpressions` into aggregate `Func` results.

