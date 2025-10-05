using System.Globalization;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for type coercion and conversion with fallback/default handling.
    /// </summary>
    /// <remarks>
    /// These helpers simplify safe type conversion, including enums and primitives, with custom parsing logic.
    /// </remarks>
    public static class CoerceExtensions
    {
        /// <summary>
        /// Attempts to coerce an object to the specified target type, using default value if conversion fails.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="defaultValue">The value to use if conversion fails.</param>
        /// <returns>The converted value, or <paramref name="defaultValue"/> if conversion fails.</returns>
        /// <example>
        /// <code>
        /// object val = "42";
        /// int result = (int)val.CoerceType(typeof(int), 0); // 42
        /// </code>
        /// </example>
        [Obsolete("Use built-in Convert.ChangeType, TryParse, or pattern matching in modern C#. CoerceType is superseded by language features.")]
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

        /// <summary>
        /// Attempts to coerce an object to the specified target type using a custom parse function.
        /// </summary>
        /// <typeparam name="T">The type to parse to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="returnValue">The parsed value if successful.</param>
        /// <param name="parseFunc">Custom parse function.</param>
        /// <param name="defaultValue">The value to use if conversion fails.</param>
        /// <returns>True if conversion succeeded, false otherwise.</returns>
        /// <example>
        /// <code>
        /// object val = "true";
        /// bool success = val.CoerceType<bool>(typeof(bool), out var result, (_, v) => bool.Parse(v.ToString()), false);
        /// </code>
        /// </example>
        [Obsolete("Use built-in Convert.ChangeType, TryParse, or pattern matching in modern C#. CoerceType is superseded by language features.")]
        public static bool CoerceType<T>(this object value, Type targetType, out object returnValue, Func<Type, object, T> parseFunc, object defaultValue)
            => value.CoerceType(targetType, out returnValue, _ => _.IsAssignableFrom(typeof(T)), (_type, _value) => parseFunc(_type, _value), defaultValue);

        /// <summary>
        /// Attempts to coerce an object to the specified target type using custom check and parse functions.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="returnValue">The parsed value if successful.</param>
        /// <param name="checkFunc">Function to check if the type is supported.</param>
        /// <param name="parseFunc">Custom parse function.</param>
        /// <param name="defaultValue">The value to use if conversion fails.</param>
        /// <returns>True if conversion succeeded, false otherwise.</returns>
        /// <example>
        /// <code>
        /// object val = "42";
        /// bool success = val.CoerceType(typeof(int), out var result, t => t == typeof(int), (t, v) => int.Parse(v.ToString()), 0);
        /// </code>
        /// </example>
        [Obsolete("Use built-in Convert.ChangeType, TryParse, or pattern matching in modern C#. CoerceType is superseded by language features.")]
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
