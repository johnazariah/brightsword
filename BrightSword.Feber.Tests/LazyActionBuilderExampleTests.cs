using System;
using Xunit;
using BrightSword.Feber.Samples;

namespace BrightSword.Feber.Tests
{
    public class LazyActionBuilderExampleTests
    {
        [Fact]
        public void ActionIsCompiledAndInvokable()
        {
            var builder = new LazyActionBuilderExample<object, object>();

            var action = builder.Action;

            Assert.NotNull(action);

            // Should not throw
            action(new object());
        }
    }
}
