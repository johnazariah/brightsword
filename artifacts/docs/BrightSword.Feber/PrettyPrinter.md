# PrettyPrinter

Purpose
- Pretty-prints public properties of an object to the console using a compiled action built from per-property WriteLine expressions.

Copyable example
```csharp
public class Person { public string Name { get; set; } public int Age { get; set; } }
var p = new Person { Name = "Ada", Age = 30 };
p.Print();
// Console output:
//    Name : Ada
//    Age  : 30
```
