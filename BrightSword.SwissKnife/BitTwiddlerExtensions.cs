namespace BrightSword.SwissKnife
{
    public static class BitTwiddlerExtensions
    {
        private static unsafe byte[] GetReversedBytesFor64BitValue(byte* rgb)
        {
            return
            [
          rgb[7],
          rgb[6],
          rgb[5],
          rgb[4],
          rgb[3],
          rgb[2],
          rgb[1],
          *rgb
            ];
        }

        public static unsafe byte[] GetReversedBytes(this ulong This) => GetReversedBytesFor64BitValue((byte*)&This);

        public static unsafe byte[] GetReversedBytes(this long This) => GetReversedBytesFor64BitValue((byte*)&This);
    }
}
