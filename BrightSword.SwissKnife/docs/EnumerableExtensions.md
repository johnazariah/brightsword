# EnumerableExtensions

Purpose
- A collection of extension methods for IEnumerable<T> to simplify common operations.

Key APIs
- ForEach<T>(this IEnumerable<T> source, Action<T> action)
- IsNullOrEmpty<T>(this IEnumerable<T> source)
- Partition<T>(this IEnumerable<T> source, int size)

Usage
```csharp
items.ForEach(x => Console.WriteLine(x));
if (list.IsNullOrEmpty()) ...
foreach(var chunk in items.Partition(100)) { ... }
```

Notes
- Methods are implemented with defensive checks for null and aim to be allocation-light.
