using System.Linq.Expressions;

namespace BrightSword.Feber.Core
{
    /// <summary>
    /// Base class for building unary function delegates that fold per-property expressions into a single result.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TInstance">Instance type passed to the produced function.</typeparam>
    /// <typeparam name="TResult">Result type returned by the function.</typeparam>
    /// <remarks>
    /// <para>
    /// Use <see cref="UnaryFunctionBuilderBase{TProto, TInstance, TResult}"/> to generate a <c>Func&lt;TInstance, TResult&gt;</c>
    /// that aggregates property values using a seed and a conjunction function. This is ideal for scenarios like summing, combining, or validating properties.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Sum all integer properties of an object.
    /// public sealed class SumBuilder<TProto> : FunctionBuilder<TProto, TProto, int>
    /// {
    ///     protected override int Seed => 0;
    ///     protected override Func<Expression, Expression, Expression> Conjunction => Expression.Add;
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
    ///     {
    ///         var member = Expression.Property(instanceParameter, propertyInfo);
    ///         return Expression.Convert(member, typeof(int));
    ///     }
    /// }
    /// var sumFunc = new SumBuilder<MyProto>().Function;
    /// int total = sumFunc(myProtoInstance);
    /// </code>
    /// </example>
    public abstract class UnaryFunctionBuilderBase<TProto, TInstance, TResult> : UnaryOperationBuilderBase<TProto, TInstance>
    {
        protected abstract TResult Seed { get; }
        protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
    }

    /// <summary>
    /// Base class for building binary function delegates that fold per-property expressions from two instances into a single result.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    /// <typeparam name="TResult">Result type returned by the function.</typeparam>
    /// <remarks>
    /// <para>
    /// Use <see cref="BinaryFunctionBuilderBase{TProto, TLeftInstance, TRightInstance, TResult}"/> to generate a <c>Func&lt;TLeftInstance, TRightInstance, TResult&gt;</c>
    /// that aggregates property values from two objects. This is useful for comparing, merging, or diffing objects.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Count matching integer properties between two objects.
    /// public sealed class MatchCountBuilder<TProto> : FunctionBuilder<TProto, TProto, TProto, int>
    /// {
    ///     protected override int Seed => 0;
    ///     protected override Func<Expression, Expression, Expression> Conjunction => Expression.Add;
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression left, ParameterExpression right)
    ///     {
    ///         var leftProp = Expression.Property(left, propertyInfo);
    ///         var rightProp = Expression.Property(right, propertyInfo);
    ///         var eq = Expression.Equal(leftProp, rightProp);
    ///         return Expression.Condition(eq, Expression.Constant(1), Expression.Constant(0));
    ///     }
    /// }
    /// var matchFunc = new MatchCountBuilder<MyProto>().Function;
    /// int matches = matchFunc(obj1, obj2);
    /// </code>
    /// </example>
    public abstract class BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult> : BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
    {
        protected abstract TResult Seed { get; }
        protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
    }

#pragma warning disable CA1716 // Keep member name 'Function' for compatibility
    /// <summary>
    /// Builds a unary function from per-property operation expressions and composes them using a seed and conjunction function.
    /// </summary>
    /// <typeparam name="TProto">Prototype type used to discover properties.</typeparam>
    /// <typeparam name="TInstance">Instance type passed to the produced function.</typeparam>
    /// <typeparam name="TResult">Result type returned by the function.</typeparam>
    /// <remarks>
    /// Subclasses provide a seed value and a conjunction function that folds the sequence of <see cref="OperationExpressions"/> into a single expression body.
    /// The compiled function is cached and returned from the <see cref="Function"/> property.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: compute sum of integer properties declared by TProto on an instance.
    /// public sealed class SumBuilder<TProto> : FunctionBuilder<TProto, TProto, int>
    /// {
    ///     protected override int Seed => 0;
    ///     protected override Func<Expression, Expression, Expression> Conjunction => (left, right) => Expression.Add(left, right);
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
    ///     {
    ///         var member = Expression.Property(instanceParameter, propertyInfo);
    ///         return Expression.Convert(member, typeof(int));
    ///     }
    /// }
    /// var f = new SumBuilder<MyProto>().Function;
    /// var result = f(myProtoInstance);
    /// </code>
    /// </example>
    public abstract class FunctionBuilder<TProto, TInstance, TResult> : UnaryFunctionBuilderBase<TProto, TInstance, TResult>
    {
        private Func<TInstance, TResult> _function;

        /// <summary>
        /// Gets the compiled <see cref="Func{TInstance, TResult}"/> produced from the composed operation expressions.
        /// </summary>
        public virtual Func<TInstance, TResult> Function => _function ??= BuildFunction();

        /// <summary>
        /// Builds the function by folding <see cref="OperationExpressions"/> starting from <see cref="Seed"/> via <see cref="Conjunction"/> and compiling the resulting expression.
        /// </summary>
        protected virtual Func<TInstance, TResult> BuildFunction()
        {
            var seedExpr = Expression.Constant(Seed);
            var body = OperationExpressions.Aggregate(seedExpr, Conjunction);
            return Expression.Lambda<Func<TInstance, TResult>>(body, InstanceParameterExpression).Compile();
        }
    }
#pragma warning restore CA1716

#pragma warning disable CA1716 // Keep member name 'Function' for compatibility
    /// <summary>
    /// Builds a binary function that consumes left/right instances and produces a folded result using Seed and Conjunction.
    /// </summary>
    /// <typeparam name="TProto">Prototype type used to discover properties.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    /// <typeparam name="TResult">Result type returned by the function.</typeparam>
    /// <remarks>
    /// Subclasses provide a seed value and a conjunction function that folds the sequence of <see cref="OperationExpressions"/> into a single expression body.
    /// The compiled function is cached and returned from the <see cref="Function"/> property.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Count matching integer properties between two objects.
    /// public sealed class MatchCountBuilder<TProto> : FunctionBuilder<TProto, TProto, TProto, int>
    /// {
    ///     protected override int Seed => 0;
    ///     protected override Func<Expression, Expression, Expression> Conjunction => Expression.Add;
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression left, ParameterExpression right)
    ///     {
    ///         var leftProp = Expression.Property(left, propertyInfo);
    ///         var rightProp = Expression.Property(right, propertyInfo);
    ///         var eq = Expression.Equal(leftProp, rightProp);
    ///         return Expression.Condition(eq, Expression.Constant(1), Expression.Constant(0));
    ///     }
    /// }
    /// var matchFunc = new MatchCountBuilder<MyProto>().Function;
    /// int matches = matchFunc(obj1, obj2);
    /// </code>
    /// </example>
    public abstract class FunctionBuilder<TProto, TLeftInstance, TRightInstance, TResult> : BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult>
    {
        private Func<TLeftInstance, TRightInstance, TResult> _function;

        /// <summary>
        /// Gets the compiled <see cref="Func{TLeftInstance,TRightInstance,TResult}"/> produced from the composed operation expressions.
        /// </summary>
        public virtual Func<TLeftInstance, TRightInstance, TResult> Function => _function ??= BuildFunction();

        /// <summary>
        /// Builds the binary function by folding <see cref="OperationExpressions"/> starting from <see cref="Seed"/> via <see cref="Conjunction"/> and compiling the resulting expression.
        /// </summary>
        protected virtual Func<TLeftInstance, TRightInstance, TResult> BuildFunction()
        {
            var seedExpr = Expression.Constant(Seed);
            var body = OperationExpressions.Aggregate(seedExpr, Conjunction);
            return Expression.Lambda<Func<TLeftInstance, TRightInstance, TResult>>(body, LeftInstanceParameterExpression, RightInstanceParameterExpression).Compile();
        }
    }
#pragma warning restore CA1716
}
