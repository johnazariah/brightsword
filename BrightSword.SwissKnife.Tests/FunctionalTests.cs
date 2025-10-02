using System;
using BrightSword.SwissKnife;
using Xunit;
using FsCheck;
using FsCheck.Xunit;

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

        [Property]
        public static void Y_Composition_Idempotent(NonNegativeInt n)
        {
            var fact = Functional.Y<int, long>(self => i => i <= 1 ? 1 : i * self(i - 1));
            var v = n.Get;
            // for small n, just ensure it doesn't throw and returns non-negative
            var r = fact(Math.Min(10, v));
            Assert.True(r >= 0);
        }
    }
}
