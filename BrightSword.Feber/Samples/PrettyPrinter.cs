
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    public static class PrettyPrinter
    {
        public static void Print<T>(this T This) => PrettyPrinter<T>.Print(This);
    }

    public static class PrettyPrinter<TProto>
    {
        private static readonly PrettyPrinterBuilder _builder = new PrettyPrinterBuilder();

        public static void Print(TProto instance) => _builder.Action(instance);

        private sealed class PrettyPrinterBuilder : ActionBuilder<TProto, TProto>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression instanceParameterExpression)
            {
                var memberExpression = Expression.Property(instanceParameterExpression, property);
                return property.PropertyType == typeof(string) ? Expression.Call(typeof(Console), "WriteLine", (Type[])null, Expression.Constant("\t{0} : {1}", typeof(string)), Expression.Constant(property.Name, typeof(string)), memberExpression) : Expression.Call(typeof(Console), "WriteLine", (Type[])null, Expression.Constant("\t{0} : {1}", typeof(string)), Expression.Constant(property.Name, typeof(string)), Expression.Call(typeof(Convert), "ToString", (Type[])null, memberExpression));
            }
        }
    }
}
