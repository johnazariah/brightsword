using BrightSword.SwissKnife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BrightSword.Feber.Core;

public abstract class OperationBuilderBase<TProto>
{
  protected virtual Func<PropertyInfo, bool> PropertyFilter
  {
    get => (Func<PropertyInfo, bool>) (_ => true);
  }

  public virtual IEnumerable<PropertyInfo> FilteredProperties
  {
    get
    {
      return typeof (TProto).GetAllProperties(BindingFlags.Instance | BindingFlags.Public).Where<PropertyInfo>((Func<PropertyInfo, bool>) (_propertyInfo => this.PropertyFilter(_propertyInfo)));
    }
  }

  protected virtual IEnumerable<Expression> OperationExpressions
  {
    get
    {
      return this.FilteredProperties.Select<PropertyInfo, Expression>(new Func<PropertyInfo, Expression>(this.BuildPropertyExpression));
    }
  }

  protected abstract Expression BuildPropertyExpression(PropertyInfo propertyInfo);
}

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

public abstract class BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance> :
  OperationBuilderBase<TProto>
{
  private static readonly ParameterExpression _leftParameterExpression = Expression.Parameter(typeof(TLeftInstance), "_left");
  private static readonly ParameterExpression _rightParameterExpression = Expression.Parameter(typeof(TRightInstance), "_right");

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


