using System;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for bit-level manipulation and byte reversal for 64-bit values.
    /// </summary>
    /// <remarks>
    /// These helpers are useful for serialization, endian conversion, and low-level byte operations.
    /// </remarks>
    public static class BitTwiddlerExtensions
    {
        /// <summary>
        /// Returns the bytes of a 64-bit unsigned integer in reversed order (big-endian).
        /// </summary>
        /// <param name="@this">The 64-bit unsigned integer.</param>
        /// <returns>An array of bytes in reversed order.</returns>
        /// <example>
        /// <code>
        /// ulong value = 0x0102030405060708UL;
        /// var reversed = value.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]
        /// </code>
        /// </example>
        [Obsolete("Use BitConverter or BinaryPrimitives.ReverseEndianness in .NET Core 3.0+ for byte reversal.")]
        public static unsafe byte[] GetReversedBytes(this ulong @this) => GetReversedBytesFor64BitValue((byte*)&@this);

        /// <summary>
        /// Returns the bytes of a 64-bit signed integer in reversed order (big-endian).
        /// </summary>
        /// <param name="@this">The 64-bit signed integer.</param>
        /// <returns>An array of bytes in reversed order.</returns>
        /// <example>
        /// <code>
        /// long value = 0x0102030405060708L;
        /// var reversed = value.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]
        /// </code>
        /// </example>
        [Obsolete("Use BitConverter or BinaryPrimitives.ReverseEndianness in .NET Core 3.0+ for byte reversal.")]
        public static unsafe byte[] GetReversedBytes(this long @this) => GetReversedBytesFor64BitValue((byte*)&@this);

        // Private helper for byte reversal
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
    }
}
