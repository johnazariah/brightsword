# BrightSword.SwissKnife Documentation

A concise collection of utility classes and extension methods for .NET development.

## Overview

BrightSword.SwissKnife provides small, dependency-light helpers used across BrightSword projects. This documentation reflects the current public surface in the codebase.

## Installation

```bash
dotnet add package BrightSword.SwissKnife
```

## Documented Components

All classes live in the `BrightSword.SwissKnife` namespace. The following documents describe the components that are present in the repository source files:

- [TypeExtensions](TypeExtensions.md) — type inspection and reflection helpers
- [StringExtensions](StringExtensions.md) — string tokenization helpers
- [AttributeExtensions](AttributeExtensions.md) — attribute discovery and value extraction
- [CoerceExtensions](CoerceExtensions.md) — type coercion and conversion helpers
- [ObjectDescriber](ObjectDescriber.md) — expression-based name extraction helpers
- [Validator](Validator.md) — legacy guard helpers (marked Obsolete)
- [Functional](Functional.md) — functional helpers (Y combinator, memoization)
- [ConcurrentDictionary](ConcurrentDictionary.md) — small nested concurrent dictionary helper

## Usage Examples

See each document for examples that match the current implementation. Unit tests in `BrightSword.SwissKnife.Tests` contain additional usage examples.

## Contributing

Update the per-class docs whenever public APIs change. Keep examples and API signatures in sync with source files.

---

**Part of the BrightSword family of libraries**
