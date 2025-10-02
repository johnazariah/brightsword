using BrightSword.Feber.Core;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Samples;

public static class FastComparer<T>
{
  private static readonly FastComparer<T>.FastComparerBuilder _builder = new FastComparer<T>.FastComparerBuilder();

  public static bool AllPropertiesAreEqual(T left, T right)
  {
    return FastComparer<T>._builder.Function(left, right);
  }

  private class FastComparerBuilder : FunctionBuilder<T, T, T, bool>
  {
    protected override bool Seed => true;

    protected override Func<Expression, Expression, Expression> Conjunction
    {
      get => new Func<Expression, Expression, Expression>(Expression.AndAlso);
    }

    protected override Expression PropertyExpression(
      PropertyInfo property,
      ParameterExpression leftInstanceParameterExpression,
      ParameterExpression rightInstanceParameterExpression)
    {
      return (Expression) Expression.Equal((Expression) Expression.Property((Expression) leftInstanceParameterExpression, property), (Expression) Expression.Property((Expression) rightInstanceParameterExpression, property));
    }
  }
}
