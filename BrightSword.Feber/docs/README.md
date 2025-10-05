# BrightSword.Feber Documentation

**Automated delegate generation using LINQ Expression trees for property-based operations**

## Overview

BrightSword.Feber provides builder classes that automate the generation of compiled delegates for property-based operations. It uses LINQ Expression trees to scan object properties and compose efficient, type-safe operations at runtime.

The library is designed to maximize runtime performance by building and compiling expression trees once per type, then caching the resulting delegates. This eliminates the overhead of repeated reflection while providing the flexibility of runtime code generation.

## Installation

```bash
dotnet add package BrightSword.Feber
```

## Key Concepts

### ActionBuilder

Generates `Action<T>` or `Action<TLeft, TRight>` delegates that perform side-effecting operations on object properties.

### FunctionBuilder

Generates `Func<T, TResult>` delegates that fold/aggregate property values into a result.

### OperationBuilder

Base classes that scan properties and compose Expression trees for each property.

## Usage Examples

See the sample application and test suite for working examples:
- `BrightSword.Feber.SamplesApp` - Console app with examples
- `BrightSword.Feber.Tests` - Comprehensive test suite

## API Reference

See the following documentation files for details:
- [ActionBuilder.md](ActionBuilder.md)
- [FunctionBuilder.md](FunctionBuilder.md)
- [OperationBuilders.md](OperationBuilders.md)
- [DynamicExpressionUtilities.md](DynamicExpressionUtilities.md)

## Available Classes

- **ActionBuilder**
- **FunctionBuilder**
- **OperationBuilders**
- **DynamicExpressionUtilities**

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
