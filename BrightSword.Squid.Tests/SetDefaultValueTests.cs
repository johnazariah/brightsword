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
        public void Test_TypeWasCreated()
        {
            Assert.IsNotNull(Dynamic<IInterfaceWithDefaultValue>.Type);
            Assert.IsNotNull(_instance);
        }

        private static void TestDefaultValueIsSet<T>(T expectedValue,
                                                     Func<T> accessor)
        {
            var actualValue = accessor();

            if (typeof (T).IsArray)
            {
                CollectionAssert.AreEqual((ICollection) expectedValue,
                                          (ICollection) actualValue);
                return;
            }

            Assert.AreEqual(expectedValue,
                            actualValue);
        }

#if BOOLEAN
        [TestMethod]
        public void Test_BooleanDefaultValueIsSet()
        {
            TestDefaultValueIsSet(true,
                                  () => _instance.TrueBooleanProperty);
            TestDefaultValueIsSet(false,
                                  () => _instance.FalseBooleanProperty);
        }
#endif

#if BYTE

        [TestMethod]
        public void Test_ByteDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.ByteConstant,
                                  () => _instance.ByteProperty);
        }
#endif

#if CHAR
        [TestMethod]
        public void Test_CharDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.CharConstant,
                                  () => _instance.CharProperty);
        }
#endif

#if INT16
        [TestMethod]
        public void Test_Int16DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int16Constant,
                                  () => _instance.Int16Property);
        }
#endif

#if INT32
        [TestMethod]
        public void Test_Int32DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int32Constant,
                                  () => _instance.Int32Property);
        }
#endif

#if INT64
        [TestMethod]
        public void Test_Int64DefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.Int64Constant,
                                  () => _instance.Int64Property);
        }
#endif

#if DOUBLE
        [TestMethod]
        public void Test_DoubleDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.DoubleConstant,
                                  () => _instance.DoubleProperty);
        }
#endif

#if DECIMAL
        [TestMethod]
        public void Test_DecimalDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.DecimalConstant,
                                  () => _instance.DecimalProperty);
        }
#endif

#if SINGLE
        [TestMethod]
        public void Test_SingleDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.SingleConstant,
                                  () => _instance.SingleProperty);
        }
#endif

#if STRING
        [TestMethod]
        public void Test_StringDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.StringConstant,
                                  () => _instance.StringProperty);
        }
#endif

#if TYPE
        [TestMethod]
        public void Test_TypeDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.TypeConstant,
                                  () => _instance.TypeProperty);
        }
#endif

#if ENUM
        [TestMethod]
        public void Test_EnumDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.EnumConstant,
                                  () => _instance.EnumProperty);
        }
#endif

#if STRING_ARRAY
        [TestMethod]
        public void Test_ArrayDefaultValueIsSet()
        {
            TestDefaultValueIsSet(Constants.StringArrayConstant,
                                  () => _instance.ArrayProperty);
        }
#endif

        [TestMethod]
        public void Test_IllegalDefaultValueShouldThrow()
        {
            ExceptionHelper.ExpectException<NotSupportedException>(() => new BasicDataTransferObjectTypeCreator<IInterfaceWithNonSupportedDefaultValue>().CreateInstance());
        }

        public static class Constants
        {
            public const Byte ByteConstant = 129;
            public const Char CharConstant = ';';
            public const Decimal DecimalConstant = -112.125M;
            public const Double DoubleConstant = 3.1415926D;
            public const Single SingleConstant = 6.022E23F;
            public const Int16 Int16Constant = 32223;
            public const Int32 Int32Constant = -1288490000;
            public const Int64 Int64Constant = 9223372036854775807;
            public const UInt16 UInt16Constant = 65535;
            public const UInt32 UInt32Constant = 1288490000U;
            public const UInt64 UInt64Constant = 18446744073709551615UL;
            public const String StringConstant = "e^iπ + 1 = 0";
            public const SeekOrigin EnumConstant = SeekOrigin.End;
            public static readonly Type TypeConstant = typeof (ISerializable);

            public static readonly string[] StringArrayConstant =
            {
                "This",
                "is",
                "a",
                "string",
                "array"
            };
        }

        public interface IInterfaceWithNonSupportedDefaultValue
        {
            [DefaultValue(typeof (Tuple<int, int>), "(12, 12)")]
            Tuple<int, int> SomeProperty { get; set; }
        }

        public interface IInterfaceWithDefaultValue
        {
#if BOOLEAN
            [DefaultValue(true)]
            Boolean TrueBooleanProperty { get; set; }

            [DefaultValue(false)]
            Boolean FalseBooleanProperty { get; set; }
#endif

#if BYTE
            [DefaultValue((byte) 129)]
            Byte ByteProperty { get; set; }
#endif

#if CHAR
            [DefaultValue(';')]
            Char CharProperty { get; set; }
#endif

#if DOUBLE
            [DefaultValue(3.1415926D)]
            Double DoubleProperty { get; set; }
#endif

#if DECIMAL
            [DefaultValue(typeof (decimal), "-112.125")]
            Decimal DecimalProperty { get; set; }
#endif

#if SINGLE
            [DefaultValue(6.022E23F)]
            Single SingleProperty { get; set; }
#endif

#if INT16
            [DefaultValue((short) 32223)]
            Int16 Int16Property { get; set; }
#endif

#if INT32
            [DefaultValue(-1288490000)]
            Int32 Int32Property { get; set; }
#endif

#if INT64
            [DefaultValue(9223372036854775807)]
            Int64 Int64Property { get; set; }
#endif

#if STRING
            [DefaultValue("e^iπ + 1 = 0")]
            String StringProperty { get; set; }
#endif

#if TYPE
            [DefaultValue(typeof (ISerializable))]
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
            String[] ArrayProperty { get; set; }
#endif
        }

// ReSharper disable once UnusedMember.Local
        private class InterfaceWithDefaultValue : IInterfaceWithDefaultValue
        {
            private string[] _arrayProperty;
            private byte _byteProperty;
            private char _charProperty;
            private decimal _decimalProperty;
            private double _doubleProperty;
            private SeekOrigin _enumProperty;
            private bool _falseBooleanProperty;
            private short _int16Property;
            private int _int32Property;
            private long _int64Property;
            private float _singleProperty;
            private string _stringProperty;
            private bool _trueBooleanProperty;
            private Type _typeProperty;

            public InterfaceWithDefaultValue()
            {
                _trueBooleanProperty = true;
                _byteProperty = 129;
                _charProperty = ';';
                _doubleProperty = 3.1415926D;
                _decimalProperty = 112.125M;
                _int16Property = 32223;
                _int32Property = -1288490000;
                _int64Property = 9223372036854775807;
                //_uint16Property = 65535;
                //_uint32Property = 1288490000U;
                //_uint64Property = 18446744073709551615UL;
                _stringProperty = "e^iπ + 1 = 0";
                _typeProperty = typeof (ISerializable);
                _enumProperty = SeekOrigin.End;
                _arrayProperty = new[]
                                 {
                                     "This",
                                     "is",
                                     "a",
                                     "string",
                                     "array"
                                 };
            }

// ReSharper disable ConvertToAutoProperty

            public Type TypeProperty
            {
                get { return _typeProperty; }
                set { _typeProperty = value; }
            }

            public SeekOrigin EnumProperty
            {
                get { return _enumProperty; }
                set { _enumProperty = value; }
            }

            public string[] ArrayProperty
            {
                get { return _arrayProperty; }
                set { _arrayProperty = value; }
            }

            public char CharProperty
            {
                get { return _charProperty; }
                set { _charProperty = value; }
            }

            public string StringProperty
            {
                get { return _stringProperty; }
                set { _stringProperty = value; }
            }

            public bool TrueBooleanProperty
            {
                get { return _trueBooleanProperty; }
                set { _trueBooleanProperty = value; }
            }

            public bool FalseBooleanProperty
            {
                get { return _falseBooleanProperty; }
                set { _falseBooleanProperty = value; }
            }

            public byte ByteProperty
            {
                get { return _byteProperty; }
                set { _byteProperty = value; }
            }

            public Single SingleProperty
            {
                get { return _singleProperty; }
                set { _singleProperty = value; }
            }

            public double DoubleProperty
            {
                get { return _doubleProperty; }
                set { _doubleProperty = value; }
            }

            public Decimal DecimalProperty
            {
                get { return _decimalProperty; }
                set { _decimalProperty = value; }
            }

            public short Int16Property
            {
                get { return _int16Property; }
                set { _int16Property = value; }
            }

            public int Int32Property
            {
                get { return _int32Property; }
                set { _int32Property = value; }
            }

            public long Int64Property
            {
                get { return _int64Property; }
                set { _int64Property = value; }
            }

// ReSharper restore ConvertToAutoProperty
        }
    }
}