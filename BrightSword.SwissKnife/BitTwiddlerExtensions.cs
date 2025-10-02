// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.BitTwiddlerExtensions
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

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
