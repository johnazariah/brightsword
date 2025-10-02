using System;
using BrightSword.SwissKnife;
using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class CoerceExtensionsTests
    {
        private enum TestEnum { Alpha, Beta }

        [Fact]
    public void CoerceBoolYNWorks()
        {
            Assert.True(((object)"y").CoerceType(typeof(bool), null) is bool b && b);
            Assert.False(((object)"n").CoerceType(typeof(bool), null) is bool b2 && b2);
        }

        [Fact]
    public void CoerceNumberWorks()
        {
            var i = ((object)"123").CoerceType(typeof(int), null);
            Assert.Equal(123, i);
        }

        [Fact]
    public void CoerceEnumWorks()
        {
            var e = ((object)"Beta").CoerceType(typeof(TestEnum), null);
            Assert.Equal(TestEnum.Beta, e);
        }

        [Fact]
    public void CoerceInvalidReturnsOriginal()
        {
            var orig = new object();
            var r = orig.CoerceType(typeof(DateTime), orig);
            Assert.Equal(orig, r);
        }

        [Property]
        public static void CoerceIntRoundtrip(int x)
        {
            var s = x.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var o = ((object)s).CoerceType(typeof(int), null);
            Assert.Equal(x, o);
        }
    }
}
