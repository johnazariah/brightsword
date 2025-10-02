using System.Linq;
using System.Collections.Generic;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class StringExtensionsTests
    {
        private static readonly string[] CamelCaseExampleExpected = new[] { "Camel", "Case", "Example" };
        private static readonly string[] XMLHttpRequestExpected = new[] { "XML", "Http", "Request" };

        public static TheoryData<string, string[]> SplitCamelCaseData() => new TheoryData<string, string[]>
        {
            { "CamelCaseExample", CamelCaseExampleExpected },
            { "XMLHttpRequest", XMLHttpRequestExpected },
            { string.Empty, System.Array.Empty<string>() }
        };

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
            Assert.Equal(new[] { "part", "one", "two" }, actual);
        }
    }
}
