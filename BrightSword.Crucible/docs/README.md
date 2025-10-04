# BrightSword.Crucible Documentation

**Unit testing utilities for MSTest**

## Overview

BrightSword.Crucible provides helper methods for writing cleaner, more expressive unit tests with MSTest. It focuses on making exception testing easier and more readable.

## Installation

```bash
dotnet add package BrightSword.Crucible
```

## Features

### Exception Testing

The primary feature is the `ExpectException<TException>` extension method, which provides a fluent, expressive way to test that code throws expected exceptions.

## Quick Start

### Basic Exception Testing

```csharp
using BrightSword.Crucible;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MyServiceTests
{
    [TestMethod]
    public void DivideByZero_ShouldThrowDivideByZeroException()
    {
        // Arrange
        var service = new MathService();
        
        // Act & Assert - Exception is expected and caught
        var exception = new Action(() => service.Divide(10, 0))
            .ExpectException<DivideByZeroException>();
        
        // The test passes because the exception was thrown
        Assert.IsNotNull(exception);
    }
    
    [TestMethod]
    public void InvalidOperation_ShouldThrowWithSpecificMessage()
    {
        // Arrange
        var service = new MyService();
        
        // Act & Assert - Exception with specific message is expected
        var exception = new Action(() => service.DoSomething(null))
            .ExpectException<ArgumentNullException>("Value cannot be null. (Parameter 'input')");
        
        // Test passes if exception is thrown with exact message
        Assert.IsNotNull(exception);
    }
}
```

## API Reference

### ExpectException<TException>

```csharp
public static TException ExpectException<TException>(
    this Action action,
    string expectedExceptionMessage = null) 
    where TException : Exception
```

**Parameters:**
- `action` - The action that should throw an exception
- `expectedExceptionMessage` (optional) - The exact exception message to expect

**Returns:**
- The caught exception of type `TException`

**Throws:**
- `InvalidOperationException` if:
  - No exception is thrown
  - Wrong exception type is thrown
  - Exception message doesn't match (when `expectedExceptionMessage` is specified)

## Usage Examples

### Example 1: Test Any Exception of Specific Type

```csharp
[TestMethod]
public void InvalidInput_ShouldThrowArgumentException()
{
    // Just verify ArgumentException is thrown, don't care about message
    var exception = new Action(() => 
        {
            throw new ArgumentException("Some error");
        })
        .ExpectException<ArgumentException>();
    
    Assert.IsNotNull(exception);
}
```

### Example 2: Test Exception with Specific Message

```csharp
[TestMethod]
public void NullParameter_ShouldThrowWithCorrectMessage()
{
    // Verify both exception type AND message
    var exception = new Action(() => 
        {
            throw new ArgumentNullException("parameter", "Value cannot be null.");
        })
        .ExpectException<ArgumentNullException>("Value cannot be null.");
    
    Assert.AreEqual("parameter", exception.ParamName);
}
```

### Example 3: Test Method Call

```csharp
public class Calculator
{
    public int Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }
}

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Divide_ByZero_ShouldThrow()
    {
        // Arrange
        var calc = new Calculator();
        
        // Act & Assert
        var exception = new Action(() => calc.Divide(10, 0))
            .ExpectException<DivideByZeroException>("Cannot divide by zero");
        
        Assert.IsNotNull(exception);
    }
}
```

### Example 4: Test Async Methods

```csharp
[TestMethod]
public async Task AsyncMethod_ShouldThrow()
{
    // For async methods, wrap in synchronous Action
    var service = new MyAsyncService();
    
    var exception = new Action(() => 
        {
            // Note: This will block - for truly async testing, 
            // consider using async Assert patterns
            service.ProcessAsync(null).GetAwaiter().GetResult();
        })
        .ExpectException<ArgumentNullException>();
    
    Assert.IsNotNull(exception);
}
```

## Benefits Over Standard MSTest

### Traditional MSTest Approach

```csharp
[TestMethod]
[ExpectedException(typeof(ArgumentNullException))]
public void TraditionalWay_TestsException()
{
    // Problem: Can't verify exception message
    // Problem: Can't inspect the exception
    // Problem: Less clear what's being tested
    DoSomethingThatThrows();
}
```

### Crucible Approach

```csharp
[TestMethod]
public void CrucibleWay_TestsException()
{
    // ? Can verify exception message
    // ? Can inspect the exception
    // ? Clear intent
    var exception = new Action(() => DoSomethingThatThrows())
        .ExpectException<ArgumentNullException>("Parameter cannot be null");
    
    // Can assert on exception properties
    Assert.AreEqual("param", exception.ParamName);
}
```

## Advanced Usage

### Verify Exception Properties

```csharp
[TestMethod]
public void ComplexException_ShouldHaveCorrectProperties()
{
    // Arrange
    var service = new ValidationService();
    
    // Act
    var exception = new Action(() => service.Validate(invalidData))
        .ExpectException<ValidationException>();
    
    // Assert on exception properties
    Assert.AreEqual(3, exception.Errors.Count);
    Assert.IsTrue(exception.Errors.Contains("Name is required"));
}
```

### Multiple Exception Tests

```csharp
[TestMethod]
public void Service_ShouldThrowDifferentExceptionsForDifferentInputs()
{
    var service = new MyService();
    
    // Test null input
    new Action(() => service.Process(null))
        .ExpectException<ArgumentNullException>();
    
    // Test empty string
    new Action(() => service.Process(""))
        .ExpectException<ArgumentException>("Input cannot be empty");
    
    // Test invalid format
    new Action(() => service.Process("invalid"))
        .ExpectException<FormatException>();
}
```

## Best Practices

### ? DO

- **Use ExpectException for exception testing** - More expressive than `[ExpectedException]`
- **Verify exception messages** - Ensures correct error reporting
- **Inspect exception properties** - Verify inner exceptions, data, etc.
- **Keep test intent clear** - The fluent syntax makes tests readable

### ? DON'T

- **Don't use for async methods directly** - Wrap async calls appropriately
- **Don't test too much in one test** - One exception scenario per test
- **Don't ignore the returned exception** - Use it to verify details

### Example: Good Test Structure

```csharp
[TestMethod]
public void ProcessOrder_WithNullCustomer_ShouldThrowArgumentNullException()
{
    // Arrange
    var orderService = new OrderService();
    var order = new Order { Customer = null };
    
    // Act
    var exception = new Action(() => orderService.ProcessOrder(order))
        .ExpectException<ArgumentNullException>("Customer cannot be null");
    
    // Assert
    Assert.AreEqual("Customer", exception.ParamName);
}
```

## Comparison with Other Approaches

### MSTest ExpectedException Attribute

```csharp
// ? Can't verify message, can't inspect exception
[TestMethod]
[ExpectedException(typeof(ArgumentException))]
public void OldWay() => DoSomething();

// ? Can verify message and inspect
[TestMethod]
public void NewWay()
{
    var ex = new Action(() => DoSomething())
        .ExpectException<ArgumentException>("Expected message");
    Assert.AreEqual("param", ex.ParamName);
}
```

### Try-Catch Approach

```csharp
// ? Verbose, easy to forget to Assert.Fail
[TestMethod]
public void VerboseWay()
{
    try
    {
        DoSomething();
        Assert.Fail("Expected exception");
    }
    catch (ArgumentException ex)
    {
        Assert.AreEqual("Expected", ex.Message);
    }
}

// ? Concise and clear
[TestMethod]
public void ConciseWay()
{
    var ex = new Action(() => DoSomething())
        .ExpectException<ArgumentException>("Expected");
}
```

## Testing

The library itself is well-tested. See test projects for examples of how to use Crucible in various scenarios.

## Dependencies

- **MSTest.TestFramework** - Version 2.2.10+
- **.NET 10**

## Version History

### 1.0.16 (Current)
- Initial monorepo release
- .NET 10 support
- Enhanced documentation

## Contributing

See the [Contributing Guidelines](../../docs/CONTRIBUTING.md).

## License

This project is licensed under the Creative Commons Legal Code (CC0 1.0 Universal). See the [main repository LICENSE](../../LICENSE) for details.

---

**Part of the BrightSword family of libraries**

## Related Libraries

- [BrightSword.SwissKnife](../BrightSword.SwissKnife/docs/README.md) - Utility functions
- [BrightSword.Feber](../BrightSword.Feber/docs/README.md) - Expression builders
- [BrightSword.Squid](../BrightSword.Squid/docs/README.md) - Type emission utilities
