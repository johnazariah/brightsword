using System.Collections.Concurrent;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class ConcurrentDictionaryTests
    {
        [Fact]
        public void Indexer_SetGet_Works()
        {
            var cd = new ConcurrentDictionary<string, string, int>();
            cd["a", "b"] = 42;
            Assert.Equal(42, cd["a", "b"]);
        }
    }
}
