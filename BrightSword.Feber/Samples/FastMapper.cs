
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using BrightSword.Feber.Core;

namespace BrightSword.Feber.Samples
{
    public static class FastMapper
    {
        public static T MapStaticToStatic<T>(this T This, T source)
        {
            FastMapper<T>.MapStaticToStatic(source, This);
            return This;
        }

        public static T MapDynamicToStatic<T>(this T This, object source)
        {
            FastMapper<T>.MapDynamicToStatic(source, This);
            return This;
        }

        public static object MapStaticToDynamic<T>(this object This, T source)
        {
            FastMapper<T>.MapStaticToDynamic(source, This);
            return This;
        }

        public static object MapDynamicToDynamic<T>(this object This, object source)
        {
            FastMapper<T>.MapDynamicToDynamic(source, This);
            return This;
        }

        public static object MapToBackingFields<T>(this T This, object source)
        {
            FastMapper<T>.MapToBackingFields(source, This);
            return This;
        }
    }

#pragma warning disable CA1000 // Allow static members on generic sample types
    public static class FastMapper<T>
    {
#pragma warning disable RCS1250 // Allow target-typed new() in samples
        private static readonly StaticToStaticMapperBuilder _staticToStaticMapper = new();
        private static readonly DynamicToDynamicMapperBuilder _dynamicToDynamicMapper = new();
        private static readonly StaticToDynamicMapperBuilder _staticToDynamicMapper = new();
        private static readonly DynamicToStaticMapperBuilder _dynamicToStaticMapper = new();
        private static readonly DynamicToStaticBackingFieldsMapperBuilder _dynamicToStaticBackingFieldsMapper = new();
#pragma warning restore RCS1250

        public static void MapDynamicToDynamic(object source, object destination) => _dynamicToDynamicMapper.Action(destination, source);

        public static void MapStaticToStatic(T source, T destination) => _staticToStaticMapper.Action(destination, source);

        public static void MapStaticToDynamic(T source, object destination) => _staticToDynamicMapper.Action(destination, source);

        public static void MapDynamicToStatic(object source, T destination) => _dynamicToStaticMapper.Action(destination, source);

        public static void MapToBackingFields(object source, T destination) => _dynamicToStaticBackingFieldsMapper.Action(destination, source);

        /// <summary>
        /// Maps from a dynamic source (object) to a dynamic destination (object) using runtime binder expressions.
        /// </summary>
        /// <remarks>
        /// Demonstrates <see cref="DynamicExpressionUtilities.GetDynamicPropertyAccessorExpression{T}"/> and
        /// <see cref="DynamicExpressionUtilities.GetDynamicPropertyMutatorExpression{T}"/> to read/write properties dynamically in expression trees.
        /// </remarks>
        private sealed class DynamicToDynamicMapperBuilder : ActionBuilder<T, object, object>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter) => rightInstanceParameter.GetDynamicPropertyMutatorExpression<T>(propertyInfo, leftInstanceParameter.GetDynamicPropertyAccessorExpression<T>(propertyInfo));
        }

        /// <summary>
        /// Maps from a dynamic source to static destination backing fields (private fields) when properties are implemented with backing fields.
        /// </summary>
        /// <remarks>
        /// Shows how to locate a backing field conventionally named _propertyName and assign the dynamic value to that field.
        /// Useful if the destination exposes read-only properties backed by private fields.
        /// </remarks>
        private sealed class DynamicToStaticBackingFieldsMapperBuilder : ActionBuilder<T, T, object>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter)
            {
                var field = typeof(T).GetField($"_{propertyInfo.Name[..1].ToLower(System.Globalization.CultureInfo.CurrentCulture)}{propertyInfo.Name[1..]}", BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(field is not null);
                return Expression.Assign(Expression.Field(leftInstanceParameter, field), rightInstanceParameter.GetDynamicPropertyAccessorExpression<T>(propertyInfo));
            }
        }

        /// <summary>
        /// Maps dynamic source properties into static destination properties.
        /// </summary>
        private sealed class DynamicToStaticMapperBuilder : ActionBuilder<T, T, object>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter) => Expression.Assign(Expression.Property(leftInstanceParameter, propertyInfo), rightInstanceParameter.GetDynamicPropertyAccessorExpression<T>(propertyInfo));
        }

        /// <summary>
        /// Maps static source properties into a dynamic destination by emitting dynamic mutator calls.
        /// </summary>
        private sealed class StaticToDynamicMapperBuilder : ActionBuilder<T, object, T>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter) => leftInstanceParameter.GetDynamicPropertyMutatorExpression<T>(propertyInfo, Expression.Property(rightInstanceParameter, propertyInfo));
        }

        /// <summary>
        /// Simple static-to-static mapper: generates assignments between matching properties.
        /// </summary>
        private sealed class StaticToStaticMapperBuilder : ActionBuilder<T, T, T>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression leftInstanceParameter,
                ParameterExpression rightInstanceParameter) => Expression.Assign(Expression.Property(leftInstanceParameter, propertyInfo), Expression.Property(rightInstanceParameter, propertyInfo));
        }
    }
#pragma warning restore CA1000
}
