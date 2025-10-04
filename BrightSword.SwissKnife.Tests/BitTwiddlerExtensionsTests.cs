namespace BrightSword.SwissKnife.Tests
{
    public class BitTwiddlerExtensionsTests
    {
        [Fact]
        public void GetReversedBytesUInt64Works()
        {
#pragma warning disable CS0618
            var value = 0x0102030405060708UL;
            var bytes = value.GetReversedBytes();
#pragma warning restore CS0618
            var expected = BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Fact]
        public void GetReversedBytesInt64Works()
        {
#pragma warning disable CS0618
            var value = 0x0102030405060708L;
            var bytes = value.GetReversedBytes();
#pragma warning restore CS0618
            var expected = BitConverter.GetBytes(value).Reverse().ToArray();
            Assert.Equal(expected, bytes);
        }

        [Property]
        public static void ReverseRoundtrip(int x)
        {
#pragma warning disable CS0618
            var bytes = ((long)x).GetReversedBytes();
#pragma warning restore CS0618
            Assert.Equal(bytes.Length, BitConverter.GetBytes((long)x).Length);
        }
    }
}
