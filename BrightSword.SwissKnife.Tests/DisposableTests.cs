namespace BrightSword.SwissKnife.Tests
{
    public class DisposableTests
    {
        [Fact]
        public void DisposableInvokesDispose()
        {
#pragma warning disable CS0618
            var disposed = false;
            var d = new Disposable<int>(42, _ => disposed = true);
            d.Dispose();
#pragma warning restore CS0618
            Assert.True(disposed);
        }

        [Fact]
        public void DisposableInstancePropertyIsSet()
        {
#pragma warning disable CS0618
            var d = new Disposable<string>("hello", _ => { });
#pragma warning restore CS0618
            Assert.Equal("hello", d.Instance);
        }

        [Property]
        public static void DisposableDisposeInvokes(int x)
        {
#pragma warning disable CS0618
            var disposed = false;
            var d = new Disposable<int>(x, _ => disposed = true);
            d.Dispose();
#pragma warning restore CS0618
            Assert.True(disposed);
        }
    }
}
