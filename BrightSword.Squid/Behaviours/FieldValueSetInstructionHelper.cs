using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
// Microsoft.CSharp.RuntimeBinder removed: use explicit type-based dispatch instead of dynamic

namespace BrightSword.Squid.Behaviours
{
    /// <summary>
    /// Helper that generates IL instructions to set field values for emitted types.
    /// This class yields <see cref="Action{ILGenerator}"/> sequences that emit the
    /// required opcodes to initialize fields with given values.
    /// </summary>
    public class FieldValueSetInstructionHelper
    {
        // Cached parameter type arrays to avoid repeated allocations (CA1861)
        private static readonly Type[] GetTypeFromHandleArgTypes = [typeof(RuntimeTypeHandle)];
        private static readonly MethodInfo GetTypeFromHandleMethod = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, GetTypeFromHandleArgTypes, null);

        /// <summary>
        /// Generate IL instructions to set <paramref name="field"/> to <paramref name="value"/>.
        /// This method supports a variety of simple runtime types (primitives, enums, decimal,
        /// string, Type, arrays of simple types and IList{int}/IList{string}) and will attempt to
        /// use <see cref="TypeConverter"/> conversions for string-typed attribute values when
        /// appropriate. For unsupported combinations a <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <param name="field">The target field to initialize.</param>
        /// <param name="value">The value to set for the field. May be null for reference types.</param>
        /// <returns>An enumerable of actions which, when executed against an <see cref="ILGenerator"/>, emit the instructions to set the field.</returns>
        internal IEnumerable<Action<ILGenerator>> GenerateCodeToSetFieldValue(FieldInfo field,
                                                                              object value)
        {
            if (field == null)
            {
                ArgumentNullException.ThrowIfNull(field);
            }

            // Handle nulls explicitly: reference types may accept null, value types do not here.
            if (value == null)
            {
                return field.FieldType.IsValueType
                    ? throw new NotSupportedException($"Cannot set default value for {field.Name}")
                    : [
                    (_ => _.Emit(OpCodes.Ldarg_0)),
                    _ => _.Emit(OpCodes.Ldnull),
                    _ => _.Emit(OpCodes.Stfld, field)
                ];
            }

            var valueType = value.GetType();

            if (value is bool b) { return GenerateCode(field, b); }
            if (value is char c) { return GenerateCode(field, c); }
            if (value is float f) { return GenerateCode(field, f); }
            if (value is double d) { return GenerateCode(field, d); }
            if (value is long l) { return GenerateCode(field, l); }
            if (value is decimal m) { return GenerateCode(field, m); }
            if (value is string s)
            {
                if (field.FieldType == typeof(string))
                {
                    return GenerateCode(field, s);
                }

                // Try using a TypeConverter to convert the string into the target field type.
                var converter = TypeDescriptor.GetConverter(field.FieldType);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        var converted = converter.ConvertFrom(null, CultureInfo.InvariantCulture, s);
                        if (converted != null)
                        {
                            return GenerateCodeToSetFieldValue(field, converted);
                        }
                    }
                    catch
                    {
                        // fall through to NotSupported
                    }
                }

                throw new NotSupportedException($"Cannot set default value for {field.Name}");
            }
            if (value is Type t) { return GenerateCode(field, t); }

            if (valueType.IsEnum) { return GenerateCode(field, (Enum)value); }

            // Numeric types that fit into Int64
            if (value is sbyte or byte or short or ushort or int or uint)
            {
                try
                {
                    var asLong = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    return GenerateCode(field, asLong);
                }
                catch (OverflowException)
                {
                    throw new NotSupportedException($"Cannot set default value for {field.Name}");
                }
            }

            if (value is IList<int> ints)
            {
                return GenerateCode(field, ints);
            }

            if (value is IList<string> strings)
            {
                return GenerateCode(field, strings);
            }

            // Fallback: only try to convert for simple target types (numbers, enum, string, decimal, Type, arrays, IList<int>/IList<string>)
            static bool IsSimpleTarget(Type t)
            {
                if (t.IsPrimitive || t.IsEnum)
                {
                    return true;
                }

                if (t == typeof(decimal) || t == typeof(string) || t == typeof(Type))
                {
                    return true;
                }

                if (t.IsArray)
                {
                    return true;
                }

                return typeof(IList<int>).IsAssignableFrom(t) || typeof(IList<string>).IsAssignableFrom(t);
            }

            if (IsSimpleTarget(field.FieldType))
            {
                try
                {
                    if (value is IConvertible)
                    {
                        var converted = Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture);
                        if (converted != null && converted.GetType() != valueType)
                        {
                            return GenerateCodeToSetFieldValue(field, converted);
                        }
                    }
                }
                catch
                {
                    // Ignore conversion errors and fall through to NotSupported
                }
            }
            else
            {
                // For complex target types we don't attempt conversions; preserve original "not supported" behavior.
                throw new NotSupportedException($"Cannot set default value for {field.Name}");
            }

            throw new NotSupportedException($"Cannot set default value for {field.Name}");
        }

        /// <summary>
        /// Generate IL to set a boolean field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        bool value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return GenerateCodeToWriteIntegralValue(value
                                                              ? 1
                                                              : 0);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to set a char field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        char value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return GenerateCodeToWriteIntegralValue(value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to set a float field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        float value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldc_R4,
                                     value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        // takes care of unsigned byte, char, short, int
        /// <summary>
        /// Generate IL to set a double field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        double value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldc_R8,
                                     value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to set a long/integral field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        long value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return GenerateCodeToWriteIntegralValue(value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to set a string field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        string value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldstr,
                                     value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to set a decimal field by emitting the constructor that takes the internal bits.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        decimal value)
        {
            var bits = decimal.GetBits(value);

            yield return _ => _.Emit(OpCodes.Ldarg_0);

            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     bits[0]);
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     bits[1]);
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     bits[2]);

            var isNegative = ((0x1 << 31) & bits[3]) != 0;
            yield return _ => _.Emit(OpCodes.Ldc_I4, isNegative ? 1 : 0);

            var scale = (0x000F0000 & bits[3]) >> 16;
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     scale);
            yield return _ => _.Emit(OpCodes.Newobj,
                                     typeof(decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                     null,
                                                                     [
                                                                         typeof(int),
                                                                         typeof(int),
                                                                         typeof(int),
                                                                         typeof(bool),
                                                                         typeof(byte)
                                                                     ],
                                                                     null));
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        private static Action<ILGenerator> GenerateCodeToWriteIntegralValue(long value)
        {
            // Prefer the smallest opcode that can hold the value where possible.
            if (value is >= sbyte.MinValue and <= sbyte.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            }

            if (value is >= byte.MinValue and <= byte.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4_S, (byte)value);
            }

            return value is >= int.MinValue and <= int.MaxValue ? (_ => _.Emit(OpCodes.Ldc_I4, (int)value)) : (_ => _.Emit(OpCodes.Ldc_I8, value));
        }

        /// <summary>
        /// Generate IL to set an enum-typed field by writing its integral value.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        Enum value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);

            yield return GenerateCodeToWriteIntegralValue((long)Convert.ChangeType(value,
                                                                                    typeof(long),
                                                                                    CultureInfo.InvariantCulture));

            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to store a System.Type instance into a field using the <c>ldtoken</c> instruction
        /// followed by a call to <see cref="Type.GetTypeFromHandle"/>.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        Type value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldtoken,
                                     value);
            yield return _ => _.Emit(OpCodes.Call,
                                     typeof(Type).GetMethod("GetTypeFromHandle",
                                                             BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                             null,
                                                             GetTypeFromHandleArgTypes,
                                                             null));
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL instructions for populating an array field with the provided list items.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCodeForArray<T>(IList<T> value,
                                                                                   FieldInfo field,
                                                                                   Func<T, Action<ILGenerator>[]> itemSetInstructions)
        {
            yield return _ => _.DeclareLocal(typeof(T[]));

            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     value.Count);
            yield return _ => _.Emit(OpCodes.Newarr,
                                     typeof(T));
            yield return _ => _.Emit(OpCodes.Stloc_0);
            yield return _ => _.Emit(OpCodes.Ldloc_0);

            for (var index = 0;
                 index < value.Count;
                 index++)
            {
                var _index = index;
                yield return _ => _.Emit(OpCodes.Ldc_I4,
                                         _index);

                foreach (var itemSetInstruction in itemSetInstructions(value[_index]))
                {
                    yield return itemSetInstruction;
                }

                yield return _ => _.Emit(OpCodes.Ldloc_0);
            }

            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        /// <summary>
        /// Generate IL to initialize an integer array field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        IList<int> value)
        {
            return GenerateCodeForArray(value,
                                        field,
                                        _item => [(_ => _.Emit(OpCodes.Ldc_I4, _item)), _ => _.Emit(OpCodes.Stelem_I4)]);
        }

        /// <summary>
        /// Generate IL to initialize a string array field.
        /// </summary>
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        IList<string> value)
        {
            return GenerateCodeForArray(value,
                                        field,
                                        _item => [(_ => _.Emit(OpCodes.Ldstr, _item)), _ => _.Emit(OpCodes.Stelem_Ref)]);
        }
    }
}
