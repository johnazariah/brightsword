
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    public static class SimpleSerializer
    {
        public static string Serialize<T>(this T This) => SimpleSerializer<T>.Serialize(This);
    }

#pragma warning disable CA1000 // Allow static members on generic sample types
    public static class SimpleSerializer<T>
    {
#pragma warning disable RCS1250 // allow target-typed new() in samples
        private static readonly SimpleSerializerBuilder _builder = new();
#pragma warning restore RCS1250

        public static string Serialize(T instance) => $"{{{_builder.Function(instance)}}}";

        private sealed class SimpleSerializerBuilder : FunctionBuilder<T, T, string>
        {
            private static readonly MethodInfo _concat = typeof(string).GetMethod("Concat", [typeof(string), typeof(string)]);

            protected override string Seed => string.Empty;

            protected override Func<Expression, Expression, Expression> Conjunction => (_l, _r) => Expression.Call(_concat, _l, _r);

            private static MethodCallExpression GetToStringExpression(
              PropertyInfo property,
              Expression instanceParameterExpression)
            {
                return property.PropertyType != typeof(DateTime)
                    ? Expression.Call(Expression.Property(instanceParameterExpression, property), "ToString", (Type[])null)
                    : Expression.Call(Expression.Property(instanceParameterExpression, property), "ToString", (Type[])null, Expression.Constant("O"));
            }

            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression instanceParameterExpression) => Expression.Call(typeof(string), "Format", (Type[])null, Expression.Constant("{0}:{1},", typeof(string)), Expression.Constant(property.Name, typeof(string)), GetToStringExpression(property, instanceParameterExpression));
        }
    }
#pragma warning restore CA1000
}
