using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace BrightSword.Feber.Core;

public static class DynamicExpressionUtilities
{
    private const CSharpArgumentInfoFlags C_VALUE_FLAGS = CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant;
    private const CSharpArgumentInfoFlags C_THIS_FLAGS = CSharpArgumentInfoFlags.None;
    private const CSharpBinderFlags C_BINDER_SET_FLAGS = CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded;
    private const CSharpBinderFlags C_BINDER_GET_FLAGS = CSharpBinderFlags.InvokeSpecialName;
    private static readonly CSharpArgumentInfo _thisArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
    private static readonly CSharpArgumentInfo _valueArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null);

    public static Expression GetDynamicPropertyAccessorExpression<T>(this ParameterExpression parameterExpression, PropertyInfo propertyInfo)
        => parameterExpression.GetDynamicPropertyAccessorExpression<T>(propertyInfo.Name, propertyInfo.PropertyType);

    public static Expression GetDynamicPropertyAccessorExpression<T>(this ParameterExpression parameterExpression, string propertyName, Type propertyType)
    {
        var callSiteBinder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.InvokeSpecialName, propertyName, typeof(T), new[] { _thisArgument });
        var dyn = Expression.Dynamic(callSiteBinder, typeof(object), parameterExpression);
        return Expression.Convert(dyn, propertyType);
    }

    public static Expression GetDynamicPropertyMutatorExpression<T>(this ParameterExpression parameterExpression, PropertyInfo propertyInfo, Expression valueExpression)
        => parameterExpression.GetDynamicPropertyMutatorExpression<T>(propertyInfo.Name, valueExpression);

    public static Expression GetDynamicPropertyMutatorExpression<T>(this ParameterExpression parameterExpression, string propertyName, Expression valueExpression)
    {
        var callSiteBinder = Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded, propertyName, typeof(T), new[] { _thisArgument, _valueArgument });
        return Expression.Dynamic(callSiteBinder, typeof(object), parameterExpression, valueExpression);
    }
}
