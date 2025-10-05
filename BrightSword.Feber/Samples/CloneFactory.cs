using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;
using BrightSword.SwissKnife;

namespace BrightSword.Feber.Samples
{
    public static class CloneFactory
    {
        public static TSource Clone<TSource>(this TSource source) where TSource : new() => CloneFactory<TSource, TSource, TSource>.Clone(source);

        public static TDestination Clone<TSource, TDestination>(this TSource source) where TDestination : new() => CloneFactory<TSource, TSource, TDestination>.Clone(source);

        public static TDestination Clone<TSource, TDestination, TBase>(this TSource source) where TDestination : new() => CloneFactory<TBase, TSource, TDestination>.Clone(source);
    }

#pragma warning disable CA1000 // Allow static members on generic sample types
    /// <summary>
    /// Sample clone factory that demonstrates using an <see cref="ActionBuilder{TProto, TLeftInstance, TRightInstance}"/>
    /// to copy matching public properties from a source object to a destination object.
    /// </summary>
    /// <remarks>
    /// The inner <see cref="CloneFactoryBuilder"/> overrides <see cref="ActionBuilder{TProto, TLeftInstance,TRightInstance}.PropertyExpression"/>
    /// (via the binary subclass) to produce per-property <see cref="Expression"/> values which either assign matching properties or emit a no-op.
    /// This demonstrates matching properties by name and type and shows how builders can compose assignment expressions into a compiled delegate.
    /// </remarks>
    /// <typeparam name="TProto">Prototype type used to determine which properties to process (often the same as the source/destination).</typeparam>
    /// <typeparam name="TSource">Source object type whose properties are read.</typeparam>
    /// <typeparam name="TDestination">Destination object type whose properties are written.</typeparam>
    public static class CloneFactory<TProto, TSource, TDestination> where TDestination : new()
    {
#pragma warning disable RCS1250 // allow target-typed new() in samples
        private static readonly CloneFactoryBuilder builder = new();
#pragma warning restore RCS1250

        public static TDestination Clone(TSource source)
        {
            var destination = new TDestination();
            builder.Action(destination, source);
            return destination;
        }

        private sealed class CloneFactoryBuilder : ActionBuilder<TProto, TDestination, TSource>
        {
            private static readonly IEnumerable<PropertyInfo> sourceProperties = typeof(TSource).GetAllProperties(BindingFlags.Instance | BindingFlags.Public);
            private static readonly IEnumerable<PropertyInfo> destinationProperties = typeof(TDestination).GetAllProperties(BindingFlags.Instance | BindingFlags.Public);
            /// <summary>
            /// Build per-property expression used by the clone operation.
            /// If both source and destination declare a matching public property with a getter/setter, this method returns an assignment expression.
            /// Otherwise it returns a default(void) expression (no-op) so the composed block skips the property.
            /// </summary>
            /// <param name="propertyInfo">The prototype property currently being processed.</param>
            /// <param name="leftInstanceParameter">ParameterExpression for the destination instance.</param>
            /// <param name="rightInstanceParameter">ParameterExpression for the source instance.</param>
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter)
            {
                var property1 = MatchProperty(destinationProperties, propertyInfo);
                var property2 = MatchProperty(sourceProperties, propertyInfo);
                return property1 is null || property1.GetSetMethod() is null || property2 is null || property2.GetGetMethod() is null
                        ? Expression.Default(typeof(void))
                        : Expression.Assign(Expression.Property(leftInstanceParameter, property1), Expression.Property(rightInstanceParameter, property2));
            }

            /// <summary>
            /// Helper used to find a property in a collection that matches name and type of the prototype property.
            /// </summary>
            /// <param name="properties">Collection of properties to search.</param>
            /// <param name="propertyInfo">Prototype property to match against.</param>
            private static PropertyInfo MatchProperty(
                IEnumerable<PropertyInfo> properties,
                PropertyInfo propertyInfo) => properties.FirstOrDefault(_ => _.Name.Equals(propertyInfo.Name, System.StringComparison.Ordinal) && _.PropertyType == propertyInfo.PropertyType);
        }
    }
#pragma warning restore CA1000
}
