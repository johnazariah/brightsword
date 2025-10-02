namespace BrightSword.SwissKnife.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void NoneWithNullCollectionThrowsArgumentNullException()
        {
            IEnumerable<int> items = null!;
            Assert.Throws<ArgumentNullException>(() => items.None());
        }

        [Fact]
        public void NoneWithEmptyCollectionReturnsTrue()
        {
            var items = new List<int>();
            Assert.True(items.None());
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
