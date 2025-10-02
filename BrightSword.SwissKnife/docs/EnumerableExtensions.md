# EnumerableExtensions

Purpose

Collection helper extensions to simplify common iteration and chunking patterns.

Public API (signatures)

- void ForEach<T>(this IEnumerable<T> source, Action<T> action)
- bool IsNullOrEmpty<T>(this IEnumerable<T> source)
- IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)

Examples

```csharp
// ForEach
items.ForEach(x => Console.WriteLine(x));

// Null/empty check
if (list.IsNullOrEmpty()) return;

// Partition into chunks of 100
foreach (var chunk in items.Partition(100)) {
	ProcessChunk(chunk);
}
```

Remarks

These helpers perform null checks and are intended for readability. `Partition` yields lazy subsequences and avoids buffering the entire collection where possible.
