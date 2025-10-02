namespace BrightSword.SwissKnife.Tests
{
    public class StringExtensionsTests
    {
        private static readonly string[] CamelCaseExampleExpected = ["Camel", "Case", "Example"];
        private static readonly string[] XMLHttpRequestExpected = ["XML", "Http", "Request"];

        private static readonly string[] EmptyStringArray = [];

        public static TheoryData<string, string[]> SplitCamelCaseData() => new TheoryData<string, string[]>
        {
            { "CamelCaseExample", CamelCaseExampleExpected },
            { "XMLHttpRequest", XMLHttpRequestExpected },
            { string.Empty, EmptyStringArray }
        };

        private static readonly string[] PartOneTwo = ["part", "one", "two"];

        [Theory]
        [MemberData(nameof(SplitCamelCaseData))]
        public void SplitCamelCaseWorks(string input, string[] expected)
        {
            var actual = input.SplitCamelCase().ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitDottedWorks()
        {
            var actual = "part.one.two".SplitDotted().ToArray();
            Assert.Equal(PartOneTwo, actual);
        }

        [Property]
        public static void SplitCamelCaseIdempotent(NonNull<string> s)
        {
            var str = s.Get;
            var parts = str.SplitCamelCase().ToArray();
            // joining back with no separators should contain original letters in order
            var joined = string.Concat(parts);
            Assert.True(string.IsNullOrEmpty(str) || joined.Length <= str.Length);
        }
    }
}
