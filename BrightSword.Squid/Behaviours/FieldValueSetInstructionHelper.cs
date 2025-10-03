using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp.RuntimeBinder;

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
        private static readonly Type[] GetTypeFromHandleArgTypes = new[] { typeof(RuntimeTypeHandle) };
        private static readonly MethodInfo GetTypeFromHandleMethod = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, GetTypeFromHandleArgTypes, null);

        /// <summary>
        /// Generate IL instructions to set <paramref name="field"/> to <paramref name="value"/>.
        /// </summary>
        internal IEnumerable<Action<ILGenerator>> GenerateCodeToSetFieldValue(FieldInfo field,
                                                                              object value)
        {
            if (field == null)
            {
                ArgumentNullException.ThrowIfNull(field);
            }

            try
            {
                return GenerateCode(field,
                                    (dynamic)value);
            }
            catch (RuntimeBinderException)
            {
                throw new NotSupportedException($"Cannot set default value for {field.Name}");
            }
        }

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

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        char value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return GenerateCodeToWriteIntegralValue(value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

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
        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        double value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldc_R8,
                                     value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        long value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return GenerateCodeToWriteIntegralValue(value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        string value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldstr,
                                     value);
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        decimal value)
        {
            var bits = Decimal.GetBits(value);

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
                                                                     new[]
                                                                     {
                                                                         typeof(int),
                                                                         typeof(int),
                                                                         typeof(int),
                                                                         typeof(bool),
                                                                         typeof(byte)
                                                                     },
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

            if (value is >= int.MinValue and <= int.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4, (int)value);
            }

            return _ => _.Emit(OpCodes.Ldc_I8, value);
        }

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

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        IList<int> value)
        {
            return GenerateCodeForArray(value,
                                        field,
                                        _item => new Action<ILGenerator>[] { _ => _.Emit(OpCodes.Ldc_I4, _item), _ => _.Emit(OpCodes.Stelem_I4) });
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        IList<string> value)
        {
            return GenerateCodeForArray(value,
                                        field,
                                        _item => new Action<ILGenerator>[] { _ => _.Emit(OpCodes.Ldstr, _item), _ => _.Emit(OpCodes.Stelem_Ref) });
        }
    }
}
