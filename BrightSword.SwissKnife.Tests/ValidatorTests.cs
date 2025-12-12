namespace BrightSword.SwissKnife.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public void CheckBoolThrowsOnFalse() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => false.Check("bad"));
#pragma warning restore CS0618

        [Fact]
        public void CheckBoolDoesNotThrowOnTrue() =>
#pragma warning disable CS0618
            true.Check("good");
#pragma warning restore CS0618

        [Fact]
        public void CheckBoolTExceptionWorks() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => false.Check<InvalidOperationException>("bad"));
#pragma warning restore CS0618

        [Fact]
        public void CheckFuncThrowsOnFalse() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => new Func<bool>(() => false).Check("bad"));
#pragma warning restore CS0618

        [Fact]
        public void CheckFuncDoesNotThrowOnTrue() =>
#pragma warning disable CS0618
            new Func<bool>(() => true).Check("good");
#pragma warning restore CS0618

        [Fact]
        public void CheckFuncTExceptionWorks() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => new Func<bool>(() => false).Check<InvalidOperationException>("bad"));
#pragma warning restore CS0618

        [Fact]
        public void CheckFuncThrowsArgumentNullExceptionOnNullPredicate() =>
#pragma warning disable CS0618
            Assert.Throws<ArgumentNullException>(() => ((Func<bool>)null).Check("bad"));
#pragma warning restore CS0618

        [Fact]
        public void CheckBoolTExceptionFallbacksToDefaultConstructor() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => false.Check<InvalidOperationException>(null));
#pragma warning restore CS0618

        [Fact]
        public void CheckFuncTExceptionFallbacksToDefaultConstructor() =>
#pragma warning disable CS0618
            Assert.Throws<InvalidOperationException>(() => new Func<bool>(() => false).Check<InvalidOperationException>(null));
#pragma warning restore CS0618
    }
}
