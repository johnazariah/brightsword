using System;
using System.Linq;
using BrightSword.SwissKnife;
using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class BitTwiddlerExtensionsTests
    {
        [Fact]
        public void GetReversedBytesUInt64Works()
        {
            ulong value = 0x0102030405060708UL;
            var bytes = value.GetReversedBytes();
            var expected = System.BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void GetReversedBytesInt64Works()
        {
            long value = 0x0102030405060708L;
            var bytes = value.GetReversedBytes();
            var expected = System.BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Property]
        public static void ReverseRoundtrip(int x)
        {
            var bytes = ((long)x).GetReversedBytes();
            Assert.Equal(bytes.Length, System.BitConverter.GetBytes((long)x).Length);
        }
    }
}
