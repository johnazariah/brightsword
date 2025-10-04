# BrightSword.Feber Documentation

**Automated delegate generation using LINQ Expression trees for property-based operations**

## Overview

BrightSword.Feber provides builder classes that **automate the generation of compiled delegates** for property-based operations. It uses LINQ Expression trees to scan object properties and compose efficient, type-safe operations at runtime.

The library is designed to **maximize runtime performance** by building and compiling expression trees once per type, then caching the resulting delegates. This eliminates the overhead of repeated reflection while providing the flexibility of runtime code generation.

## Installation

```bash
dotnet add package BrightSword.Feber
```

## What Problem Does Feber Solve?

When you need to perform repetitive property-based operations (copying, printing, mapping, validating), you typically have two options:

1. **Manual code** - Write explicit code for each property (tedious, error-prone, doesn't scale)
2. **Reflection** - Use reflection on every call (slow, high overhead)

**Feber provides a third option**: Generate the code once using Expression trees, compile it to a delegate, and cache it. You get the flexibility of code generation with performance comparable to hand-written code.

## Key Concepts

### ActionBuilder

Generates `Action<T>` or `Action<TLeft, TRight>` delegates that perform side-effecting operations on object properties.

**Use cases:**
- Property copying/cloning
- Pretty-printing objects
- Property validation
- Property mapping
- Any operation that modifies or inspects properties

### FunctionBuilder

Generates `Func<T, TResult>` delegates that fold/aggregate property values into a result.

**Use cases:**
- Computing hash codes
- Aggregating property values
- Property-based equality checks
- Generating summaries

### OperationBuilder

Base classes that scan properties and compose Expression trees for each property.

## Installation

```bash
dotnet add package BrightSword.Feber
```

## Quick Start

### Example 1: Property Copier

```csharp
using BrightSword.Feber.Core;
using System.Linq.Expressions;
using System.Reflection;

public class Person 
{ 
    public string Name { get; set; } 
    public int Age { get; set; } 
}

// Define a copier builder
public class PersonCopier : ActionBuilder<Person, Person, Person>
{
    protected override Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression target, 
        ParameterExpression source)
    {
        // Generate: target.PropertyName = source.PropertyName
        var sourceProp = Expression.Property(source, propertyInfo);
        var targetProp = Expression.Property(target, propertyInfo);
        return Expression.Assign(targetProp, sourceProp);
    }
}

// Usage
var copier = new PersonCopier();
var action = copier.Action; // Compiled once, cached

var source = new Person { Name = "John", Age = 30 };
var target = new Person();

action(target, source); // Fast execution
// target.Name is now "John", target.Age is now 30
```

### Example 2: Pretty Printer

```csharp
public class PersonPrinter : ActionBuilder<Person, Person>
{
    protected override Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression instanceParameter)
    {
        // Generate: Console.WriteLine($"{propertyName}: {propertyValue}")
        var member = Expression.Property(instanceParameter, propertyInfo);
        var toString = Expression.Call(member, typeof(object).GetMethod("ToString"));
        return Expression.Call(
            typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
            Expression.Call(
                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                Expression.Constant(propertyInfo.Name + ": "),
                toString));
    }
}

// Usage
var printer = new PersonPrinter();
var action = printer.Action; // Compiled once

action(new Person { Name = "Alice", Age = 25 });
// Output:
// Name: Alice
// Age: 25
```

### Example 3: Function Builder (Hash Code)

```csharp
public class HashCodeBuilder<T> : FunctionBuilder<T, int>
{
    protected override Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression instanceParameter, 
        ParameterExpression aggregatorParameter)
    {
        // Generate: aggregator = aggregator * 31 + property.GetHashCode()
        var propertyAccess = Expression.Property(instanceParameter, propertyInfo);
        var getHashCode = Expression.Call(propertyAccess, typeof(object).GetMethod("GetHashCode"));
        var multiply = Expression.Multiply(aggregatorParameter, Expression.Constant(31));
        var add = Expression.Add(multiply, getHashCode);
        return Expression.Assign(aggregatorParameter, add);
    }
    
    protected override Expression SeedExpression => Expression.Constant(17);
}

// Usage
var hasher = new HashCodeBuilder<Person>();
var func = hasher.Function; // Compiled once

var person = new Person { Name = "Bob", Age = 40 };
int hashCode = func(person); // Fast execution
```

## Performance Characteristics

### First Call (Per Type)

When you first access `.Action` or `.Function`:
- **Property scanning** via reflection: ~1-5ms
- **Expression tree building**: ~5-20ms
- **JIT compilation**: ~10-100ms
- **Total**: ~15-125ms (one-time cost)

### Subsequent Calls

After the first call, the delegate is cached:
- **Delegate invocation**: <0.001ms (microseconds)
- **Performance**: Comparable to hand-written code
- **No reflection overhead**

### Memory Usage

- Expression tree: ~1-10KB per builder
- Compiled delegate: ~1KB
- Cache overhead: Minimal

## Best Practices

### ? DO

- **Cache builder instances** - Create once, use many times
- **Warm up at startup** - Call `.Action` or `.Function` during initialization
- **Use Lazy<T>** - For thread-safe lazy compilation
- **Use provided ParameterExpression** - Don't create new ones
- **Test incrementally** - Verify property filtering and expression building separately

### ? DON'T

- **Create builders repeatedly** - Build once, reuse
- **Compile in hot paths** - Pre-compile during initialization
- **Create new ParameterExpression** - Use `InstanceParameterExpression`, etc.
- **Ignore first-call cost** - Measure and warm up if needed

### Production Example

```csharp
public class PersonService
{
    // Thread-safe lazy compilation
    private readonly Lazy<Action<Person, Person>> _copier;
    private readonly Lazy<Action<Person>> _printer;
    private readonly Lazy<Func<Person, int>> _hasher;
    
    public PersonService()
    {
        _copier = new Lazy<Action<Person, Person>>(() => 
            new PersonCopier().Action);
        _printer = new Lazy<Action<Person>>(() => 
            new PersonPrinter().Action);
        _hasher = new Lazy<Func<Person, int>>(() => 
            new HashCodeBuilder<Person>().Function);
    }
    
    public void CopyPerson(Person target, Person source)
    {
        _copier.Value(target, source); // Fast
    }
    
    public void PrintPerson(Person person)
    {
        _printer.Value(person); // Fast
    }
    
    public int GetHashCode(Person person)
    {
        return _hasher.Value(person); // Fast
    }
}
```

## Advanced Usage

### Custom Property Filtering

```csharp
public class FilteredCopier : ActionBuilder<Person, Person, Person>
{
    protected override IEnumerable<PropertyInfo> FilteredProperties =>
        base.FilteredProperties
            .Where(p => p.Name != "Id") // Exclude Id property
            .Where(p => !p.GetCustomAttributes<IgnoreAttribute>().Any());
    
    protected override Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression target, 
        ParameterExpression source)
    {
        var sourceProp = Expression.Property(source, propertyInfo);
        var targetProp = Expression.Property(target, propertyInfo);
        return Expression.Assign(targetProp, sourceProp);
    }
}
```

### Custom Compilation Strategy

```csharp
public class CustomBuilder : ActionBuilder<Person, Person>
{
    protected override Action<Person> BuildAction()
    {
        var param = InstanceParameterExpression;
        var body = Expression.Block(OperationExpressions);
        var lambda = Expression.Lambda<Action<Person>>(body, param);
        
        // Use interpreted mode for lower memory usage
        return lambda.Compile(preferInterpretation: true);
    }
    
    protected override Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression instanceParameter)
    {
        // Your logic here
    }
}
```

## API Reference

### ActionBuilder<TProto, TInstance>

Unary action builder for single-parameter operations.

```csharp
public abstract class ActionBuilder<TProto, TInstance>
{
    /// <summary>
    /// Gets the compiled action delegate (cached).
    /// </summary>
    public virtual Action<TInstance> Action { get; }
    
    /// <summary>
    /// Override to define the expression for each property.
    /// </summary>
    protected abstract Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression instanceParameter);
    
    /// <summary>
    /// Override to customize compilation.
    /// </summary>
    protected virtual Action<TInstance> BuildAction();
}
```

### ActionBuilder<TProto, TLeftInstance, TRightInstance>

Binary action builder for two-parameter operations.

```csharp
public abstract class ActionBuilder<TProto, TLeftInstance, TRightInstance>
{
    /// <summary>
    /// Gets the compiled action delegate (cached).
    /// </summary>
    public virtual Action<TLeftInstance, TRightInstance> Action { get; }
    
    /// <summary>
    /// Override to define the expression for each property pair.
    /// </summary>
    protected abstract Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression leftParameter,
        ParameterExpression rightParameter);
    
    /// <summary>
    /// Override to customize compilation.
    /// </summary>
    protected virtual Action<TLeftInstance, TRightInstance> BuildAction();
}
```

### FunctionBuilder<TProto, TInstance, TResult>

Function builder for aggregating property values.

```csharp
public abstract class FunctionBuilder<TProto, TInstance, TResult>
{
    /// <summary>
    /// Gets the compiled function delegate (cached).
    /// </summary>
    public virtual Func<TInstance, TResult> Function { get; }
    
    /// <summary>
    /// Override to define the expression for each property.
    /// </summary>
    protected abstract Expression PropertyExpression(
        PropertyInfo propertyInfo, 
        ParameterExpression instanceParameter,
        ParameterExpression aggregatorParameter);
    
    /// <summary>
    /// Override to provide the initial seed value.
    /// </summary>
    protected abstract Expression SeedExpression { get; }
    
    /// <summary>
    /// Override to customize compilation.
    /// </summary>
    protected virtual Func<TInstance, TResult> BuildFunction();
}
```

## Troubleshooting

### Issue: Parameter not bound error

```
The parameter 'instance' was not bound in the specified LINQ to Entities query expression.
```

**Solution**: Use the provided parameter expressions.

```csharp
// ? Wrong
var param = Expression.Parameter(typeof(T), "instance");

// ? Correct
var param = InstanceParameterExpression;
```

### Issue: NullReferenceException during execution

**Solution**: Add null checks in your expressions.

```csharp
protected override Expression PropertyExpression(
    PropertyInfo propertyInfo, 
    ParameterExpression instanceParameter)
{
    var propertyAccess = Expression.Property(instanceParameter, propertyInfo);
    
    // Add null check if property type is nullable
    if (propertyInfo.PropertyType.IsClass || 
        Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
    {
        var isNull = Expression.Equal(propertyAccess, Expression.Constant(null));
        var defaultValue = Expression.Default(propertyInfo.PropertyType);
        return Expression.Condition(isNull, defaultValue, propertyAccess);
    }
    
    return propertyAccess;
}
```

## Testing

### Test Property Filtering

```csharp
[Fact]
public void Builder_ShouldFilterCorrectProperties()
{
    // Arrange
    var builder = new PersonCopier();
    
    // Act
    var properties = builder.FilteredProperties.ToList();
    
    // Assert
    Assert.Equal(2, properties.Count);
    Assert.Contains(properties, p => p.Name == "Name");
    Assert.Contains(properties, p => p.Name == "Age");
}
```

### Test Compiled Delegate

```csharp
[Fact]
public void Copier_ShouldCopyAllProperties()
{
    // Arrange
    var copier = new PersonCopier();
    var source = new Person { Name = "John", Age = 30 };
    var target = new Person();
    
    // Act
    copier.Action(target, source);
    
    // Assert
    Assert.Equal(source.Name, target.Name);
    Assert.Equal(source.Age, target.Age);
}
```

## Samples

See the sample application for working examples:
- `BrightSword.Feber.SamplesApp` - Console app with examples
- `BrightSword.Feber.Tests` - Comprehensive test suite

## Dependencies

- **BrightSword.SwissKnife** - Utility functions
- **.NET 10** - Expression trees and compilation

## Documentation

Detailed documentation for each component:
- [ActionBuilder.md](ActionBuilder.md) - Unary and binary action builders
- [FunctionBuilder.md](FunctionBuilder.md) - Function builders for aggregation
- [OperationBuilders.md](OperationBuilders.md) - Base classes and internals
- [DynamicExpressionUtilities.md](DynamicExpressionUtilities.md) - Expression helpers

## Version History

### 2.0.3 (Current)
- Improved expression caching
- Better null handling
- Enhanced documentation
- .NET 10 support

## Contributing

See the [Contributing Guidelines](../../docs/CONTRIBUTING.md).

## License

This project is licensed under the Creative Commons Legal Code (CC0 1.0 Universal). See the [main repository LICENSE](../../LICENSE) for details.

---

**Part of the BrightSword family of libraries**
