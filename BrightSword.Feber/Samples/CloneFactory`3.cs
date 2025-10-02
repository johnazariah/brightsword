// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.CloneFactory`3
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using BrightSword.Feber.Core;
using BrightSword.SwissKnife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Samples;

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
    private static readonly IEnumerable<PropertyInfo> sourceProperties = TypeMemberDiscoverer.GetAllProperties(typeof (TSource), BindingFlags.Instance | BindingFlags.Public);
    private static readonly IEnumerable<PropertyInfo> destinationProperties = TypeMemberDiscoverer.GetAllProperties(typeof (TDestination), BindingFlags.Instance | BindingFlags.Public);

    protected override Expression PropertyExpression(
      PropertyInfo property,
      ParameterExpression leftInstanceParameterExpression,
      ParameterExpression rightInstanceParameterExpression)
    {
      PropertyInfo property1 = CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.MatchProperty(CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.destinationProperties, property);
      PropertyInfo property2 = CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.MatchProperty(CloneFactory<TProto, TSource, TDestination>.CloneFactoryBuilder.sourceProperties, property);
      if (property1 == (PropertyInfo) null || property1.GetSetMethod() == (MethodInfo) null)
        return (Expression) Expression.Default(typeof (void));
      return property2 == (PropertyInfo) null || property2.GetGetMethod() == (MethodInfo) null ? (Expression) Expression.Default(typeof (void)) : (Expression) Expression.Assign((Expression) Expression.Property((Expression) leftInstanceParameterExpression, property1), (Expression) Expression.Property((Expression) rightInstanceParameterExpression, property2));
    }

    private static PropertyInfo MatchProperty(
      IEnumerable<PropertyInfo> properties,
      PropertyInfo property)
    {
      return properties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (_ => _.Name.Equals(property.Name) && _.PropertyType == property.PropertyType));
    }
  }
}
