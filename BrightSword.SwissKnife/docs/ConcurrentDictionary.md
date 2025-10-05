# ConcurrentDictionary

A small helper wrapper implemented in `ConcurrentDictionary.cs`.

## Purpose

Provides a nested concurrent dictionary type `ConcurrentDictionary<TKey1, TKey2, TValue>` that inherits from `ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>` and adds a convenient indexer `this[key1, key2]` for two-key access.

## API

- `TValue this[TKey1 key1, TKey2 key2] { get; set; }` — Indexer that provides get/set semantics. Setting a value ensures the inner dictionary is created via `GetOrAdd`.

## Example

```csharp
var map = new ConcurrentDictionary<string, string, int>();
map["user", "score"] = 100;
var s = map["user", "score"]; // 100
```

## Remarks

This type is a small convenience; it allows using a nested dictionary without repeatedly creating inner dictionaries. It is not a full substitute for a dedicated two-key map with custom semantics.
