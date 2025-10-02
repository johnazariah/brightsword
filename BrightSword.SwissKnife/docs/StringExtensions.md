# StringExtensions

Purpose
- String helper extensions used by the library for trimming, safe parsing, and formatting operations.

Key APIs
- IsNullOrEmptyOrWhiteSpace(this string s)
- ToSnakeCase(this string s)

Usage
```csharp
if (s.IsNullOrEmptyOrWhiteSpace()) ...
var snake = "HelloWorld".ToSnakeCase();
```

Notes
- Implementations favor correctness and small allocations.
