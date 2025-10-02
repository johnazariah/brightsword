# BitTwiddlerExtensions

Purpose
- Low-level bit manipulation helpers to make common bit-twiddling operations easier and more readable.

Key APIs
- CountBits(uint/ulong) — count set bits.
- ReverseBits(value) — reverse bit order for integer types.
- RotateLeft/RotateRight(value, int)

Usage
```csharp
int setCount = BitTwiddlerExtensions.CountBits(someValue);
var rotated = someValue.RotateLeft(3);
```

Notes
- These methods are designed for performance and are implemented with bitwise arithmetic to avoid allocations.
