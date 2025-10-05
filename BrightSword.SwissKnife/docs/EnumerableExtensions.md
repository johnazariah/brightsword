# EnumerableExtensions

## Status

Note: The project currently does not include a dedicated `EnumerableExtensions.cs` source file. This document is a placeholder that describes expected enumerable helpers; update or remove when the implementation changes.

## Typical Helpers (suggested)

If you add enumerable helpers, consider providing these APIs:

- `bool None<T>(this IEnumerable<T> @this, Func<T, bool> filter = null)` — Returns true when sequence contains no elements or no elements matching `filter`.
- `bool AllUnique<T>(this IEnumerable<T> @this)` — Returns true if all elements are unique.
- `bool AllUniqueSorted<T>(this IList<T> @this)` — Returns true if sorted list contains no adjacent duplicates.
- `T LastButOne<T>(this IEnumerable<T> @this)` — Returns the second-to-last element or default.

## Recommendations

- If you implement these helpers, include unit tests and document behavior for `null` sequences and empty sequences.
