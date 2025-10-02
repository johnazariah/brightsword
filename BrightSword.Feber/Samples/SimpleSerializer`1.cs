// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.SimpleSerializer`1
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using BrightSword.Feber.Core;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Samples;

public static class SimpleSerializer<T>
{
  private static readonly SimpleSerializer<T>.SimpleSerializerBuilder _builder = new SimpleSerializer<T>.SimpleSerializerBuilder();

  public static string Serialize(T instance)
  {
    return $"{{{SimpleSerializer<T>._builder.Function(instance)}}}";
  }

  private class SimpleSerializerBuilder : FunctionBuilder<T, T, string>
  {
    private static readonly MethodInfo _concat = typeof (string).GetMethod("Concat", new Type[2]
    {
      typeof (string),
      typeof (string)
    });

    protected override string Seed => string.Empty;

    protected override Func<Expression, Expression, Expression> Conjunction
    {
      get
      {
        return (Func<Expression, Expression, Expression>) ((_l, _r) => (Expression) Expression.Call(SimpleSerializer<T>.SimpleSerializerBuilder._concat, _l, _r));
      }
    }

    private static MethodCallExpression GetToStringExpression(
      PropertyInfo property,
      Expression instanceParameterExpression)
    {
      if (!(property.PropertyType == typeof (DateTime)))
        return Expression.Call((Expression) Expression.Property(instanceParameterExpression, property), "ToString", (Type[]) null);
      return Expression.Call((Expression) Expression.Property(instanceParameterExpression, property), "ToString", (Type[]) null, (Expression) Expression.Constant((object) "O"));
    }

    protected override Expression PropertyExpression(
      PropertyInfo property,
      ParameterExpression instanceParameterExpression)
    {
      return (Expression) Expression.Call(typeof (string), "Format", (Type[]) null, (Expression) Expression.Constant((object) "{0}:{1},", typeof (string)), (Expression) Expression.Constant((object) property.Name, typeof (string)), (Expression) SimpleSerializer<T>.SimpleSerializerBuilder.GetToStringExpression(property, (Expression) instanceParameterExpression));
    }
  }
}
