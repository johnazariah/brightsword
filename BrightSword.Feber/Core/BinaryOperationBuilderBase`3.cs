// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.BinaryOperationBuilderBase`3
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

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
