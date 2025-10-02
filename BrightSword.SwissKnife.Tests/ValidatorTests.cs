using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public void CheckBool_ThrowsOnFalse()
        {
            Assert.Throws<Exception>(() => false.Check("bad"));
        }

        [Fact]
        public void CheckBool_TException_Works()
        {
            Assert.Throws<InvalidOperationException>(() => false.Check<InvalidOperationException>("bad"));
        }

        [Fact]
        public void CheckFunc_ThrowsOnFalse()
        {
            Assert.Throws<Exception>(() => new Func<bool>(() => false).Check("bad"));
        }

        [Fact]
        public void CheckFunc_TException_Works()
        {
            Assert.Throws<InvalidOperationException>(() => new Func<bool>(() => false).Check<InvalidOperationException>("bad"));
        }
    }
}
