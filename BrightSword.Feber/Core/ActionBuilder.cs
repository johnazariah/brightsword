using System;
using System.Linq.Expressions;

namespace BrightSword.Feber.Core
{
    /// <summary>
    /// Represents an abstract builder for creating unary operations that act on instances of <typeparamref name="TInstance"/>.
    /// Provides functionality to build and cache an <see cref="Action{TInstance}"/> delegate based on operation expressions.
    /// </summary>
    /// <typeparam name="TProto">The prototype type used in the operation builder.</typeparam>
    /// <typeparam name="TInstance">The instance type on which the action operates.</typeparam>
    /// <usage>
    /// 1. Derive from this class and override <see cref="PropertyExpression"/> to return an expression that acts on each of the properties in <typeparamref name="TProto"/>.
    /// 2. Access the <see cref="Action"/> property to get the compiled action.
    /// 3. Call this action with an instance of <typeparamref name="TInstance"/>.
    ///
    /// You can wrap all of these steps nicely so that you can then surface an extension method that performs the action as an extension method on <typeparamref name="TInstance"/> as in the example.
    /// </usage>
    /// <example>
    /// This example shows how to use an <see cref="ActionBuilder{TProto, TInstance}"/> to build an expression that pretty-prints all the properties of type <typeparamref name="TProto"/> in an object of (possibly derived) type <typeparamref name="TInstance"/>
    ///
    /// <code>
    ///    public static class PrettyPrinter
    ///    {
    ///        private static class PrettyPrinterImpl<TProto>
    ///        {
    ///            private static readonly PrettyPrinterBuilder _builder = new();
    ///
    ///            private sealed class PrettyPrinterBuilder : ActionBuilder<TProto, TProto>
    ///            {
    ///                protected override Expression PropertyExpression(
    ///                    PropertyInfo propertyInfo,
    ///                    ParameterExpression instanceParameter)
    ///                {
    ///                    var memberExpression = Expression.Property(instanceParameter, propertyInfo);
    ///                    Expression stringExpression = propertyInfo.PropertyType == typeof(string)
    ///                        ? memberExpression
    ///                        : Expression.Call(typeof(Convert), "ToString", Type.EmptyTypes, memberExpression);
    ///                    return Expression.Call(typeof(Console), "WriteLine", Type.EmptyTypes, Expression.Constant("\t{0} : {1}", typeof(string)), Expression.Constant(propertyInfo.Name, typeof(string)), stringExpression);
    ///                }
    ///            }
    ///
    ///            public static void Print(TProto instance) => _builder.Action(instance);
    ///        }
    ///
    ///        public static void Print<T>(this T This) => PrettyPrinterImpl<T>.Print(This);
    ///    }
    /// </code>
    ///
    /// </example>
    ///
    public abstract class ActionBuilder<TProto, TInstance> : UnaryOperationBuilderBase<TProto, TInstance>
    {
        private Action<TInstance> _action;

        /// <summary>
        /// Gets a compiled <see cref="Action{TInstance}"/> that executes the composed operation expressions for the provided instance.
        /// </summary>
        /// <remarks>
        /// The action is compiled lazily and cached in the private field <c>_action</c>. On first access the implementation calls
        /// <see cref="BuildAction"/> which composes <see cref="OperationExpressions"/> into a single block and compiles it into an <see cref="Action{TInstance}"/> delegate.
        /// Derived types may override <see cref="BuildAction"/> to customize how the delegate is constructed or cached.
        /// </remarks>
        /// <returns>A compiled delegate of type <see cref="Action{TInstance}"/>.</returns>
        public virtual Action<TInstance> Action => _action ??= BuildAction();

        /// <summary>
        /// Creates and compiles an <see cref="Action{TInstance}"/> from the sequence of <see cref="OperationExpressions"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation wraps <see cref="OperationExpressions"/> into an expression block and compiles that block into
        /// an <see cref="Action{TInstance}"/> using <see cref="UnaryOperationBuilderBase{TProto, TInstance}.InstanceParameterExpression"/> as the parameter.
        /// Override this method to change compilation strategy (for example to emit different expression shapes or to change caching behavior).
        /// </remarks>
        /// <returns>A compiled <see cref="Action{TInstance}"/> delegate that executes the built operations for a given instance.</returns>
        protected virtual Action<TInstance> BuildAction() => Expression.Lambda<Action<TInstance>>(Expression.Block(OperationExpressions), InstanceParameterExpression).Compile();
    }

    public abstract class ActionBuilder<TProto, TLeftInstance, TRightInstance> :
        BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
    {
        private Action<TLeftInstance, TRightInstance> _action;

        /// <summary>
        /// Gets a compiled <see cref="Action{TLeftInstance,TRightInstance}"/> which executes the composed operation expressions using the left/right instance parameters.
        /// </summary>
        /// <remarks>
        /// The compiled action is cached in <c>_action</c>. By default this property invokes <see cref="BuildAction"/> on first access to produce the delegate.
        /// Override <see cref="BuildAction"/> to change how the delegate is constructed or cached.
        /// </remarks>
        /// <returns>A compiled delegate of type <see cref="Action{TLeftInstance,TRightInstance}"/>.</returns>
        public virtual Action<TLeftInstance, TRightInstance> Action => _action ??= BuildAction();

        /// <summary>
        /// Creates and compiles an <see cref="Action{TLeftInstance,TRightInstance}"/> from the sequence of <see cref="OperationExpressions"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation composes the <see cref="OperationExpressions"/> into an expression block and compiles it using
        /// <see cref="BinaryOperationBuilderBase{TProto,TLeftInstance,TRightInstance}.LeftInstanceParameterExpression"/> and
        /// <see cref="BinaryOperationBuilderBase{TProto,TLeftInstance,TRightInstance}.RightInstanceParameterExpression"/> as parameters.
        /// </remarks>
        /// <returns>A compiled <see cref="Action{TLeftInstance,TRightInstance}"/> delegate that executes the built operations for given left and right instances.</returns>
        protected virtual Action<TLeftInstance, TRightInstance> BuildAction() => Expression.Lambda<Action<TLeftInstance, TRightInstance>>(Expression.Block(OperationExpressions), LeftInstanceParameterExpression, RightInstanceParameterExpression).Compile();
    }
}
