using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class FunctionalTests
    {
        [Fact]
        public void Y_Factorial_Works()
        {
            var fact = Functional.Y<int, long>(self => n => n <= 1 ? 1 : n * self(n - 1));
            Assert.Equal(120, fact(5));
        }

        [Fact]
        public void MemoizeFix_Fib_Works()
        {
            var fib = Functional.MemoizeFix<int, int>(self => n => n <= 1 ? n : self(n - 1) + self(n - 2));
            Assert.Equal(55, fib(10));
        }
    }
}
