# ObjectDescriber

Purpose

Produces human-readable descriptions of objects (for logging, diagnostics, tests), including nested objects and collections.

Public API

- string Describe(object o, int maxDepth = 3)
- IDictionary<string, object?> DescribeProperties(object o)

Examples

```csharp
var desc = ObjectDescriber.Describe(myObj, maxDepth: 2);
Console.WriteLine(desc);

var props = ObjectDescriber.DescribeProperties(myObj);
```

Remarks

Useful for debugging and unit-test failure messages. The output is formatted for readability and tries to avoid circular references by respecting `maxDepth`.
