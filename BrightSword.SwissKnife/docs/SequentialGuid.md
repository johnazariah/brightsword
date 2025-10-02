# SequentialGuid

Purpose

Generates sequential GUIDs suitable for database insertion order or other scenarios where GUID monotonicity improves index locality.

Public API

- Guid NewSequentialGuid()

Examples

```csharp
var g = SequentialGuid.NewSequentialGuid();
Console.WriteLine(g);
```

Remarks

Sequential GUIDs embed a timestamp portion to keep insert order roughly increasing; they reduce index fragmentation compared to purely random GUIDs.
