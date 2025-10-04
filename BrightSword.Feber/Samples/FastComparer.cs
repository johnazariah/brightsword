
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    public static class FastComparer
    {
        public static bool AllPropertiesAreEqualWith<T>(this T This, T other) => FastComparer<T>.AllPropertiesAreEqual(This, other);
    }
#pragma warning disable CA1000 // Allow static members on generic sample types
    public static class FastComparer<T>
    {
#pragma warning disable RCS1250 // allow target-typed new() in samples
        private static readonly FastComparerBuilder _builder = new();
#pragma warning restore RCS1250

        public static bool AllPropertiesAreEqual(T left, T right) => _builder.Function(left, right);

        /// <summary>
        /// Implements a <see cref="FunctionBuilder{TProto,TLeftInstance,TRightInstance,TResult}"/> that compares all public properties for equality.
        /// </summary>
        /// <remarks>
        /// This builder demonstrates the Seed/Conjunction pattern used by <see cref="FunctionBuilder{TProto,TInstance,TResult}"/>:
        /// - <see cref="Seed"/> provides the initial value (true)
        /// - <see cref="Conjunction"/> is <see cref="Expression.AndAlso"/>, folding per-property equality checks into a single boolean expression
        /// - <see cref="PropertyExpression(PropertyInfo, ParameterExpression, ParameterExpression)"/> generates an equality test for each property
        /// The resulting compiled function evaluates whether all properties are equal between two instances.
        /// </remarks>
        private sealed class FastComparerBuilder : FunctionBuilder<T, T, T, bool>
        {
            protected override bool Seed => true;

            protected override Func<Expression, Expression, Expression> Conjunction => Expression.AndAlso;

            /// <summary>
            /// Produce an expression comparing the property on left and right instances using <see cref="Expression.Equal"/>.
            /// </summary>
            /// <param name="propertyInfo">Property being compared.</param>
            /// <param name="leftInstanceParameter">Left instance parameter expression.</param>
            /// <param name="rightInstanceParameter">Right instance parameter expression.</param>
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter) => Expression.Equal(Expression.Property(leftInstanceParameter, propertyInfo), Expression.Property(rightInstanceParameter, propertyInfo));
        }
    }
#pragma warning restore CA1000
}
