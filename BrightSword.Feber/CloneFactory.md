# CloneFactory

Purpose
- Demonstrates `ActionBuilder<TProto,TDestination,TSource>` to copy matching public properties from a source object to a destination object.

Copyable example
```csharp
using System;

public class PersonProto { public string Name { get; set; } public int Age { get; set; } }

class Program
{
    static void Main()
    {
        var src = new PersonProto { Name = "Ada", Age = 30 };
        var dst = src.Clone<PersonProto, PersonProto>();
        Console.WriteLine($"Name={dst.Name}, Age={dst.Age}");
    }
}
```

Notes
- The inner `CloneFactoryBuilder` matches properties by name and type and generates assignment expressions for matched properties.
