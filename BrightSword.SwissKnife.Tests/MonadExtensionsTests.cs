namespace BrightSword.SwissKnife.Tests
{
    public class MonadExtensionsTests
    {
        [Fact]
        public void MaybeReturnsDefaultForNull()
        {
#pragma warning disable CS0618
            string? s = null;
            Assert.Equal(-1, s.Maybe(str => str!.Length, -1));
#pragma warning restore CS0618
        }

        [Fact]
        public void MaybeActionInvokedWhenNotNull()
        {
            var s = "hi";
            var called = false;
#pragma warning disable CS0618
            _ = s.Maybe(_ => called = true);
#pragma warning restore CS0618
            Assert.True(called);
        }

        [Fact]
        public void WhenActionExecutesOnPredicate()
        {
            var s = "hello";
            var executed = false;
#pragma warning disable CS0618
            _ = s.When(_ => _.Length > 3, _ => executed = true);
#pragma warning restore CS0618
            Assert.True(executed);
        }

        [Fact]
        public void UnlessActionExecutesWhenPredicateFalse()
        {
            var s = "ok";
            var executed = false;
#pragma warning disable CS0618
            _ = s.Unless(_ => _.Length > 3, _ => executed = true);
#pragma warning restore CS0618
            Assert.True(executed);
        }

        [Property]
        public static void MaybeReturnsDefaultForNullProperty(NonNull<string> _)
        {
#pragma warning disable CS0618
            string? s = null;
            Assert.Equal(-1, s.Maybe(str => str!.Length, -1));
#pragma warning restore CS0618
        }
    }
}
