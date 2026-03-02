# SimpleSerializer

Purpose
- Produces a simple comma-delimited key:value string from public properties using a `FunctionBuilder` that concatenates fragments.

Copyable example
```csharp
public class Person { public string Name { get; set; } public int Age { get; set; } }
var p = new Person { Name = "Ada", Age = 30 };
var s = p.Serialize(); // {Name:Ada,Age:30,}
```

Notes
- Uses a DateTime-specific formatting branch to emit ISO-8601 (Round-trip) format for DateTime properties.
