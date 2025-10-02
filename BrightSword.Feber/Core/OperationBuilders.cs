using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.SwissKnife;

namespace BrightSword.Feber.Core
{
    /// <summary>
    /// Base class for operation builders that scan a prototype type (<typeparamref name="TProto"/>) and produce per-property <see cref="Expression"/> objects.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned to create operation expressions.</typeparam>
    /// <remarks>
    /// Implementors typically override <see cref="BuildPropertyExpression(PropertyInfo)"/> or, in the unary/binary subclasses, <see cref="PropertyExpression(PropertyInfo, ParameterExpression)"/> to produce per-property expressions.
    /// The default behavior is to expose <see cref="FilteredProperties"/> and to turn those into <see cref="OperationExpressions"/> which can then be composed by higher-level builders (e.g., <see cref="ActionBuilder{TProto,TInstance}"/> or <see cref="FunctionBuilder{TProto,TInstance,TResult}"/>).
    /// </remarks>
    public abstract class OperationBuilderBase<TProto>
    {
        /// <summary>
        /// Predicate used to filter properties on <typeparamref name="TProto"/>. Override to change which properties are included.
        /// </summary>
        protected virtual Func<PropertyInfo, bool> PropertyFilter => _ => true;

        /// <summary>
        /// Returns the public instance properties of <typeparamref name="TProto"/> filtered by <see cref="PropertyFilter"/>.
        /// </summary>
        public virtual IEnumerable<PropertyInfo> FilteredProperties => typeof(TProto)
            .GetAllProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(PropertyFilter);

        /// <summary>
        /// Returns the sequence of <see cref="Expression"/> instances produced for each filtered property.
        /// </summary>
        protected virtual IEnumerable<Expression> OperationExpressions => FilteredProperties.Select(BuildPropertyExpression);

        /// <summary>
        /// Build the expression for a single property. Override in subclasses to implement per-property behavior.
        /// </summary>
        /// <param name="propertyInfo">Property being processed.</param>
        /// <returns>An <see cref="Expression"/> that represents the operation for the property.</returns>
        protected abstract Expression BuildPropertyExpression(PropertyInfo propertyInfo);
    }

    /// <summary>
    /// Unary operation builder base which provides an <see cref="InstanceParameterExpression"/> and dispatches to <see cref="PropertyExpression(PropertyInfo, ParameterExpression)"/>.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TInstance">The runtime instance type that expressions will accept as a parameter.</typeparam>
    public abstract class UnaryOperationBuilderBase<TProto, TInstance> : OperationBuilderBase<TProto>
    {
        private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof(TInstance), "_instance");

        /// <summary>
        /// ParameterExpression representing the instance passed to compiled delegates. Tests or custom builders may override to provide a different parameter expression.
        /// </summary>
        protected virtual ParameterExpression InstanceParameterExpression => _parameterExpression;

        protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
            => PropertyExpression(propertyInfo, InstanceParameterExpression);

        /// <summary>
        /// Implement this method to produce an <see cref="Expression"/> for a single property given the instance parameter expression.
        /// </summary>
        /// <param name="propertyInfo">Property being processed.</param>
        /// <param name="instanceParameter">ParameterExpression representing the instance passed to compiled delegates.</param>
        protected abstract Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter);
    }

    /// <summary>
    /// Binary operation builder base which supplies left/right parameter expressions and dispatches to <see cref="PropertyExpression(PropertyInfo, ParameterExpression, ParameterExpression)"/>.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    public abstract class BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance> : OperationBuilderBase<TProto>
    {
        private static readonly ParameterExpression _leftParameterExpression = Expression.Parameter(typeof(TLeftInstance), "_left");
        private static readonly ParameterExpression _rightParameterExpression = Expression.Parameter(typeof(TRightInstance), "_right");

        /// <summary>
        /// ParameterExpression representing the left instance passed to compiled delegates.
        /// </summary>
        protected virtual ParameterExpression LeftInstanceParameterExpression => _leftParameterExpression;

        /// <summary>
        /// ParameterExpression representing the right instance passed to compiled delegates.
        /// </summary>
        protected virtual ParameterExpression RightInstanceParameterExpression => _rightParameterExpression;

        protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
            => PropertyExpression(propertyInfo, LeftInstanceParameterExpression, RightInstanceParameterExpression);

        /// <summary>
        /// Implement this method to produce an <see cref="Expression"/> for a single property given left/right instance parameter expressions.
        /// </summary>
        /// <param name="propertyInfo">Property being processed.</param>
        /// <param name="leftInstanceParameter">ParameterExpression representing the left instance.</param>
        /// <param name="rightInstanceParameter">ParameterExpression representing the right instance.</param>
        protected abstract Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression leftInstanceParameter, ParameterExpression rightInstanceParameter);
    }
}
