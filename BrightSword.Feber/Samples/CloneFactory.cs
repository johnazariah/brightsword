namespace BrightSword.Feber.Samples;

using BrightSword.Feber.Core;
using BrightSword.SwissKnife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public static class CloneFactory
{
    public static TSource Clone<TSource>(this TSource source) where TSource : new()
    {
        return CloneFactory<TSource, TSource, TSource>.Clone(source);
    }

    public static TDestination Clone<TSource, TDestination>(this TSource source) where TDestination : new()
    {
        return CloneFactory<TSource, TSource, TDestination>.Clone(source);
    }

    public static TDestination Clone<TSource, TDestination, TBase>(this TSource source) where TDestination : new()
    {
        return CloneFactory<TBase, TSource, TDestination>.Clone(source);
    }
}

public static class CloneFactory<TProto, TSource, TDestination> where TDestination : new()
{
    private static readonly CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder builder = new CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder();

    public static TDestination Clone(TSource source)
    {
        TDestination destination = new TDestination();
        CloneFactory<TProto, TSource, TDestination>.builder.Action(destination, source);
        return destination;
    }

    private class CloneFactoryBuilder : ActionBuilder<TProto, TDestination, TSource>
    {
        private static readonly IEnumerable<PropertyInfo> sourceProperties = typeof(TSource).GetAllProperties(BindingFlags.Instance | BindingFlags.Public);
        private static readonly IEnumerable<PropertyInfo> destinationProperties = typeof(TDestination).GetAllProperties(BindingFlags.Instance | BindingFlags.Public);

        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            PropertyInfo property1 = CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.MatchProperty(CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.destinationProperties, property);
            PropertyInfo property2 = CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.MatchProperty(CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.sourceProperties, property);
            if (property1 == (PropertyInfo)null || property1.GetSetMethod() == (MethodInfo)null)
                return (Expression)Expression.Default(typeof(void));
            return property2 == (PropertyInfo)null || property2.GetGetMethod() == (MethodInfo)null ? (Expression)Expression.Default(typeof(void)) : (Expression)Expression.Assign((Expression)Expression.Property((Expression)leftInstanceParameterExpression, property1), (Expression)Expression.Property((Expression)rightInstanceParameterExpression, property2));
        }

        private static PropertyInfo MatchProperty(
          IEnumerable<PropertyInfo> properties,
          PropertyInfo property)
        {
            return properties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>)(_ => _.Name.Equals(property.Name) && _.PropertyType == property.PropertyType));
        }
    }
}
