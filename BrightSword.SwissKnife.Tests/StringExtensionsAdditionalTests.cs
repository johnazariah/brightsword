namespace BrightSword.SwissKnife.Tests
{
    public class StringExtensionsAdditionalTests
    {
        [Fact]
        public void SplitCamelCase_LeadingSpaces_SkipsWhitespace()
        {
            var parts = "  HelloWorld".SplitCamelCase().ToList();
            Assert.Equal(new[] { "Hello", "World" }, parts);
        }

        [Fact]
        public void SplitCamelCase_TrailingSpaces_SkipsWhitespace()
        {
            var parts = "HelloWorld  ".SplitCamelCase().ToList();
            Assert.Equal(new[] { "Hello", "World" }, parts);
        }

        [Fact]
        public void SplitCamelCase_SingleCharacter()
        {
            var parts = "A".SplitCamelCase().ToList();
            Assert.Single(parts);
            Assert.Equal("A", parts[0]);
        }

        [Fact]
        public void SplitCamelCase_AllUppercase_ReturnsSingleSegment()
        {
            var parts = "ABC".SplitCamelCase().ToList();
            Assert.Single(parts);
            Assert.Equal("ABC", parts[0]);
        }

        [Fact]
        public void SplitCamelCase_ConsecutiveSpaces_SkippedAsWhitespace()
        {
            var parts = "Hello   World".SplitCamelCase().ToList();
            Assert.Equal(new[] { "Hello", "World" }, parts);
        }

        [Fact]
        public void SplitCamelCaseAndUnderscore_MixedPunctuation()
        {
            var parts = "Hello_World_Test".SplitCamelCaseAndUnderscore().ToList();
            Assert.Equal(new[] { "Hello", "World", "Test" }, parts);
        }

        [Fact]
        public void SplitCamelCaseAndUnderscore_ConsecutiveUnderscores()
        {
            var parts = "Hello__World".SplitCamelCaseAndUnderscore().ToList();
            Assert.Equal(new[] { "Hello", "World" }, parts);
        }

        [Fact]
        public void SplitDotted_SingleSegment_NoDots()
        {
            var parts = "Hello".SplitDotted().ToList();
            Assert.Single(parts);
            Assert.Equal("Hello", parts[0]);
        }

        [Fact]
        public void SplitDotted_TrailingDot()
        {
            var parts = "Hello.World.".SplitDotted().ToList();
            Assert.Equal(new[] { "Hello", "World" }, parts);
        }

        [Fact]
        public void SplitIntoSegments_AllOptionsDisabled_ReturnsFull()
        {
            var parts = "Hello World_Test.Foo".SplitIntoSegments(false, false, false).ToList();
            Assert.Single(parts);
            Assert.Equal("Hello World_Test.Foo", parts[0]);
        }

        [Fact]
        public void SplitCamelCase_LowerThenUpperThenLower()
        {
            var parts = "getHTTPResponse".SplitCamelCase().ToList();
            Assert.Equal(new[] { "get", "HTTP", "Response" }, parts);
        }

        [Fact]
        public void SplitIntoSegments_SingleLowerChar()
        {
            var parts = "a".SplitIntoSegments(true, true, true).ToList();
            Assert.Single(parts);
            Assert.Equal("a", parts[0]);
        }

        [Fact]
        public void SplitCamelCase_SingleWord_ReturnsSingleElement()
        {
            var parts = "hello".SplitCamelCase().ToList();
            Assert.Single(parts);
            Assert.Equal("hello", parts[0]);
        }

        [Fact]
        public void SplitCamelCase_WithAcronyms_SplitsCorrectly()
        {
            var parts = "HTMLParser".SplitCamelCase().ToList();
            Assert.Equal(new[] { "HTML", "Parser" }, parts);
        }

        [Fact]
        public void SplitIntoSegments_SplitBySpace()
        {
            var parts = "hello world".SplitIntoSegments(splitBySpace: true, splitOnCamelCase: false, splitOnPunctuation: false).ToList();
            Assert.Equal(new[] { "hello", "world" }, parts);
        }

        [Fact]
        public void SplitIntoSegments_NoSplitting_ReturnsWhole()
        {
            var parts = "hello".SplitIntoSegments(false, false, false).ToList();
            Assert.Single(parts);
            Assert.Equal("hello", parts[0]);
        }
    }
}
