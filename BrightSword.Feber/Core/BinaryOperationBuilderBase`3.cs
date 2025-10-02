using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance> : 
  OperationBuilderBase<TProto>
{
  private static readonly ParameterExpression _leftParameterExpression = Expression.Parameter(typeof (TLeftInstance), "_left");
  private static readonly ParameterExpression _rightParameterExpression = Expression.Parameter(typeof (TRightInstance), "_right");

  protected virtual ParameterExpression LeftInstanceParameterExpression
  {
    get
    {
      return BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>._leftParameterExpression;
    }
  }

  protected virtual ParameterExpression RightInstanceParameterExpression
  {
    get
    {
      return BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>._rightParameterExpression;
    }
  }

  protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
  {
    return this.PropertyExpression(propertyInfo, this.LeftInstanceParameterExpression, this.RightInstanceParameterExpression);
  }

  protected abstract Expression PropertyExpression(
    PropertyInfo property,
    ParameterExpression leftInstanceParameterExpression,
    ParameterExpression rightInstanceParameterExpression);
}
