using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.CSharp.RuntimeBinder;

namespace BrightSword.Squid.Behaviours
{
    public class FieldValueSetInstructionHelper
    {
        internal IEnumerable<Action<ILGenerator>> GenerateCodeToSetFieldValue(FieldInfo field,
                                                                              object value)
        {
            try
            {
                return GenerateCode(field,
                                    (dynamic) value);
            }
            catch (RuntimeBinderException)
            {
                throw new NotSupportedException(String.Format("Cannot set default value for {0}",
                                                              field.Name));
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

            var isNegative = (0x1 << 31 & bits[3]) != 0;
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     isNegative
                                         ? 1
                                         : 0);

            var scale = (0x000F0000 & bits[3]) >> 16;
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     scale);
            yield return _ => _.Emit(OpCodes.Newobj,
                                     typeof (Decimal).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                     null,
                                                                     new[]
                                                                     {
                                                                         typeof (Int32),
                                                                         typeof (Int32),
                                                                         typeof (Int32),
                                                                         typeof (Boolean),
                                                                         typeof (Byte)
                                                                     },
                                                                     null));
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        private static Action<ILGenerator> GenerateCodeToWriteIntegralValue(long value)
        {
            if (value >= sbyte.MinValue
                && value <= sbyte.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4_S,
                                   (sbyte) value);
            }

            if (value >= byte.MinValue
                && value <= byte.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4_S,
                                   (byte) value);
            }

            if (value >= int.MinValue
                && value <= int.MaxValue)
            {
                return _ => _.Emit(OpCodes.Ldc_I4,
                                   (int) value);
            }

            return _ => _.Emit(OpCodes.Ldc_I8,
                               value);
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        Enum value)
        {
            yield return _ => _.Emit(OpCodes.Ldarg_0);

            yield return GenerateCodeToWriteIntegralValue((long) Convert.ChangeType(value,
                                                                                    typeof (long)));

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
                                     typeof (Type).GetMethod("GetTypeFromHandle",
                                                             BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                             null,
                                                             new[]
                                                             {
                                                                 typeof (RuntimeTypeHandle)
                                                             },
                                                             null));
            yield return _ => _.Emit(OpCodes.Stfld,
                                     field);
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCodeForArray<T>(IList<T> value,
                                                                                   FieldInfo field,
                                                                                   Func<T, Action<ILGenerator>[]> itemSetInstructions)
        {
            yield return _ => _.DeclareLocal(typeof (T[]));

            yield return _ => _.Emit(OpCodes.Ldarg_0);
            yield return _ => _.Emit(OpCodes.Ldc_I4,
                                     value.Count);
            yield return _ => _.Emit(OpCodes.Newarr,
                                     typeof (T));
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
                                        _item => new Action<ILGenerator>[]
                                                 {
                                                     _ => _.Emit(OpCodes.Ldc_I4,
                                                                 _item),
                                                     _ => _.Emit(OpCodes.Stelem_I4)
                                                 });
        }

        protected virtual IEnumerable<Action<ILGenerator>> GenerateCode(FieldInfo field,
                                                                        IList<string> value)
        {
            return GenerateCodeForArray(value,
                                        field,
                                        _item => new Action<ILGenerator>[]
                                                 {
                                                     _ => _.Emit(OpCodes.Ldstr,
                                                                 _item),
                                                     _ => _.Emit(OpCodes.Stelem_Ref)
                                                 });
        }
    }
}