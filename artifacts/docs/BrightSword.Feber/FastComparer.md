# FastComparer

Purpose
- Demonstrates `FunctionBuilder<TProto,TLeft,TRight,bool>` folding per-property equality checks into a single boolean using `Seed=true` and `Conjunction=Expression.AndAlso`.

Copyable example
```csharp
using System;

public class Person { public string Name { get; set; } public int Age { get; set; } }

class Program
{
    static void Main()
    {
        var a = new Person { Name = "Ada", Age = 30 };
        var b = new Person { Name = "Ada", Age = 30 };
        Console.WriteLine(a.AllPropertiesAreEqualWith(b)); // True
    }
}
```
