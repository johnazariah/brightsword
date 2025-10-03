using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;

namespace BrightSword.Squid.Behaviours
{
    public class CloneBehaviour : IBehaviour
    {
        public virtual IEnumerable<Func<TypeBuilder, TypeBuilder>> Operations
        {
            get
            {
                yield return _ => _.AddCustomAttribute<SerializableAttribute>();
                yield return AddCloneMethod;
            }
        }

        public virtual TypeBuilder AddCloneMethod(TypeBuilder typeBuilder)
        {
            var method = typeBuilder.DefineMethod("Clone",
                                                  MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                                                  typeof (Object),
                                                  new Type[]
                                                  {});

            var gen = method.GetILGenerator();

            gen.DeclareLocal(typeof (BinaryFormatter));
            gen.DeclareLocal(typeof (MemoryStream));
            gen.DeclareLocal(typeof (Object));
            gen.DeclareLocal(typeof (Boolean));

            var labelReturnAndExit = gen.DefineLabel();
            var labelEndFinally = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);

            gen.Emit(OpCodes.Newobj,
                     typeof (BinaryFormatter).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                             null,
                                                             new Type[]
                                                             {},
                                                             null));
            gen.Emit(OpCodes.Stloc_0);

            gen.Emit(OpCodes.Newobj,
                     typeof (MemoryStream).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                          null,
                                                          new Type[]
                                                          {},
                                                          null));
            gen.Emit(OpCodes.Stloc_1);

            gen.BeginExceptionBlock();
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt,
                     typeof (BinaryFormatter).GetMethod("Serialize",
                                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                        null,
                                                        new[]
                                                        {
                                                            typeof (Stream),
                                                            typeof (Object)
                                                        },
                                                        null));
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Conv_I8);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Callvirt,
                     typeof (Stream).GetMethod("Seek",
                                               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                               null,
                                               new[]
                                               {
                                                   typeof (Int64),
                                                   typeof (SeekOrigin)
                                               },
                                               null));
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc_1);

            gen.Emit(OpCodes.Callvirt,
                     typeof (BinaryFormatter).GetMethod("Deserialize",
                                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                        null,
                                                        new[]
                                                        {
                                                            typeof (Stream)
                                                        },
                                                        null));
            gen.Emit(OpCodes.Stloc_2);
            gen.Emit(OpCodes.Leave_S,
                     labelReturnAndExit);
            gen.BeginFinallyBlock();
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldnull);

            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Stloc_3);
            gen.Emit(OpCodes.Ldloc_3);
            gen.Emit(OpCodes.Brtrue_S,
                     labelEndFinally);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Callvirt,
                     typeof (IDisposable).GetMethod("Dispose",
                                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                    null,
                                                    new Type[]
                                                    {},
                                                    null));
            gen.Emit(OpCodes.Nop);
            gen.MarkLabel(labelEndFinally);
            gen.Emit(OpCodes.Endfinally);
            gen.EndExceptionBlock();
            gen.MarkLabel(labelReturnAndExit);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldloc_2);
            gen.Emit(OpCodes.Ret);

            return typeBuilder;
        }
    }
}