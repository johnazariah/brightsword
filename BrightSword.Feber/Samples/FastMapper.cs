
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

        private sealed class DynamicToDynamicMapperBuilder : ActionBuilder<T, object, object>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression) => rightInstanceParameterExpression.GetDynamicPropertyMutatorExpression<T>(property, leftInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
        }

        private sealed class DynamicToStaticBackingFieldsMapperBuilder : ActionBuilder<T, T, object>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression)
            {
                var field = typeof(T).GetField($"_{property.Name[..1].ToLower(System.Globalization.CultureInfo.CurrentCulture)}{property.Name[1..]}", BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(field != null);
                return Expression.Assign(Expression.Field(leftInstanceParameterExpression, field), rightInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
            }
        }

        private sealed class DynamicToStaticMapperBuilder : ActionBuilder<T, T, object>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression) => Expression.Assign(Expression.Property(leftInstanceParameterExpression, property), rightInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
        }

        private sealed class StaticToDynamicMapperBuilder : ActionBuilder<T, object, T>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression) => leftInstanceParameterExpression.GetDynamicPropertyMutatorExpression<T>(property, Expression.Property(rightInstanceParameterExpression, property));
        }

        private sealed class StaticToStaticMapperBuilder : ActionBuilder<T, T, T>
        {
            protected override Expression PropertyExpression(
              PropertyInfo property,
              ParameterExpression leftInstanceParameterExpression,
              ParameterExpression rightInstanceParameterExpression) => Expression.Assign(Expression.Property(leftInstanceParameterExpression, property), Expression.Property(rightInstanceParameterExpression, property));
        }
    }
#pragma warning restore CA1000
}
