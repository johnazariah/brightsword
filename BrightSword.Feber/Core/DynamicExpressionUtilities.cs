using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace BrightSword.Feber.Core
{
    public static class DynamicExpressionUtilities
    {
        private static readonly CSharpArgumentInfo _thisArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
        private static readonly CSharpArgumentInfo _valueArgument = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null);
        // Reuse argument info arrays to avoid repeated allocations (CA1861)
        private static readonly CSharpArgumentInfo[] _getMemberArgs = [_thisArgument];
        private static readonly CSharpArgumentInfo[] _setMemberArgs = [_thisArgument, _valueArgument];

        /// <summary>
        /// Builds an expression that reads a dynamic property (by <see cref="PropertyInfo"/>) from the provided <see cref="ParameterExpression"/>.
        /// </summary>
        /// <typeparam name="T">The compile-time type used as the dynamic binder context.</typeparam>
        /// <param name="parameterExpression">Parameter expression representing the instance to read from.</param>
        /// <param name="propertyInfo">Property information describing the dynamic property to read.</param>
        /// <returns>An <see cref="Expression"/> that reads the property and converts it to the property's type.</returns>
        /// <example>
        /// <code>
        /// // Example: create an expression that reads a dynamic property's value and converts it to the declared property type.
        /// var param = Expression.Parameter(typeof(object), "dyn");
        /// var propInfo = typeof(MyProto).GetProperty("Name");
        /// var accessor = param.GetDynamicPropertyAccessorExpression<MyProto>(propInfo);
        /// // accessor can then be used inside a larger expression tree and compiled.
        /// </code>
        /// </example>
        public static Expression GetDynamicPropertyAccessorExpression<T>(this ParameterExpression parameterExpression, PropertyInfo propertyInfo)
            => parameterExpression.GetDynamicPropertyAccessorExpression<T>(propertyInfo.Name, propertyInfo.PropertyType);

        /// <summary>
        /// Builds an expression that reads a dynamic property by name from the provided <see cref="ParameterExpression"/> and casts it to <paramref name="propertyType"/>.
        /// </summary>
        /// <typeparam name="T">The compile-time type used as the dynamic binder context.</typeparam>
        /// <param name="parameterExpression">Parameter expression representing the instance to read from.</param>
        /// <param name="propertyName">Name of the dynamic property to read.</param>
        /// <param name="propertyType">Target CLR type to convert the dynamic result to.</param>
        /// <returns>An <see cref="Expression"/> that reads the named dynamic property and converts it to the specified type.</returns>
        public static Expression GetDynamicPropertyAccessorExpression<T>(this ParameterExpression parameterExpression, string propertyName, Type propertyType)
        {
            var callSiteBinder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.InvokeSpecialName, propertyName, typeof(T), _getMemberArgs);
            var dyn = Expression.Dynamic(callSiteBinder, typeof(object), parameterExpression);
            return Expression.Convert(dyn, propertyType);
        }

        /// <summary>
        /// Builds an expression that assigns a value to a dynamic property described by <paramref name="propertyInfo"/> on the provided <paramref name="parameterExpression"/>.
        /// </summary>
        /// <typeparam name="T">The compile-time type used as the dynamic binder context.</typeparam>
        /// <param name="parameterExpression">Parameter expression representing the instance to write to.</param>
        /// <param name="propertyInfo">Property information describing the dynamic property to write.</param>
        /// <param name="valueExpression">Expression that produces the value to assign to the property.</param>
        /// <returns>An <see cref="Expression"/> that assigns <paramref name="valueExpression"/> to the named dynamic property.</returns>
        public static Expression GetDynamicPropertyMutatorExpression<T>(this ParameterExpression parameterExpression, PropertyInfo propertyInfo, Expression valueExpression)
            => parameterExpression.GetDynamicPropertyMutatorExpression<T>(propertyInfo.Name, valueExpression);

        /// <summary>
        /// Builds an expression that assigns a value to a dynamic property by name on the provided <paramref name="parameterExpression"/>.
        /// </summary>
        /// <typeparam name="T">The compile-time type used as the dynamic binder context.</typeparam>
        /// <param name="parameterExpression">Parameter expression representing the instance to write to.</param>
        /// <param name="propertyName">Name of the dynamic property to write.</param>
        /// <param name="valueExpression">Expression that produces the value to assign to the property.</param>
        /// <returns>An <see cref="Expression"/> representing the dynamic set-member call.</returns>
        public static Expression GetDynamicPropertyMutatorExpression<T>(this ParameterExpression parameterExpression, string propertyName, Expression valueExpression)
        {
            var callSiteBinder = Microsoft.CSharp.RuntimeBinder.Binder.SetMember(CSharpBinderFlags.InvokeSpecialName | CSharpBinderFlags.ResultDiscarded, propertyName, typeof(T), _setMemberArgs);
            return Expression.Dynamic(callSiteBinder, typeof(object), parameterExpression, valueExpression);
        }
    }
}
