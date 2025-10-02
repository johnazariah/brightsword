#nullable disable
namespace BrightSword.SwissKnife;

public static class BitTwiddlerExtensions
{
  private static unsafe byte[] GetReversedBytesFor64BitValue(byte* rgb)
  {
    return new byte[8]
    {
      rgb[7],
      rgb[6],
      rgb[5],
      rgb[4],
      rgb[3],
      rgb[2],
      rgb[1],
      *rgb
    };
  }

  public static unsafe byte[] GetReversedBytes(this ulong _this)
  {
    return BitTwiddlerExtensions.GetReversedBytesFor64BitValue((byte*) &_this);
  }

  public static unsafe byte[] GetReversedBytes(this long _this)
  {
    return BitTwiddlerExtensions.GetReversedBytesFor64BitValue((byte*) &_this);
  }
}
