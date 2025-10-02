# BitTwiddlerExtensions

## Purpose
Provides extension methods for bit-level manipulation and byte reversal for 64-bit values. These helpers are useful for serialization, endian conversion, and low-level byte operations.

## When to Use
- When you need to reverse the byte order of 64-bit integers for serialization or endian conversion.
- When you want to perform bitwise operations efficiently and with clear intent.

## How to Use
Use these methods to get reversed bytes for <code>ulong</code> or <code>long</code> values.

## Key APIs
- <code>GetReversedBytes(this ulong @this)</code>: Returns the bytes of a 64-bit unsigned integer in reversed order (big-endian).
- <code>GetReversedBytes(this long @this)</code>: Returns the bytes of a 64-bit signed integer in reversed order (big-endian).

## Examples
```csharp
ulong value = 0x0102030405060708UL;
var reversed = value.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]

long value2 = 0x0102030405060708L;
var reversed2 = value2.GetReversedBytes(); // [0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01]
```

## Remarks
These methods are designed for performance and are implemented with bitwise arithmetic to avoid allocations. They are useful in serialization, cryptography, and low-level data manipulation.
