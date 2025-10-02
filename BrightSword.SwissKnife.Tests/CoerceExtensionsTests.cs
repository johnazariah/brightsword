using System.Globalization;

namespace BrightSword.SwissKnife.Tests
{
    public class CoerceExtensionsTests
    {
        private enum TestEnum { Alpha, Beta }

        [Fact]
        public void CoerceBoolYNWorks()
        {
#pragma warning disable CS0618
            Assert.True("y".CoerceType(typeof(bool), null) is bool b && b);
            Assert.False("n".CoerceType(typeof(bool), null) is bool b2 && b2);
#pragma warning restore CS0618
        }

        [Fact]
        public void CoerceNumberWorks()
        {
#pragma warning disable CS0618
            var i = "123".CoerceType(typeof(int), null);
#pragma warning restore CS0618
            Assert.Equal(123, i);
        }

        [Fact]
        public void CoerceEnumWorks()
        {
#pragma warning disable CS0618
            var e = "Beta".CoerceType(typeof(TestEnum), null);
#pragma warning restore CS0618
            Assert.Equal(TestEnum.Beta, e);
        }

        [Fact]
        public void CoerceInvalidReturnsOriginal()
        {
#pragma warning disable CS0618
            var orig = new object();
            var r = orig.CoerceType(typeof(DateTime), orig);
#pragma warning restore CS0618
            Assert.Equal(orig, r);
        }

        [Property]
        public static void CoerceIntRoundtrip(int x)
        {
            var s = x.ToString(CultureInfo.InvariantCulture);
#pragma warning disable CS0618
            var o = s.CoerceType(typeof(int), null);
#pragma warning restore CS0618
            Assert.Equal(x, o);
        }
    }
}
