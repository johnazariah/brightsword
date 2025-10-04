using System;
using System.Buffers.Binary;

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
        /// <param name="this">The 64-bit unsigned integer.</param>
        /// <returns>An array of bytes in reversed order.</returns>
        /// <example>
        /// <code>
        /// ulong value = 0x0102030405060708UL;
        /// var reversed = value.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]
        /// </code>
        /// </example>
        [Obsolete("Use BitConverter or BinaryPrimitives.ReverseEndianness in .NET Core 3.0+ for byte reversal.")]
        public static byte[] GetReversedBytes(this ulong @this)
        {
            // Reverse the value at the integer level and then serialize — this avoids allocations/copies and is JIT-friendly.
            var reversed = BinaryPrimitives.ReverseEndianness(@this);
            return BitConverter.GetBytes(reversed);
        }

        /// <summary>
        /// Returns the bytes of a 64-bit signed integer in reversed order (big-endian).
        /// </summary>
        /// <param name="this">The 64-bit signed integer.</param>
        /// <returns>An array of bytes in reversed order.</returns>
        /// <example>
        /// <code>
        /// long value = 0x0102030405060708L;
        /// var reversed = value.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]
        /// </code>
        /// </example>
        [Obsolete("Use BitConverter or BinaryPrimitives.ReverseEndianness in .NET Core 3.0+ for byte reversal.")]
        public static byte[] GetReversedBytes(this long @this)
        {
            var reversed = BinaryPrimitives.ReverseEndianness(@this);
            return BitConverter.GetBytes(reversed);
        }

        /// <summary>
        /// Writes the reversed bytes for a 64-bit unsigned integer into the provided span.
        /// This method avoids allocating a new byte[] and is intended for hot paths.
        /// </summary>
        /// <param name="this">The 64-bit unsigned integer.</param>
        /// <param name="destination">A span with at least 8 bytes of space.</param>
        /// <exception cref="ArgumentException">If <paramref name="destination"/> is too small.</exception>
        public static void WriteReversedBytes(this ulong @this, Span<byte> destination)
        {
            if (destination.Length < 8)
            {
                throw new ArgumentException("Destination must be at least 8 bytes", nameof(destination));
            }

            var reversed = BinaryPrimitives.ReverseEndianness(@this);
            BinaryPrimitives.WriteUInt64LittleEndian(destination, reversed);
        }

        /// <summary>
        /// Writes the reversed bytes for a 64-bit signed integer into the provided span.
        /// This method avoids allocating a new byte[] and is intended for hot paths.
        /// </summary>
        /// <param name="this">The 64-bit signed integer.</param>
        /// <param name="destination">A span with at least 8 bytes of space.</param>
        /// <exception cref="ArgumentException">If <paramref name="destination"/> is too small.</exception>
        public static void WriteReversedBytes(this long @this, Span<byte> destination)
        {
            if (destination.Length < 8)
            {
                throw new ArgumentException("Destination must be at least 8 bytes", nameof(destination));
            }

            var reversed = BinaryPrimitives.ReverseEndianness(@this);
            BinaryPrimitives.WriteInt64LittleEndian(destination, reversed);
        }

        // kept for historical reference; new implementations use BitConverter and Array.Reverse
    }
}
