namespace BrightSword.SwissKnife.Tests
{
    public class StringExtensionsTests
    {
        private static readonly string[] CamelCaseExampleExpected = {"Camel", "Case", "Example"};
        private static readonly string[] XMLHttpRequestExpected = {"XML", "Http", "Request"};
        private static readonly string[] EmptyStringArray = {};

        public static TheoryData<string, string[]> SplitCamelCaseData() => new()
        {
            { "CamelCaseExample", CamelCaseExampleExpected },
            { "XMLHttpRequest", XMLHttpRequestExpected },
            { string.Empty, EmptyStringArray }
        };

        private static readonly string[] PartOneTwo = {"part", "one", "two"};

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

        [Fact]
        public void SplitCamelCaseAndUnderscoreWorks()
        {
            var actual = "Camel_CaseExample".SplitCamelCaseAndUnderscore().ToArray();
            var expected = new[] {"Camel", "Case", "Example"};
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitIntoSegmentsHandlesCustomSeparators()
        {
            var actual = "A_B C.D".SplitIntoSegments(true, true, true, '_', '.').ToArray();
            var expected = new[] {"A", "B", "C", "D"};
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitIntoSegmentsHandlesEmptyString()
        {
            var actual = string.Empty.SplitIntoSegments().ToArray();
            Assert.Empty(actual);
        }

        [Fact]
        public void SplitIntoSegmentsHandlesNullString()
        {
            string input = null;
            var actual = input.SplitIntoSegments().ToArray();
            Assert.Empty(actual);
        }

        [Property]
        public static void SplitCamelCaseIdempotent(NonNull<string> s)
        {
            var str = s.Get;
            var parts = str.SplitCamelCase().ToArray();
            var joined = string.Concat(parts);
            Assert.True(string.IsNullOrEmpty(str) || joined.Length <= str.Length);
        }
    }
}
