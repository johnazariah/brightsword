#define BOOLEAN
#define BYTE
#define CHAR
#define INT16
#define INT32
#define INT64
#define SINGLE
#define DOUBLE
#define DECIMAL
#define STRING
#define TYPE
#define ENUM
#define STRING_ARRAY

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

using BrightSword.Crucible;
using BrightSword.Squid;
using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class SetDefaultValueTests
    {
        private readonly IInterfaceWithDefaultValue _instance = Dynamic<IInterfaceWithDefaultValue>.NewInstance();

        [TestMethod]
        public void TestTypeWasCreated()
        {
            Assert.IsNotNull(Dynamic<IInterfaceWithDefaultValue>.Type);
            Assert.IsNotNull(_instance);
        }

        private static void TestDefaultValueIsSet<T>(T expectedValue,
                                                     Func<T> accessor)
        {
            var actualValue = accessor();

            if (typeof(T).IsArray)
            {
                CollectionAssert.AreEqual((ICollection)expectedValue,
                                          (ICollection)actualValue);
                return;
            }

            Assert.AreEqual(expectedValue,
                            actualValue);
        }

#if BOOLEAN
        [TestMethod]
        public void TestBooleanDefaultValueIsSet()
        {
            TestDefaultValueIsSet(true,
                                  () => _instance.TrueBooleanProperty);
            TestDefaultValueIsSet(false,
                                  () => _instance.FalseBooleanProperty);
        }
#endif

#if BYTE

        [TestMethod]
        public void TestByteDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.ByteConstant,
                                  () => _instance.ByteProperty);
        }
#endif

#if CHAR
        [TestMethod]
        public void TestCharDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.CharConstant,
                                  () => _instance.CharProperty);
        }
#endif

#if INT16
        [TestMethod]
        public void TestInt16DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int16Constant,
                                  () => _instance.Int16Property);
        }
#endif

#if INT32
        [TestMethod]
        public void TestInt32DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int32Constant,
                                  () => _instance.Int32Property);
        }
#endif

#if INT64
        [TestMethod]
        public void TestInt64DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int64Constant,
                                  () => _instance.Int64Property);
        }
#endif

#if DOUBLE
        [TestMethod]
        public void TestDoubleDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.DoubleConstant,
                                  () => _instance.DoubleProperty);
        }
#endif

#if DECIMAL
        [TestMethod]
        public void TestDecimalDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.DecimalConstant,
                                  () => _instance.DecimalProperty);
        }
#endif

#if SINGLE
        [TestMethod]
        public void TestSingleDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.SingleConstant,
                                  () => _instance.SingleProperty);
        }
#endif

#if STRING
        [TestMethod]
        public void TestStringDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.StringConstant,
                                  () => _instance.StringProperty);
        }
#endif

#if TYPE
        [TestMethod]
        public void TestTypeDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.TypeConstant,
                                  () => _instance.TypeProperty);
        }
#endif

#if ENUM
        [TestMethod]
        public void TestEnumDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.EnumConstant,
                                  () => _instance.EnumProperty);
        }
#endif

#if STRING_ARRAY
        [TestMethod]
        public void TestArrayDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.StringArrayConstant,
                                  () => _instance.ArrayProperty);
        }
#endif

        [TestMethod]
        public void TestIllegalDefaultValueShouldThrow() => ExceptionHelper.ExpectException<NotSupportedException>(() => new BasicDataTransferObjectTypeCreator<IInterfaceWithNonSupportedDefaultValue>().CreateInstance());

        public static class Constants
        {
            public const byte ByteConstant = 129;
            public const char CharConstant = ';';
            public const decimal DecimalConstant = -112.125M;
            public const double DoubleConstant = 3.1415926D;
            public const float SingleConstant = 6.022E23F;
            public const short Int16Constant = 32223;
            public const int Int32Constant = -1288490000;
            public const long Int64Constant = 9223372036854775807;
            public const ushort UInt16Constant = 65535;
            public const uint UInt32Constant = 1288490000U;
            public const ulong UInt64Constant = 18446744073709551615UL;
            public const string StringConstant = "e^iπ + 1 = 0";
            public const SeekOrigin EnumConstant = SeekOrigin.End;
            public static readonly Type TypeConstant = typeof(ISerializable);

            public static readonly string[] StringArrayConstant =
            [
                "This",
                "is",
                "a",
                "string",
                "array"
            ];
        }

        public interface IInterfaceWithNonSupportedDefaultValue
        {
            [DefaultValue(typeof(Tuple<int, int>), "(12, 12)")]
            Tuple<int, int> SomeProperty { get; set; }
        }

        public interface IInterfaceWithDefaultValue
        {
#if BOOLEAN
            [DefaultValue(true)]
            bool TrueBooleanProperty { get; set; }

            [DefaultValue(false)]
            bool FalseBooleanProperty { get; set; }
#endif

#if BYTE
            [DefaultValue((byte)129)]
            byte ByteProperty { get; set; }
#endif

#if CHAR
            [DefaultValue(';')]
            char CharProperty { get; set; }
#endif

#if DOUBLE
            [DefaultValue(3.1415926D)]
            double DoubleProperty { get; set; }
#endif

#if DECIMAL
            [DefaultValue(typeof(decimal), "-112.125")]
            decimal DecimalProperty { get; set; }
#endif

#if SINGLE
            [DefaultValue(6.022E23F)]
            float SingleProperty { get; set; }
#endif

#if INT16
            [DefaultValue((short)32223)]
            short Int16Property { get; set; }
#endif

#if INT32
            [DefaultValue(-1288490000)]
            int Int32Property { get; set; }
#endif

#if INT64
            [DefaultValue(9223372036854775807)]
            long Int64Property { get; set; }
#endif

#if STRING
            [DefaultValue("e^iπ + 1 = 0")]
            string StringProperty { get; set; }
#endif

#if TYPE
            [DefaultValue(typeof(ISerializable))]
            Type TypeProperty { get; set; }
#endif

#if ENUM
            [DefaultValue(SeekOrigin.End)]
            SeekOrigin EnumProperty { get; set; }
#endif

#if STRING_ARRAY
            [DefaultValue(new[]
                          {
                              "This",
                              "is",
                              "a",
                              "string",
                              "array"
                          })]
            string[] ArrayProperty { get; set; }
#endif
        }

        // ReSharper disable once UnusedMember.Local
        private sealed class InterfaceWithDefaultValue : IInterfaceWithDefaultValue
        {
            public InterfaceWithDefaultValue()
            {
                TrueBooleanProperty = true;
                ByteProperty = 129;
                CharProperty = ';';
                DoubleProperty = 3.1415926D;
                DecimalProperty = 112.125M;
                Int16Property = 32223;
                Int32Property = -1288490000;
                Int64Property = 9223372036854775807;
                //_uint16Property = 65535;
                //_uint32Property = 1288490000U;
                //_uint64Property = 18446744073709551615UL;
                StringProperty = "e^iπ + 1 = 0";
                TypeProperty = typeof(ISerializable);
                EnumProperty = SeekOrigin.End;
                ArrayProperty =
                                 [
                                     "This",
                                     "is",
                                     "a",
                                     "string",
                                     "array"
                                 ];
            }

            // ReSharper disable ConvertToAutoProperty

            public Type TypeProperty { get; set; }

            public SeekOrigin EnumProperty { get; set; }

            public string[] ArrayProperty { get; set; }

            public char CharProperty { get; set; }

            public string StringProperty { get; set; }

            public bool TrueBooleanProperty { get; set; }

            public bool FalseBooleanProperty { get; set; }

            public byte ByteProperty { get; set; }

            public float SingleProperty { get; set; }

            public double DoubleProperty { get; set; }

            public decimal DecimalProperty { get; set; }

            public short Int16Property { get; set; }

            public int Int32Property { get; set; }

            public long Int64Property { get; set; }

            // ReSharper restore ConvertToAutoProperty
        }
    }
}
