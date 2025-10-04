# BrightSword.SwissKnife Documentation

A comprehensive collection of utility classes and extension methods for .NET development.

## Overview

BrightSword.SwissKnife provides a "Swiss Army knife" of utilities for common .NET development tasks. It includes extension methods, helper classes, and utility functions designed to make your code more concise and readable.

## Installation

```bash
dotnet add package BrightSword.SwissKnife
```

## Features

### Extension Methods

All classes are in the `BrightSword.SwissKnife` namespace:

- **TypeExtensions** - Type inspection and reflection helpers
- **StringExtensions** - String manipulation utilities
- **EnumerableExtensions** - Collection operations
- **AttributeExtensions** - Attribute handling
- **CoerceExtensions** - Type coercion helpers
- **MonadExtensions** - Monadic operations
- **BitTwiddlerExtensions** - Bit manipulation utilities

### Utility Classes

Helper classes in the `BrightSword.SwissKnife` namespace:

- **Validator** - Argument validation and guard helpers
- **Disposable** - Disposable pattern helpers
- **Functional** - Functional programming utilities
- **ObjectDescriber** - Object description and printing
- **SequentialGuid** - Sequential GUID generation
- **ConcurrentDictionary** - Thread-safe dictionary extensions

## Usage Examples

### Type Extensions

```csharp
using BrightSword.SwissKnife;

// Get printable type name
var name = typeof(List<int>).PrintableName(); // "List<Int32>"

// Get all properties including inherited
var properties = typeof(MyClass).GetAllProperties();

// Get all methods including inherited
var methods = typeof(IMyInterface).GetAllMethods();

// Convert interface name to concrete type name
var concreteName = typeof(IMyInterface).RenameToConcreteType(); // "MyInterface"
```

### String Extensions

```csharp
using BrightSword.SwissKnife;

// String utilities (check actual implementation for available methods)
string text = "  hello world  ";
var trimmed = text.Trim();
```

### Collection Extensions

```csharp
using BrightSword.SwissKnife;

var numbers = new[] { 1, 2, 3, 4, 5 };

// Use enumerable extensions
// (Check EnumerableExtensions.cs for actual available methods)
```

### Validator

```csharp
using BrightSword.SwissKnife;

// Note: Validator methods are marked as Obsolete
// Modern C# provides better built-in patterns

// Use ArgumentNullException.ThrowIfNull instead
ArgumentNullException.ThrowIfNull(myObject, nameof(myObject));

// Use ArgumentException for conditions
if (value <= 0)
{
    throw new ArgumentException("Value must be positive", nameof(value));
}
```

### Object Describer

```csharp
using BrightSword.SwissKnife;

// Describe objects for debugging/logging
var obj = new MyClass { Name = "Test", Value = 42 };
var description = ObjectDescriber.Describe(obj);
```

### Monads

```csharp
using BrightSword.SwissKnife;

// Monadic extensions for functional-style programming
// (Check MonadExtensions.cs for actual available methods)
```

## API Documentation

### Single Namespace

**All classes are in the `BrightSword.SwissKnife` namespace.**

There are no sub-namespaces. Simply use:

```csharp
using BrightSword.SwissKnife;
```

### Key Classes

#### TypeExtensions

Extension methods for `Type` to simplify reflection and type-name helpers.

```csharp
public static class TypeExtensions
{
    /// <summary>
    /// Friendly printable name for types including generic types.
    /// </summary>
    public static string PrintableName(this Type type);

    /// <summary>
    /// Backwards-compatible Name() extension.
    /// </summary>
    public static string Name(this Type type);

    /// <summary>
    /// Convert interface type names to concrete class-like names.
    /// Example: IMyInterface -> MyInterface
    /// </summary>
    public static string RenameToConcreteType(this Type type);

    /// <summary>
    /// Return all properties including inherited interface and base class properties.
    /// </summary>
    public static IEnumerable<PropertyInfo> GetAllProperties(
        this Type type, 
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public);

    /// <summary>
    /// Return all methods including inherited interface methods.
    /// </summary>
    public static IEnumerable<MethodInfo> GetAllMethods(
        this Type type, 
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public);

    /// <summary>
    /// Return all events including inherited interface events.
    /// </summary>
    public static IEnumerable<EventInfo> GetAllEvents(
        this Type type, 
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public);
}
```

#### Validator

Provides guard and validation helpers (marked as Obsolete).

```csharp
public static class Validator
{
    /// <summary>
    /// Throws an exception if the condition is false.
    /// </summary>
    /// <remarks>
    /// OBSOLETE: Use ArgumentNullException, ArgumentException, or Debug.Assert instead.
    /// </remarks>
    [Obsolete("Use built-in guard patterns instead")]
    public static void Check(this bool condition, string message = null);

    [Obsolete("Use built-in guard patterns instead")]
    public static void Check<TException>(this bool condition, string message = null) 
        where TException : Exception, new();

    [Obsolete("Use built-in guard patterns instead")]
    public static void Check(this Func<bool> predicate, string message = null);

    [Obsolete("Use built-in guard patterns instead")]
    public static void Check<TException>(this Func<bool> predicate, string message = null) 
        where TException : Exception, new();
}
```

#### AttributeExtensions

Extension methods for working with attributes.

```csharp
// See AttributeExtensions.cs for actual API
```

#### EnumerableExtensions

Extension methods for collection operations.

```csharp
// See EnumerableExtensions.cs for actual API
```

#### StringExtensions

Extension methods for string manipulation.

```csharp
// See StringExtensions.cs for actual API
```

#### CoerceExtensions

Extension methods for type coercion.

```csharp
// See CoerceExtensions.cs for actual API
```

#### MonadExtensions

Monadic operations for functional-style programming.

```csharp
// See MonadExtensions.cs for actual API
```

#### BitTwiddlerExtensions

Bit manipulation utilities.

```csharp
// See BitTwiddlerExtensions.cs for actual API
```

## Best Practices

### When to Use

? **DO** use SwissKnife for:
- Type inspection and reflection
- Working with interface hierarchies
- Getting printable type names
- Object description and debugging

? **DON'T** use deprecated Validator methods:
- Use `ArgumentNullException.ThrowIfNull()` instead
- Use `ArgumentException` for validation
- Use `Debug.Assert()` for debug-time checks

### Performance Considerations

- **TypeExtensions** methods handle interface inheritance efficiently with cycle detection
- Reflection-based methods have caching where appropriate
- Extension methods have minimal overhead

### Null Safety

SwissKnife uses nullable reference types and modern C# patterns:

```csharp
// TypeExtensions throws ArgumentNullException for null types
ArgumentNullException.ThrowIfNull(type);

// Use modern null checks instead of deprecated Validator
ArgumentNullException.ThrowIfNull(myObject, nameof(myObject));
```

## Available Classes

The following classes are available in `BrightSword.SwissKnife`:

1. **TypeExtensions** - Type and reflection utilities
2. **Validator** - Guard helpers (deprecated, use built-in patterns)
3. **AttributeExtensions** - Attribute utilities
4. **EnumerableExtensions** - Collection extensions
5. **StringExtensions** - String utilities
6. **CoerceExtensions** - Type coercion
7. **MonadExtensions** - Functional patterns
8. **BitTwiddlerExtensions** - Bit manipulation
9. **Disposable** - Disposable pattern helpers
10. **Functional** - Functional programming utilities
11. **ObjectDescriber** - Object description for debugging
12. **SequentialGuid** - Sequential GUID generation
13. **ConcurrentDictionary** - Thread-safe dictionary helpers

## Documentation Files

See the `docs/` folder for detailed documentation on each class:
- [TypeExtensions](TypeExtensions.md)
- [Validator](Validator.md)
- [AttributeExtensions](AttributeExtensions.md)
- [EnumerableExtensions](EnumerableExtensions.md)
- [StringExtensions](StringExtensions.md)

## Samples

See the test project for comprehensive examples:
- `BrightSword.SwissKnife.Tests` - Unit tests showing usage

## Version History

### 1.0.19 (Current)
- Initial monorepo release
- Full XML documentation
- .NET 10 support
- Validator methods marked as Obsolete

## Contributing

See the [Contributing Guidelines](../../docs/CONTRIBUTING.md) for information on how to contribute to SwissKnife.

## Support

- **Source Code**: [GitHub Repository](https://github.com/brightsword/BrightSword)
- **NuGet Package**: [NuGet.org](https://www.nuget.org/packages/BrightSword.SwissKnife/)
- **Issues**: [GitHub Issues](https://github.com/brightsword/BrightSword/issues)

## License

This project is licensed under the Creative Commons Legal Code (CC0 1.0 Universal). See the [main repository LICENSE](../../LICENSE) for details.

---

**Part of the BrightSword family of libraries**
