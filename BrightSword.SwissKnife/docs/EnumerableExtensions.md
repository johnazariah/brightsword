# EnumerableExtensions

## Purpose
Provides extension methods for <code>IEnumerable<T></code> and <code>IList<T></code> for common collection operations. These helpers simplify checks for uniqueness, emptiness, and access to elements.

## When to Use
- When you need to check if a collection is empty or contains no elements matching a filter.
- When you want to check for uniqueness in a collection or get the second-to-last element.

## How to Use
Use these methods to perform null/empty checks, uniqueness checks, and access elements safely.

## Key APIs
- <code>None<T>(this IEnumerable<T> @this, Func<T, bool> filter = null)</code>: Returns true if the sequence contains no elements, or no elements matching the filter.
- <code>AllUnique<T>(this IEnumerable<T> @this)</code>: Returns true if all elements in the sequence are unique.
- <code>AllUniqueSorted<T>(this IList<T> @this)</code>: Returns true if all elements in the sorted list are unique (no adjacent duplicates).
- <code>LastButOne<T>(this IEnumerable<T> @this)</code>: Returns the second-to-last element in the sequence, or default if not available.

## Examples
```csharp
var empty = new int[0].None(); // true
var noneEven = new[] { 1, 3, 5 }.None(x => x % 2 == 0); // true
var unique = new[] { 1, 2, 3 }.AllUnique(); // true
var notUnique = new[] { 1, 2, 2 }.AllUnique(); // false
var sortedUnique = new List<int> { 1, 2, 3 }.AllUniqueSorted(); // true
var sortedNotUnique = new List<int> { 1, 2, 2, 3 }.AllUniqueSorted(); // false
var arr = new[] { 1, 2, 3 };
var lastButOne = arr.LastButOne(); // 2
var single = new[] { 42 }.LastButOne(); // 0 (default)
```

## Remarks
These helpers perform null checks and are intended for readability. They simplify common collection operations and improve code clarity.
