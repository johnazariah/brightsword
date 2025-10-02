using System.Collections.Generic;
using System.Linq;
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

    public static class CloneFactory<TProto, TSource, TDestination> where TDestination : new()
    {
        private static readonly CloneFactoryBuilder builder = new CloneFactoryBuilder();

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

            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression)
            {
                var property1 = MatchProperty(destinationProperties, property);
                var property2 = MatchProperty(sourceProperties, property);
                if (property1 == null || property1.GetSetMethod() == null)
                {
                    return Expression.Default(typeof(void));
                }
                else if (property2 == null || property2.GetGetMethod() == null)
                {
                    return Expression.Default(typeof(void));
                }
                else
                {
                    return Expression.Assign(Expression.Property(leftInstanceParameterExpression, property1), Expression.Property(rightInstanceParameterExpression, property2));
                }
            }

            private static PropertyInfo MatchProperty(
              IEnumerable<PropertyInfo> properties,
              PropertyInfo property) => properties.FirstOrDefault(_ => _.Name.Equals(property.Name, System.StringComparison.Ordinal) && _.PropertyType == property.PropertyType);
        }
    }
}
