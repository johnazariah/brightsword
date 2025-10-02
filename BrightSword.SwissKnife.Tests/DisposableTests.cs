using System;
using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class DisposableTests
    {
        [Fact]
        public void DisposableInvokesDispose()
        {
            var disposed = false;
            var d = new Disposable<int>(42, _ => disposed = true);
            d.Dispose();
            Assert.True(disposed);
        }

        [Fact]
        public void DisposableInstancePropertyIsSet()
        {
            var d = new Disposable<string>("hello", _ => { });
            Assert.Equal("hello", d.Instance);
        }

        [Property]
        public static void DisposableDisposeInvokes(int x)
        {
            var disposed = false;
            var d = new Disposable<int>(x, _ => disposed = true);
            d.Dispose();
            Assert.True(disposed);
        }
    }
}
