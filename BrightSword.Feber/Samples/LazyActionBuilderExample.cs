using System;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    /// <summary>
    /// Demonstrates using Lazy<T> to lazily initialize the compiled delegate in a thread-safe manner.
    /// This is a sample pattern; it does not modify core builder caching behavior.
    /// </summary>
    public sealed class LazyActionBuilderExample<TProto, TInstance> : ActionBuilder<TProto, TInstance>
    {
        private readonly Lazy<Action<TInstance>> _lazyAction;

        public LazyActionBuilderExample()
        {
            _lazyAction = new Lazy<Action<TInstance>>(() => BuildAction(), isThreadSafe: true);
        }

        public new Action<TInstance> Action => _lazyAction.Value;

        protected override Expression PropertyExpression(PropertyInfo propertyInfo, ParameterExpression instanceParameter)
        {
            // Create an expression that writes the property name and its value to the console.
            // memberExpression: instance.Property
            var memberExpression = Expression.Property(instanceParameter, propertyInfo);

            // Convert the member to object for safe boxing and to use Convert.ToString for nulls
            var objectConverted = Expression.Convert(memberExpression, typeof(object));

            // Call Convert.ToString(object) to get a string representation that handles nulls
            var toStringCall = Expression.Call(typeof(Convert), "ToString", Type.EmptyTypes, objectConverted);

            // Format string and call Console.WriteLine(format, name, value)
            var format = Expression.Constant("\t{0} : {1}", typeof(string));
            var nameConst = Expression.Constant(propertyInfo.Name, typeof(string));

            return Expression.Call(typeof(Console), "WriteLine", Type.EmptyTypes, format, nameConst, toStringCall);
        }
    }
}
