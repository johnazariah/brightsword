namespace BrightSword.Feber.Samples;

using BrightSword.Feber.Core;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
public static class FastMapper
{
    public static T MapStaticToStatic<T>(this T _this, T source)
    {
        FastMapper<T>.MapStaticToStatic(source, _this);
        return _this;
    }

    public static T MapDynamicToStatic<T>(this T _this, object source)
    {
        FastMapper<T>.MapDynamicToStatic(source, _this);
        return _this;
    }

    public static object MapStaticToDynamic<T>(this object _this, T source)
    {
        FastMapper<T>.MapStaticToDynamic(source, _this);
        return _this;
    }

    public static object MapDynamicToDynamic<T>(this object _this, object source)
    {
        FastMapper<T>.MapDynamicToDynamic(source, _this);
        return _this;
    }

    public static object MapToBackingFields<T>(this T _this, object source)
    {
        FastMapper<T>.MapToBackingFields(source, _this);
        return (object)_this;
    }
}

public static class FastMapper<T>
{
    private static readonly FastMapper<T>.StaticToStaticMapperBuilder _staticToStaticMapper = new FastMapper<T>.StaticToStaticMapperBuilder();
    private static readonly FastMapper<T>.DynamicToDynamicMapperBuilder _dynamicToDynamicMapper = new FastMapper<T>.DynamicToDynamicMapperBuilder();
    private static readonly FastMapper<T>.StaticToDynamicMapperBuilder _staticToDynamicMapper = new FastMapper<T>.StaticToDynamicMapperBuilder();
    private static readonly FastMapper<T>.DynamicToStaticMapperBuilder _dynamicToStaticMapper = new FastMapper<T>.DynamicToStaticMapperBuilder();
    private static readonly FastMapper<T>.DynamicToStaticBackingFieldsMapperBuilder _dynamicToStaticBackingFieldsMapper = new FastMapper<T>.DynamicToStaticBackingFieldsMapperBuilder();

    public static void MapDynamicToDynamic(object source, object destination)
    {
        FastMapper<T>._dynamicToDynamicMapper.Action(destination, source);
    }

    public static void MapStaticToStatic(T source, T destination)
    {
        FastMapper<T>._staticToStaticMapper.Action(destination, source);
    }

    public static void MapStaticToDynamic(T source, object destination)
    {
        FastMapper<T>._staticToDynamicMapper.Action(destination, source);
    }

    public static void MapDynamicToStatic(object source, T destination)
    {
        FastMapper<T>._dynamicToStaticMapper.Action(destination, source);
    }

    public static void MapToBackingFields(object source, T destination)
    {
        FastMapper<T>._dynamicToStaticBackingFieldsMapper.Action(destination, source);
    }

    private class DynamicToDynamicMapperBuilder : ActionBuilder<T, object, object>
    {
        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            return rightInstanceParameterExpression.GetDynamicPropertyMutatorExpression<T>(property, leftInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
        }
    }

    private class DynamicToStaticBackingFieldsMapperBuilder : ActionBuilder<T, T, object>
    {
        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            FieldInfo field = typeof(T).GetField($"_{property.Name.Substring(0, 1).ToLower()}{property.Name.Substring(1)}", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(field != (FieldInfo)null);
            return (Expression)Expression.Assign((Expression)Expression.Field((Expression)leftInstanceParameterExpression, field), rightInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
        }
    }

    private class DynamicToStaticMapperBuilder : ActionBuilder<T, T, object>
    {
        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            return (Expression)Expression.Assign((Expression)Expression.Property((Expression)leftInstanceParameterExpression, property), rightInstanceParameterExpression.GetDynamicPropertyAccessorExpression<T>(property));
        }
    }

    private class StaticToDynamicMapperBuilder : ActionBuilder<T, object, T>
    {
        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            return leftInstanceParameterExpression.GetDynamicPropertyMutatorExpression<T>(property, (Expression)Expression.Property((Expression)rightInstanceParameterExpression, property));
        }
    }

    private class StaticToStaticMapperBuilder : ActionBuilder<T, T, T>
    {
        protected override Expression PropertyExpression(
          PropertyInfo property,
          ParameterExpression leftInstanceParameterExpression,
          ParameterExpression rightInstanceParameterExpression)
        {
            return (Expression)Expression.Assign((Expression)Expression.Property((Expression)leftInstanceParameterExpression, property), (Expression)Expression.Property((Expression)rightInstanceParameterExpression, property));
        }
    }
}
