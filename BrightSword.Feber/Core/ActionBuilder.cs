using System;
using System.Linq.Expressions;

namespace BrightSword.Feber.Core
{
    /// <summary>
    /// Base class for building unary actions that perform operations on each property of an object.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TInstance">Instance type passed to the produced action.</typeparam>
    /// <remarks>
    /// <para>
    /// Use <see cref="ActionBuilder{TProto, TInstance}"/> to generate an <c>Action&lt;TInstance&gt;</c>
    /// that performs a side effect (such as printing, copying, or validating) for each property.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Pretty-print all properties of an object.
    /// public sealed class PrettyPrinterBuilder<TProto> : ActionBuilder<TProto, TProto>
    /// {
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
    ///     {
    ///         var member = Expression.Property(instanceParameter, propertyInfo);
    ///         var toString = Expression.Call(member, typeof(object).GetMethod("ToString"));
    ///         return Expression.Call(
    ///             typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
    ///             Expression.Call(typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
    ///                 Expression.Constant(propertyInfo.Name + ": "), toString));
    ///     }
    /// }
    /// var printer = new PrettyPrinterBuilder<MyProto>().Action;
    /// printer(myProtoInstance); // Prints all properties
    /// </code>
    /// </example>
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

    /// <summary>
    /// Base class for building binary actions that perform operations on each property of two objects.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    /// <remarks>
    /// <para>
    /// Use <see cref="ActionBuilder{TProto, TLeftInstance, TRightInstance}"/> to generate an <c>Action&lt;TLeftInstance, TRightInstance&gt;</c>
    /// that performs a side effect for each property pair (such as copying, merging, or comparing).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Copy matching properties from source to target.
    /// public sealed class CopierBuilder<TProto> : ActionBuilder<TProto, TProto, TProto>
    /// {
    ///     protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression target, ParameterExpression source)
    ///     {
    ///         var sourceProp = Expression.Property(source, propertyInfo);
    ///         var targetProp = Expression.Property(target, propertyInfo);
    ///         return Expression.Assign(targetProp, sourceProp);
    ///     }
    /// }
    /// var copier = new CopierBuilder<MyProto>().Action;
    /// copier(targetInstance, sourceInstance); // Copies properties
    /// </code>
    /// </example>
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
