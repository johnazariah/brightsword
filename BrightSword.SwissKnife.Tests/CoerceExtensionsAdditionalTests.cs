namespace BrightSword.SwissKnife.Tests
{
    public class CoerceExtensionsAdditionalTests
    {
        [Fact]
        public void CoerceType_SameType_ReturnsValue()
        {
#pragma warning disable CS0618
            var result = "hello".CoerceType(typeof(string), null);
#pragma warning restore CS0618
            Assert.Equal("hello", result);
        }

        [Fact]
        public void CoerceType_StringToBool_Empty_ReturnsFalse()
        {
#pragma warning disable CS0618
            Assert.Equal(false, "".CoerceType(typeof(bool), true));
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_StringToBool_Invalid_Throws()
        {
#pragma warning disable CS0618
            Assert.Throws<ArgumentException>(() => "maybe".CoerceType(typeof(bool), false));
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_StringToBool_CaseInsensitive_Y()
        {
#pragma warning disable CS0618
            Assert.Equal(true, "Y".CoerceType(typeof(bool), false));
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_StringToBool_CaseInsensitive_N()
        {
#pragma warning disable CS0618
            Assert.Equal(false, "N".CoerceType(typeof(bool), true));
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_StringToDecimal_Converts()
        {
#pragma warning disable CS0618
            var result = "3.14".CoerceType(typeof(decimal), 0m);
#pragma warning restore CS0618
            Assert.Equal(3.14m, result);
        }

        [Fact]
        public void CoerceType_StringToLong_Converts()
        {
#pragma warning disable CS0618
            var result = "999999999".CoerceType(typeof(long), 0L);
#pragma warning restore CS0618
            Assert.Equal(999999999L, result);
        }

        [Fact]
        public void CoerceType_StringToDouble_Converts()
        {
#pragma warning disable CS0618
            var result = "2.71".CoerceType(typeof(double), 0.0);
#pragma warning restore CS0618
            Assert.Equal(2.71, result);
        }

        [Fact]
        public void CoerceType_StringToFloat_Converts()
        {
#pragma warning disable CS0618
            var result = "1.5".CoerceType(typeof(float), 0f);
#pragma warning restore CS0618
            Assert.Equal(1.5f, result);
        }

        [Fact]
        public void CoerceType_StringToShort_Converts()
        {
#pragma warning disable CS0618
            var result = "42".CoerceType(typeof(short), (short)0);
#pragma warning restore CS0618
            Assert.Equal((short)42, result);
        }

        [Fact]
        public void CoerceType_StringToByte_Converts()
        {
#pragma warning disable CS0618
            var result = "255".CoerceType(typeof(byte), (byte)0);
#pragma warning restore CS0618
            Assert.Equal((byte)255, result);
        }

        [Fact]
        public void CoerceType_StringToChar_Converts()
        {
#pragma warning disable CS0618
            var result = "A".CoerceType(typeof(char), ' ');
#pragma warning restore CS0618
            Assert.Equal('A', result);
        }

        [Fact]
        public void CoerceType_StringToDateTime_Converts()
        {
#pragma warning disable CS0618
            var result = "2024-01-15".CoerceType(typeof(DateTime), default(DateTime));
#pragma warning restore CS0618
            Assert.IsType<DateTime>(result);
        }

        [Fact]
        public void CoerceType_EnumCaseInsensitive()
        {
#pragma warning disable CS0618
            var result = "monday".CoerceType(typeof(DayOfWeek), DayOfWeek.Sunday);
#pragma warning restore CS0618
            Assert.Equal(DayOfWeek.Monday, result);
        }

        [Fact]
        public void CoerceType_ThreeParamOverload_CheckFuncReturnsFalse()
        {
#pragma warning disable CS0618
            var value = "hello";
            var success = value.CoerceType(
                typeof(int),
                out var returnValue,
                (Type _) => false,
                (Type _, object v) => int.Parse(v.ToString()),
                -1);
            Assert.False(success);
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_ThreeParamOverload_ParseThrowsException()
        {
#pragma warning disable CS0618
            var value = "not_a_number";
            var success = value.CoerceType(
                typeof(int),
                out var returnValue,
                (Type _) => true,
                (Type _, object v) => int.Parse(v.ToString()),
                -1);
            Assert.True(success);
            Assert.IsType<int>(returnValue);
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_ThreeParamOverload_SuccessPath()
        {
#pragma warning disable CS0618
            var value = "42";
            var success = value.CoerceType(
                typeof(int),
                out var returnValue,
                (Type _) => true,
                (Type _, object v) => int.Parse(v.ToString()),
                0);
            Assert.True(success);
            Assert.Equal(42, returnValue);
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceType_DefaultValue_NullForReferenceType()
        {
#pragma warning disable CS0618
            var value = "test";
            var success = value.CoerceType(
                typeof(Uri),
                out var returnValue,
                (Type _) => false,
                (Type _, object v) => new Uri(v.ToString()),
                null);
            Assert.False(success);
            Assert.Null(returnValue);
#pragma warning restore CS0618
        }
    }
}
