using System;
using System.Globalization;

namespace BrightSword.SwissKnife
{
    public static class CoerceExtensions
    {
        public static object CoerceType(this object value, Type targetType, object defaultValue)
        {
            if (value is string && targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(bool))
            {
                var str = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }

                var lower = str.ToLowerInvariant();
                return lower is not "y" and not "n" ? throw new ArgumentException("Boolean should be either 'y' or 'n'") : (object)(lower == "y");
            }

            if (targetType.IsEnum)
            {
                return value.CoerceType(targetType, out var returnValue, _ => true, (_type, _value) => Enum.Parse(_type, Convert.ToString(_value, CultureInfo.InvariantCulture), true), defaultValue)
                    ? returnValue
                    : throw new InvalidCastException($"Cannot cast {value} to {targetType}");
            }

            if (value.CoerceType(targetType, out var parsed, (_, _value) => bool.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture)), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => decimal.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => long.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => int.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => short.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => byte.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => char.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture)), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => double.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => float.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue) ||
                value.CoerceType(targetType, out parsed, (_, _value) => DateTime.Parse(Convert.ToString(_value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture), defaultValue))
            {
                return parsed;
            }

            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return value;
            }
        }

        public static bool CoerceType<T>(this object value, Type targetType, out object returnValue, Func<Type, object, T> parseFunc, object defaultValue)
            => value.CoerceType(targetType, out returnValue, _ => _.IsAssignableFrom(typeof(T)), (_type, _value) => parseFunc(_type, _value), defaultValue);

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
                if (defaultValue?.GetType().IsAssignableFrom(targetType) == true)
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
}
