using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class CoerceExtensionsTests
    {
        private enum TestEnum { Alpha, Beta }

        [Fact]
        public void CoerceBool_YN_Works()
        {
            Assert.True(((object)"y").CoerceType(typeof(bool), null) is bool b && b);
            Assert.False(((object)"n").CoerceType(typeof(bool), null) is bool b2 && b2);
        }

        [Fact]
        public void CoerceNumber_Works()
        {
            var i = ((object)"123").CoerceType(typeof(int), null);
            Assert.Equal(123, i);
        }

        [Fact]
        public void CoerceEnum_Works()
        {
            var e = ((object)"Beta").CoerceType(typeof(TestEnum), null);
            Assert.Equal(TestEnum.Beta, e);
        }

        [Fact]
        public void Coerce_Invalid_ReturnsOriginal()
        {
            var orig = new object();
            var r = orig.CoerceType(typeof(DateTime), orig);
            Assert.Equal(orig, r);
        }
    }
}
