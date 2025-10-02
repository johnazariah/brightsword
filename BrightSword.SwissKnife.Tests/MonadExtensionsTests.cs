using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class MonadExtensionsTests
    {
        [Fact]
        public void Maybe_ReturnsDefaultForNull()
        {
            string s = null;
            Assert.Equal(-1, s.Maybe(str => str.Length, -1));
        }

        [Fact]
        public void Maybe_Action_InvokedWhenNotNull()
        {
            string s = "hi";
            var called = false;
            s.Maybe(_ => called = true);
            Assert.True(called);
        }

        [Fact]
        public void When_Action_ExecutesOnPredicate()
        {
            string s = "hello";
            var executed = false;
            s.When(_ => _.Length > 3, _ => executed = true);
            Assert.True(executed);
        }

        [Fact]
        public void Unless_Action_ExecutesWhenPredicateFalse()
        {
            string s = "ok";
            var executed = false;
            s.Unless(_ => _.Length > 3, _ => executed = true);
            Assert.True(executed);
        }
    }
}
