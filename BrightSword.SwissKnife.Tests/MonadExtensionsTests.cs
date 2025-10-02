namespace BrightSword.SwissKnife.Tests
{
    public class MonadExtensionsTests
    {
        [Fact]
        public void MaybeReturnsDefaultForNull()
        {
            string? s = null;
            Assert.Equal(-1, s.Maybe(str => str!.Length, -1));
        }

        [Fact]
        public void MaybeActionInvokedWhenNotNull()
        {
            var s = "hi";
            var called = false;
            _ = s.Maybe(_ => called = true);
            Assert.True(called);
        }

        [Fact]
        public void WhenActionExecutesOnPredicate()
        {
            var s = "hello";
            var executed = false;
            _ = s.When(_ => _.Length > 3, _ => executed = true);
            Assert.True(executed);
        }

        [Fact]
        public void UnlessActionExecutesWhenPredicateFalse()
        {
            var s = "ok";
            var executed = false;
            _ = s.Unless(_ => _.Length > 3, _ => executed = true);
            Assert.True(executed);
        }

        [Property]
        public static void MaybeReturnsDefaultForNullProperty(NonNull<string> _)
        {
            // This property ensures Maybe for null returns default; use a NonNull parameter to satisfy FsCheck's type generation
            string? s = null;
            Assert.Equal(-1, s.Maybe(str => str!.Length, -1));
        }
    }
}
