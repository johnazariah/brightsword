using System;

namespace BrightSword.SwissKnife;

public static class CoerceExtensions
{
    public static object CoerceType(this object value, Type targetType, object defaultValue)
    {
        if (value is string && targetType == typeof(string))
            return value;

        if (targetType == typeof(bool))
        {
            var str = value.ToString();
            if (string.IsNullOrEmpty(str))
                return false;
            if (str.Equals("y", StringComparison.OrdinalIgnoreCase))
                return true;
            if (str.Equals("n", StringComparison.OrdinalIgnoreCase))
                return false;
            throw new ArgumentException("Boolean should be either 'y' or 'n'");
        }

        if (targetType.IsEnum)
        {
            if (value.CoerceType(targetType, out var returnValue, _ => true, (_type, _value) => Enum.Parse(_type, _value.ToString(), true), defaultValue))
                return returnValue;
            throw new InvalidCastException($"Cannot cast {value} to {targetType}");
        }

        if (value.CoerceType<bool>(targetType, out var parsed, (_, _value) => bool.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<decimal>(targetType, out parsed, (_, _value) => decimal.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<long>(targetType, out parsed, (_, _value) => long.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<int>(targetType, out parsed, (_, _value) => int.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<short>(targetType, out parsed, (_, _value) => short.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<byte>(targetType, out parsed, (_, _value) => byte.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<char>(targetType, out parsed, (_, _value) => char.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<double>(targetType, out parsed, (_, _value) => double.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<float>(targetType, out parsed, (_, _value) => float.Parse(_value.ToString()), defaultValue) ||
            value.CoerceType<DateTime>(targetType, out parsed, (_, _value) => DateTime.Parse(_value.ToString()), defaultValue))
            return parsed;

        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return value;
        }
    }

    public static bool CoerceType<T>(this object value, Type targetType, out object returnValue, Func<Type, object, T> parseFunc, object defaultValue)
        => value.CoerceType(targetType, out returnValue, _ => _.IsAssignableFrom(typeof(T)), (_type, _value) => (object)parseFunc(_type, _value), defaultValue);

    public static bool CoerceType(this object value, Type targetType, out object returnValue, Func<Type, bool> checkFunc, Func<Type, object, object> parseFunc, object defaultValue)
    {
        try
        {
            if (checkFunc(targetType))
            {
                returnValue = parseFunc(targetType, value);
                return true;
            }
        }
        catch
        {
            if (defaultValue != null && defaultValue.GetType().IsAssignableFrom(targetType))
            {
                returnValue = defaultValue;
                return true;
            }

            returnValue = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            return true;
        }

        returnValue = null;
        return false;
    }
}
