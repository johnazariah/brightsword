# Validator

Purpose

Helpers for validating arguments, parameters, and object state with common validation exceptions and messages.

Public API

- void RequireNotNull(object? value, string paramName)
- void RequireRange(int value, int min, int max, string paramName)
- ValidationResult ValidateModel(object model)

Examples

```csharp
Validator.RequireNotNull(arg, nameof(arg));
Validator.RequireRange(count, 0, 100, nameof(count));
var result = Validator.ValidateModel(model);
if (!result.IsValid) throw new ArgumentException("model invalid");
```

Remarks

Centralizes guard patterns and consistent exception messages used across the codebase.
