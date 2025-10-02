using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class SequentialGuidTests
    {
        [Fact]
        public void NewSequentialGuidReturnsGuid()
        {
            var g = SequentialGuid.NewSequentialGuid();
            Assert.IsType<Guid>(g);
        }

        [Fact]
        public void NewReverseSequentialGuidReturnsGuid()
        {
            var g = SequentialGuid.NewReverseSequentialGuid();
            Assert.IsType<Guid>(g);
        }

        [Fact]
        public void InitializeSetsValues()
        {
            SequentialGuid.Initialize(1, 2, 3);
            var g1 = SequentialGuid.NewSequentialGuid();
            var g2 = SequentialGuid.NewSequentialGuid();
            Assert.NotEqual(g1, g2);
        }
    }
}
