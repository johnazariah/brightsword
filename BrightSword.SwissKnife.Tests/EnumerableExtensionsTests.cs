namespace BrightSword.SwissKnife.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void NoneWithNullCollectionThrowsArgumentNullException()
        {
#pragma warning disable CS0618
            IEnumerable<int> items = null!;
            _ = Assert.Throws<ArgumentNullException>(() => items.None());
#pragma warning restore CS0618
        }

        [Fact]
        public void NoneWithEmptyCollectionReturnsTrue()
        {
#pragma warning disable CS0618
            var items = new List<int>();
            Assert.True(items.None());
#pragma warning restore CS0618
        }

        [Fact]
        public void AllUniqueReturnsTrueForUnique()
        {
            var items = new List<int> { 1, 2, 3 };
            Assert.True(items.AllUnique());
        }

        [Fact]
        public void AllUniqueReturnsFalseForDuplicate()
        {
            var items = new List<int> { 1, 2, 2 };
            Assert.False(items.AllUnique());
        }

        [Fact]
        public void LastButOneReturnsCorrectValue()
        {
            var items = new List<int> { 1, 2, 3 };
            Assert.Equal(2, items.LastButOne());
        }

        [Property]
        public static void EnumerableAllUniqueRoundtrip(int[] items)
        {
            var expected = items.Distinct().Count() == items.Length;
            Assert.Equal(expected, items.AllUnique());
        }
    }
}
