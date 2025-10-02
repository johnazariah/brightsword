using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace BrightSword.Feber.Core;

public static class DynamicExpressionUtilities
{
    private const CSharpArgumentInfoFlags C_VALUE_FLAGS = CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant;
    private const CSharpArgumentInfoFlags C_THIS_FLAGS = CSharpArgumentInfoFlags.None;
    private const CSharpBinderFlags C_BINDER_SET_FLAGS = CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded;
    private const CSharpBinderFlags C_BINDER_GET_FLAGS = CSharpBinderFlags.InvokeSpecialName;
    private static readonly CSharpArgumentInfo _thisArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string)null);
    private static readonly CSharpArgumentInfo _valueArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string)null);

    public static Expression GetDynamicPropertyAccessorExpression<T>(
      this ParameterExpression parameterExpression,
      PropertyInfo propertyInfo)
    {
        return parameterExpression.GetDynamicPropertyAccessorExpression<T>(propertyInfo.Name, propertyInfo.PropertyType);
    }

    public static Expression GetDynamicPropertyAccessorExpression<T>(
      this ParameterExpression parameterExpression,
      string propertyName,
      Type propertyType)
    {
        return (Expression)Expression.Convert((Expression)Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.InvokeSpecialName, propertyName, typeof(T), (IEnumerable<CSharpArgumentInfo>)new CSharpArgumentInfo[1]
        {
      DynamicExpressionUtilities._thisArgument
        }), typeof(object), (Expression)parameterExpression), propertyType);
    }

    public static Expression GetDynamicPropertyMutatorExpression<T>(
      this ParameterExpression parameterExpression,
      PropertyInfo propertyInfo,
      Expression valueExpression)
    {
        return parameterExpression.GetDynamicPropertyMutatorExpression<T>(propertyInfo.Name, valueExpression);
    }

    public static Expression GetDynamicPropertyMutatorExpression<T>(
      this ParameterExpression parameterExpression,
      string propertyName,
      Expression valueExpression)
    {
        return (Expression)Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded, propertyName, typeof(T), (IEnumerable<CSharpArgumentInfo>)new CSharpArgumentInfo[2]
        {
      DynamicExpressionUtilities._thisArgument,
      DynamicExpressionUtilities._valueArgument
        }), typeof(object), (Expression)parameterExpression, valueExpression);
    }
}
