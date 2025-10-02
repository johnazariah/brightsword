
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
    public static class FastComparer<T>
    {
        private static readonly FastComparerBuilder _builder = new FastComparerBuilder();

        public static bool AllPropertiesAreEqual(T left, T right) => _builder.Function(left, right);

        private sealed class FastComparerBuilder : FunctionBuilder<T, T, T, bool>
        {
            protected override bool Seed => true;

            protected override Func<Expression, Expression, Expression> Conjunction => new Func<Expression, Expression, Expression>(Expression.AndAlso);

            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression) => Expression.Equal(Expression.Property(leftInstanceParameterExpression, property), Expression.Property(rightInstanceParameterExpression, property));
        }
    }
}
