using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class UnaryOperationBuilderBase<TProto, TInstance> : OperationBuilderBase<TProto>
{
  private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof (TInstance), "_instance");

  protected virtual ParameterExpression InstanceParameterExpression
  {
    get => UnaryOperationBuilderBase<TProto, TInstance>._parameterExpression;
  }

  protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
  {
    return this.PropertyExpression(propertyInfo, this.InstanceParameterExpression);
  }

  protected abstract Expression PropertyExpression(
    PropertyInfo property,
    ParameterExpression instanceParameterExpression);
}
