namespace BrightSword.SwissKnife.Tests
{
    public class ValidatorTests
    {
        [Fact]
        public void CheckBoolThrowsOnFalse() => Assert.Throws<Exception>(() => false.Check("bad"));

        [Fact]
        public void CheckBoolTExceptionWorks() => Assert.Throws<InvalidOperationException>(() => false.Check<InvalidOperationException>("bad"));

        [Fact]
        public void CheckFuncThrowsOnFalse() => Assert.Throws<Exception>(() => new Func<bool>(() => false).Check("bad"));

        [Fact]
        public void CheckFuncTExceptionWorks() => Assert.Throws<InvalidOperationException>(() => new Func<bool>(() => false).Check<InvalidOperationException>("bad"));
    }
}
