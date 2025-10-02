using System;
using System.Linq;
using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class ConcurrentDictionaryTests
    {
        [Fact]
        public void IndexerSetGetWorks()
        {
            var cd = new ConcurrentDictionary<string, string, int>();
            cd["a", "b"] = 42;
            Assert.Equal(42, cd["a", "b"]);
        }

        [Property]
        public static void IndexerSetGetRoundtrip(NonNull<string> a, NonNull<string> b, int v)
        {
            var cd = new ConcurrentDictionary<string, string, int>();
            cd[a.Get, b.Get] = v;
            Assert.Equal(v, cd[a.Get, b.Get]);
        }
    }
}
