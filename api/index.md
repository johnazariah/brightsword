# BrightSword API Documentation

Welcome to the BrightSword API documentation. This section contains detailed API references for all packages in the monorepo.

## Packages

### [BrightSword.SwissKnife](~/swissknife/README.md)

Utility classes and extension methods for .NET development.

**Namespace:**
- `BrightSword.SwissKnife` - All utilities and extension methods

**Key classes:**
- `TypeExtensions` - Type inspection and reflection helpers
- `Validator` - Guard helpers (deprecated)
- `AttributeExtensions` - Attribute utilities
- `EnumerableExtensions` - Collection extensions
- `StringExtensions` - String utilities
- `CoerceExtensions` - Type coercion
- `MonadExtensions` - Functional patterns
- `BitTwiddlerExtensions` - Bit manipulation
- `Disposable` - Disposable pattern helpers
- `Functional` - Functional programming utilities
- `ObjectDescriber` - Object description
- `SequentialGuid` - Sequential GUID generation
- `ConcurrentDictionary` - Thread-safe dictionary helpers

**Use cases:**
- Type inspection and reflection (interface hierarchies, printable type names)
- Attribute handling
- Collection operations
- String manipulation
- Bit manipulation

### [BrightSword.Crucible](~/crucible/README.md)

Unit testing utilities for MSTest.

**Namespace:**
- `BrightSword.Crucible` - Exception testing helpers

**Key classes:**
- `ExceptionHelper` - Fluent exception testing

**Use cases:**
- Fluent exception testing with `ExpectException<T>()`
- Validating exception messages and properties
- Making test assertions more expressive

### [BrightSword.Feber](~/feber/README.md)

Automated delegate generation using LINQ Expression trees for property-based operations.

**Namespaces:**
- `BrightSword.Feber.Core` - ActionBuilder, FunctionBuilder base classes

**Key classes:**
- `ActionBuilder<TProto, TInstance>` - Unary action builders
- `ActionBuilder<TProto, TLeft, TRight>` - Binary action builders
- `FunctionBuilder<TProto, TInstance, TResult>` - Function builders
- `OperationBuilder` - Base operation builder classes

**Use cases:**
- Automated property copying/cloning
- Object pretty-printing
- Property validation
- Hash code generation
- Property-based mapping

**Performance:**
- First call: ~10-100ms (expression building + compilation)
- Subsequent calls: <0.001ms (cached compiled delegate)

### [BrightSword.Squid](~/squid/README.md)

Runtime type emission utilities using Reflection.Emit.

**Namespaces:**
- `BrightSword.Squid` - Type creators and builders
- Additional namespaces as documented in Squid docs

**Use cases:**
- Dynamic type creation
- Data transfer object generation
- Behavior-based type composition
- Advanced runtime type scenarios

## Getting Started

Browse the API reference by navigating through the packages above, or use the search functionality to find specific types and members.

## Package Dependencies

```
SwissKnife (base - no dependencies)
    ?
Feber (depends on SwissKnife)
    ?
Squid (depends on Feber and SwissKnife)

Crucible (independent - only depends on MSTest)
```

## Additional Resources

- [Build Guide](~/docs/BUILD.md)
- [Contributing Guidelines](~/docs/CONTRIBUTING.md)
- [Architecture Overview](~/docs/ARCHITECTURE.md)
- [Versioning Strategy](~/docs/VERSIONING.md)
