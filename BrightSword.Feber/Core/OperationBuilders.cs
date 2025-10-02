using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.SwissKnife;

namespace BrightSword.Feber.Core;

public abstract class OperationBuilderBase<TProto>
{
    protected virtual Func<PropertyInfo, bool> PropertyFilter => _ => true;

    public virtual IEnumerable<PropertyInfo> FilteredProperties => typeof(TProto)
        .GetAllProperties(BindingFlags.Instance | BindingFlags.Public)
        .Where(PropertyFilter);

    protected virtual IEnumerable<Expression> OperationExpressions => FilteredProperties.Select(BuildPropertyExpression);

    protected abstract Expression BuildPropertyExpression(PropertyInfo propertyInfo);
}

public abstract class UnaryOperationBuilderBase<TProto, TInstance> : OperationBuilderBase<TProto>
{
    private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof(TInstance), "_instance");

    protected virtual ParameterExpression InstanceParameterExpression => _parameterExpression;

    protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo) => PropertyExpression(propertyInfo, InstanceParameterExpression);

    protected abstract Expression PropertyExpression(PropertyInfo property, ParameterExpression instanceParameterExpression);
}

public abstract class BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance> : OperationBuilderBase<TProto>
{
    private static readonly ParameterExpression _leftParameterExpression = Expression.Parameter(typeof(TLeftInstance), "_left");
    private static readonly ParameterExpression _rightParameterExpression = Expression.Parameter(typeof(TRightInstance), "_right");

    protected virtual ParameterExpression LeftInstanceParameterExpression => _leftParameterExpression;

    protected virtual ParameterExpression RightInstanceParameterExpression => _rightParameterExpression;

    protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo) => PropertyExpression(propertyInfo, LeftInstanceParameterExpression, RightInstanceParameterExpression);

    protected abstract Expression PropertyExpression(PropertyInfo property, ParameterExpression leftInstanceParameterExpression, ParameterExpression rightInstanceParameterExpression);
}


