using System;
using System.Linq;
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
            var value = 0x0102030405060708UL;
            var bytes = value.GetReversedBytes();
            var expected = BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void GetReversedBytesInt64Works()
        {
            var value = 0x0102030405060708L;
            var bytes = value.GetReversedBytes();
            var expected = BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Property]
        public static void ReverseRoundtrip(int x)
        {
            var bytes = ((long)x).GetReversedBytes();
            Assert.Equal(bytes.Length, BitConverter.GetBytes((long)x).Length);
        }
    }
}
