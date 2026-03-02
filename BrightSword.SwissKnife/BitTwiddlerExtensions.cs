namespace BrightSword.SwissKnife;

public static class BitTwiddlerExtensions
{
	private unsafe static byte[] GetReversedBytesFor64BitValue(byte* rgb)
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

	public unsafe static byte[] GetReversedBytes(this ulong _this)
	{
		return GetReversedBytesFor64BitValue((byte*)(&_this));
	}

	public unsafe static byte[] GetReversedBytes(this long _this)
	{
		return GetReversedBytesFor64BitValue((byte*)(&_this));
	}
}
