# StringExtensions

## Purpose

Provides helpers for tokenizing and splitting strings into readable segments based on camel case, underscores, punctuation, spaces, and custom separators. Implemented in `StringExtensions.cs`.

## When to Use

- Humanizing identifiers (e.g., `CamelCase` -> `Camel Case`).
- Breaking dotted or underscored identifiers into parts for display, logging, or parsing.
- Extracting tokens from strings for code generation or documentation tools.

## API Reference

- `IEnumerable<string> SplitCamelCase(this string @this)` — Split a camel-case string into segments, ignoring punctuation splitting.
- `IEnumerable<string> SplitCamelCaseAndUnderscore(this string @this)` — Split camel case and underscores.
- `IEnumerable<string> SplitDotted(this string @this)` — Split string on dots ('.').
- `IEnumerable<string> SplitIntoSegments(this string @this, bool splitBySpace = true, bool splitOnCamelCase = true, bool splitOnPunctuation = true, params char[] separators)` — Flexible splitter with options.

## Examples

```csharp
var parts = "CamelCaseExample".SplitCamelCase().ToArray(); // ["Camel", "Case", "Example"]
var parts2 = "XMLHttpRequest".SplitCamelCase().ToArray(); // ["XML", "Http", "Request"]
var dotted = "part.one.two".SplitDotted().ToArray(); // ["part", "one", "two"]
var mixed = "A_B C.D".SplitIntoSegments(true, true, true, '_', '.').ToArray(); // ["A", "B", "C", "D"]
```

## Implementation Notes

- The splitter manages acronyms (consecutive uppercase letters) specially: `XMLHttpRequest` produces `XML`, `Http`, `Request`.
- Recognized punctuation defaults to `char.IsPunctuation`, but passing `separators` replaces punctuation recognition with those chars.
- Empty or whitespace-only segments are skipped.

## Remarks

- The algorithms are implemented for readability and correctness; they avoid allocations where reasonable but favor clarity.
- Use `string.Join(" ", input.SplitCamelCase())` to produce a human-readable phrase.

---

This document mirrors the implementation in `StringExtensions.cs`.
