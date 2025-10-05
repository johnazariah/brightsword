using System.Linq.Expressions;
using System.Reflection;
using BrightSword.SwissKnife;

namespace BrightSword.Feber.Core
{
    /// <summary>
    /// Defines the public contract for operation builders that scan a prototype type and expose its filtered properties.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned for operations.</typeparam>
    /// <remarks>
    /// <para>
    /// Operation builders are the foundation of BrightSword.Feber's expression-based code generation system.
    /// They introspect a prototype type and build property-based expressions that can be composed into
    /// delegates like <see cref="Action{T}"/> or <see cref="Func{T, TResult}"/>.
    /// </para>
    /// <para>
    /// This interface allows consumers to work with any operation builder regardless of implementation
    /// when they only need to access the filtered properties.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Creating a custom builder that works with any IOperationBuilder
    /// public static class PropertyCounter
    /// {
    ///     public static int CountProperties<TProto>(IOperationBuilder<TProto> builder)
    ///     {
    ///         return builder.FilteredProperties.Count();
    ///     }
    /// }
    ///
    /// // Usage:
    /// var myBuilder = new SomeConcreteBuilder<MyClass>();
    /// int propertyCount = PropertyCounter.CountProperties(myBuilder);
    /// </code>
    /// </example>
    public interface IOperationBuilder<TProto>
    {
        /// <summary>
        /// Returns the public instance properties of <typeparamref name="TProto"/> filtered by the builder's filter.
        /// </summary>
        IEnumerable<PropertyInfo> FilteredProperties { get; }
    }

    /// <summary>
    /// Defines a unary operation builder that works with a single input instance.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TInstance">The runtime instance type that operations will act upon.</typeparam>
    /// <remarks>
    /// <para>
    /// Unary operation builders create expressions that operate on a single object instance.
    /// They're used for operations like pretty-printing, cloning, or serializing where
    /// you have a single object to process.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Method that works with any unary operation builder
    /// public static class BuilderProcessor
    /// {
    ///     public static void RegisterBuilder<TProto, TInstance>(
    ///         IUnaryOperationBuilder<TProto, TInstance> builder,
    ///         string builderName)
    ///     {
    ///         Console.WriteLine($"Registered {builderName} for {typeof(TProto).Name} → {typeof(TInstance).Name}");
    ///         // Further processing...
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IUnaryOperationBuilder<TProto, TInstance> : IOperationBuilder<TProto>
    {
    }

    /// <summary>
    /// Defines a binary operation builder that works with two input instances - left and right.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    /// <remarks>
    /// <para>
    /// Binary operation builders create expressions that compare or transform between two object instances.
    /// They're used for operations like comparing two objects for equality, copying properties from one
    /// object to another, or mapping between different object types.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: FastComparer implementation using binary operations
    /// public static class ObjectComparer
    /// {
    ///     private class ComparerBuilder<T> : BinaryOperationBuilderBase<T, T, T>
    ///     {
    ///         protected override Expression PropertyExpression(
    ///             PropertyInfo propertyInfo,
    ///             ParameterExpression leftParam,
    ///             ParameterExpression rightParam)
    ///         {
    ///             var leftProp = Expression.Property(leftParam, propertyInfo);
    ///             var rightProp = Expression.Property(rightParam, propertyInfo);
    ///             return Expression.Equal(leftProp, rightProp);
    ///         }
    ///     }
    ///
    ///     public static bool AreEqual<T>(T left, T right)
    ///     {
    ///         // Use a binary operation builder to compare objects
    ///         var builder = new ComparerBuilder<T>();
    ///         // Further implementation...
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IBinaryOperationBuilder<TProto, TLeftInstance, TRightInstance> : IOperationBuilder<TProto>
    {
    }

    /// <summary>
    /// Base class for operation builders that scan a prototype type (<typeparamref name="TProto"/>) and produce per-property <see cref="Expression"/> objects.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned to create operation expressions.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="OperationBuilderBase{TProto}"/> class provides the foundation for all expression-building operations in Feber.
    /// It handles property scanning and filtering, then converts those properties into expressions via <see cref="BuildPropertyExpression"/>.
    /// </para>
    /// <para>
    /// This abstract base is extended by specialized builders like <see cref="UnaryOperationBuilderBase{TProto,TInstance}"/> and
    /// <see cref="BinaryOperationBuilderBase{TProto,TLeftInstance,TRightInstance}"/> which provide parameter expressions
    /// and further refinements.
    /// </para>
    /// <para>
    /// Implementors typically override <see cref="BuildPropertyExpression(PropertyInfo)"/> or, in the unary/binary subclasses,
    /// the more specialized property expression methods.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Creating a custom operation builder that collects property names
    /// public class PropertyNameCollector<T> : OperationBuilderBase<T>
    /// {
    ///     protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
    ///     {
    ///         // Create a constant expression with the property's name
    ///         return Expression.Constant(propertyInfo.Name);
    ///     }
    ///
    ///     public IEnumerable<string> GetPropertyNames()
    ///     {
    ///         // Build and evaluate expressions to get property names
    ///         return OperationExpressions
    ///             .Cast<ConstantExpression>()
    ///             .Select(e => (string)e.Value);
    ///     }
    /// }
    ///
    /// // Usage:
    /// var collector = new PropertyNameCollector<Person>();
    /// foreach (string propName in collector.GetPropertyNames())
    ///     Console.WriteLine(propName);  // Outputs: FirstName, LastName, Age, etc.
    /// </code>
    /// </example>
    public abstract class OperationBuilderBase<TProto> : IOperationBuilder<TProto>
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
    /// Unary operation builder base which provides an instance parameter expression for operations that act on a single object.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TInstance">The runtime instance type that expressions will accept as a parameter.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="UnaryOperationBuilderBase{TProto,TInstance}"/> class simplifies creating operations that
    /// act on a single object instance by providing a parameter expression and dispatching to
    /// <see cref="PropertyExpression(PropertyInfo, ParameterExpression)"/>.
    /// </para>
    /// <para>
    /// It's commonly used as the base class for property printers, cloners, or serializers where you need
    /// to access properties on a single object and perform some operation with their values.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Creating a pretty-printer that outputs all properties
    /// public static class PrettyPrinter
    /// {
    ///     private class PrinterBuilder<T> : UnaryOperationBuilderBase<T, T>
    ///     {
    ///         protected override Expression PropertyExpression(
    ///             PropertyInfo propertyInfo,
    ///             ParameterExpression instanceParameter)
    ///         {
    ///             // Build property access expression
    ///             var propAccess = Expression.Property(instanceParameter, propertyInfo);
    ///
    ///             // Build expression to print: "PropertyName: PropertyValue"
    ///             var nameConst = Expression.Constant(propertyInfo.Name + ": ");
    ///             var toStringCall = Expression.Call(
    ///                 propAccess,
    ///                 typeof(object).GetMethod("ToString"));
    ///
    ///             return Expression.Call(
    ///                 typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
    ///                 Expression.Call(
    ///                     typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
    ///                     nameConst,
    ///                     toStringCall));
    ///         }
    ///     }
    ///
    ///     // Cache the action for each type
    ///     private static class PrintCache<T>
    ///     {
    ///         private static Action<T>? _printAction;
    ///
    ///         public static Action<T> PrintAction => _printAction ??= new ActionBuilder<T, T>().Action;
    ///     }
    ///
    ///     // Public API
    ///     public static void Print<T>(T obj) => PrintCache<T>.PrintAction(obj);
    /// }
    ///
    /// // Usage:
    /// var person = new Person { FirstName = "John", LastName = "Doe", Age = 30 };
    /// PrettyPrinter.Print(person);
    /// // Output:
    /// // FirstName: John
    /// // LastName: Doe
    /// // Age: 30
    /// </code>
    /// </example>
    public abstract class UnaryOperationBuilderBase<TProto, TInstance> : OperationBuilderBase<TProto>, IUnaryOperationBuilder<TProto, TInstance>
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
    /// Binary operation builder base which supplies left/right parameter expressions for operations that work with two objects.
    /// </summary>
    /// <typeparam name="TProto">Prototype type whose properties will be scanned.</typeparam>
    /// <typeparam name="TLeftInstance">Type of the left instance parameter.</typeparam>
    /// <typeparam name="TRightInstance">Type of the right instance parameter.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="BinaryOperationBuilderBase{TProto,TLeftInstance,TRightInstance}"/> class simplifies creating operations
    /// that work with two object instances (left and right) by providing parameter expressions for both and
    /// dispatching to <see cref="PropertyExpression(PropertyInfo, ParameterExpression, ParameterExpression)"/>.
    /// </para>
    /// <para>
    /// It's commonly used as the base class for property comparers, mappers, or copiers where you need to
    /// access properties on two objects and perform some operation comparing or transferring values.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example: Fast property mapper between different types
    /// public static class FastMapper
    /// {
    ///     private class MapperBuilder<TSource, TTarget> : BinaryOperationBuilderBase<TSource, TTarget, TSource>
    ///     {
    ///         protected override Expression PropertyExpression(
    ///             PropertyInfo propertyInfo,
    ///             ParameterExpression targetParameter,
    ///             ParameterExpression sourceParameter)
    ///         {
    ///             // Skip properties that can't be written
    ///             if (!propertyInfo.CanWrite)
    ///                 return Expression.Empty();
    ///
    ///             // Find matching property on target type
    ///             var targetProp = typeof(TTarget).GetProperty(propertyInfo.Name);
    ///             if (targetProp == null || !targetProp.CanWrite)
    ///                 return Expression.Empty();
    ///
    ///             // Build property access expressions
    ///             var sourcePropAccess = Expression.Property(sourceParameter, propertyInfo);
    ///
    ///             // Build assignment expression: target.Prop = source.Prop
    ///             return Expression.Assign(
    ///                 Expression.Property(targetParameter, targetProp),
    ///                 sourcePropAccess);
    ///         }
    ///     }
    ///
    ///     // Cache the action for each type pair
    ///     private static class MapperCache<TSource, TTarget>
    ///     {
    ///         private static Action<TTarget, TSource>? _mapAction;
    ///
    ///         public static Action<TTarget, TSource> MapAction =>
    ///             _mapAction ??= new MapperBuilder<TSource, TTarget>().CompileAction();
    ///     }
    ///
    ///     // Public API
    ///     public static void Map<TSource, TTarget>(TTarget target, TSource source) =>
    ///         MapperCache<TSource, TTarget>.MapAction(target, source);
    ///
    ///     public static TTarget Map<TSource, TTarget>(TSource source)
    ///         where TTarget : new()
    ///     {
    ///         var target = new TTarget();
    ///         Map(target, source);
    ///         return target;
    ///     }
    /// }
    ///
    /// // Usage:
    /// var dto = new PersonDto { FirstName = "John", LastName = "Doe", Age = 30 };
    /// var entity = FastMapper.Map<PersonDto, PersonEntity>(dto);
    /// </code>
    /// </example>
    public abstract class BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance> : OperationBuilderBase<TProto>, IBinaryOperationBuilder<TProto, TLeftInstance, TRightInstance>
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
