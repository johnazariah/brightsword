
using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    /// <summary>
    /// Pretty-print an object's public properties to the console.
    /// </summary>
    /// <example>
    /// <code>
    /// public class Person { public string Name { get; set; } public int Age { get; set; } }
    /// var p = new Person { Name = "Ada", Age = 30 };
    /// // The extension method below calls into the compiled builder which writes each property to Console.
    /// p.Print();
    /// </code>
    /// </example>
    public static class PrettyPrinter
    {
#pragma warning disable CA1000 // Allow static members on generic sample types
        private static class PrettyPrinterImpl<TProto>
        {
#pragma warning disable RCS1250 // allow target-typed new() in samples
            private static readonly PrettyPrinterBuilder _builder = new();

            /// <summary>
            /// A simple pretty-printer builder that writes each property name/value pair to <see cref="Console"/>.
            /// </summary>
            /// <remarks>
            /// The <see cref="PropertyExpression(PropertyInfo, ParameterExpression)"/> override demonstrates how to build an expression that
            /// converts a property value to string (with special-casing for string properties) and then calls Console.WriteLine.
            /// The composed block will call WriteLine for each property when the <see cref="ActionBuilder{TProto,TInstance}.Action"/> delegate is executed.
            /// </remarks>
            private sealed class PrettyPrinterBuilder : ActionBuilder<TProto, TProto>
            {
                protected override Expression PropertyExpression(
                    PropertyInfo propertyInfo,
                    ParameterExpression instanceParameter)
                {
                    var memberExpression = Expression.Property(instanceParameter, propertyInfo);
                    Expression stringExpression = propertyInfo.PropertyType == typeof(string)
                        ? memberExpression
                        : Expression.Call(typeof(Convert), "ToString", Type.EmptyTypes, memberExpression);
                    return Expression.Call(typeof(Console), "WriteLine", Type.EmptyTypes, Expression.Constant("\t{0} : {1}", typeof(string)), Expression.Constant(propertyInfo.Name, typeof(string)), stringExpression);
                }
            }

            public static void Print(TProto instance) => _builder.Action(instance);
        }

        public static void Print<T>(this T This) => PrettyPrinterImpl<T>.Print(This);
    }
#pragma warning restore CA1000
}
