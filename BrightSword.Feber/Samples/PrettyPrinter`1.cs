using BrightSword.Feber.Core;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Samples;

public static class PrettyPrinter<TProto>
{
  private static readonly PrettyPrinter<TProto>.PrettyPrinterBuilder _builder = new PrettyPrinter<TProto>.PrettyPrinterBuilder();

  public static void Print(TProto instance) => PrettyPrinter<TProto>._builder.Action(instance);

  private class PrettyPrinterBuilder : ActionBuilder<TProto, TProto>
  {
    protected override Expression PropertyExpression(
      PropertyInfo property,
      ParameterExpression instanceParameterExpression)
    {
      MemberExpression memberExpression = Expression.Property((Expression) instanceParameterExpression, property);
      return property.PropertyType == typeof (string) ? (Expression) Expression.Call(typeof (Console), "WriteLine", (Type[]) null, (Expression) Expression.Constant((object) "\t{0} : {1}", typeof (string)), (Expression) Expression.Constant((object) property.Name, typeof (string)), (Expression) memberExpression) : (Expression) Expression.Call(typeof (Console), "WriteLine", (Type[]) null, (Expression) Expression.Constant((object) "\t{0} : {1}", typeof (string)), (Expression) Expression.Constant((object) property.Name, typeof (string)), (Expression) Expression.Call(typeof (Convert), "ToString", (Type[]) null, (Expression) memberExpression));
    }
  }
}
