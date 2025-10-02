
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    public static class SimpleSerializer
    {
        /// <summary>
        /// Serialize an object's public properties into a simple comma-delimited key:value string using the compiled sample builder.
        /// </summary>
        /// <example>
        /// <code>
        /// public class Person { public string Name { get; set; } public int Age { get; set; } }
        /// var p = new Person { Name = "Ada", Age = 30 };
        /// var s = p.Serialize(); // {Name:Ada,Age:30,}
        /// </code>
        /// </example>
        /// <typeparam name="T">Type being serialized.</typeparam>
        public static string Serialize<T>(this T This) => SimpleSerializer<T>.Serialize(This);
    }

#pragma warning disable CA1000 // Allow static members on generic sample types
    public static class SimpleSerializer<T>
    {
#pragma warning disable RCS1250 // allow target-typed new() in samples
        private static readonly SimpleSerializerBuilder _builder = new();
#pragma warning restore RCS1250

        public static string Serialize(T instance) => $"{{{_builder.Function(instance)}}}";

        /// <summary>
        /// Builds a serializer string by concatenating per-property fragments using a seed/Conjunction pattern.
        /// </summary>
        /// <remarks>
        /// Demonstrates using <see cref="FunctionBuilder{TProto,TInstance,TResult}.Seed"/> and <see cref="FunctionBuilder{TProto,TInstance,TResult}.Conjunction"/>
        /// to fold property expressions into a single string result. Property values are converted to strings (with a special ISO format for DateTime)
        /// and concatenated using <see cref="string.Concat(string, string)"/>.
        /// </remarks>
        private sealed class SimpleSerializerBuilder : FunctionBuilder<T, T, string>
        {
            private static readonly MethodInfo _concat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });

            protected override string Seed => string.Empty;

            protected override Func<Expression, Expression, Expression> Conjunction => (_l, _r) => Expression.Call(_concat, _l, _r);

            private static MethodCallExpression GetToStringExpression(
                PropertyInfo propertyInfo,
                Expression instanceParameter)
            {
                return propertyInfo.PropertyType != typeof(DateTime)
                        ? Expression.Call(Expression.Property(instanceParameter, propertyInfo), "ToString", Type.EmptyTypes)
                        : Expression.Call(Expression.Property(instanceParameter, propertyInfo), "ToString", Type.EmptyTypes, Expression.Constant("O"));
            }

            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression instanceParameter) => Expression.Call(typeof(string), "Format", Type.EmptyTypes, Expression.Constant("{0}:{1},", typeof(string)), Expression.Constant(propertyInfo.Name, typeof(string)), GetToStringExpression(propertyInfo, instanceParameter));
        }
    }
#pragma warning restore CA1000
}
