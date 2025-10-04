# ConcurrentDictionary

Purpose
- A small, possibly enhanced implementation or wrapper around System.Collections.Concurrent.ConcurrentDictionary to provide convenience methods used in the repo.

Key APIs (typical)
- GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
- TryGetValue(TKey key, out TValue value)
- AddOrUpdate(TKey key, TValue addValue, Func<TKey,TValue,TValue> updateFactory)

Usage
```csharp
var cache = new ConcurrentDictionary<string, MyType>();
var value = cache.GetOrAdd("key", k => new MyType());
```

Notes
- This file may include helpers for efficient double-checked initialization patterns used by the expression builders.
