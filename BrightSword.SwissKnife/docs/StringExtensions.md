# StringExtensions

## Purpose
Provides extension methods for <code>string</code> to split into segments by camel case, underscores, spaces, or punctuation. These helpers simplify string tokenization for display, parsing, or formatting scenarios.

## When to Use
- When you need to split strings for display, formatting, or parsing.
- When you want to convert camel case, dotted, or underscored strings into segments.

## How to Use
Use these methods to split strings by camel case, underscores, dots, or custom separators.

## Key APIs
- <code>SplitCamelCase(this string @this)</code>: Splits a camel-case string into its constituent segments.
- <code>SplitCamelCaseAndUnderscore(this string @this)</code>: Splits a string into segments by camel case and underscores.
- <code>SplitDotted(this string @this)</code>: Splits a string into segments by dots.
- <code>SplitIntoSegments(this string @this, ...)</code>: Splits a string into segments by spaces, camel case, punctuation, or custom separators.

## Examples
```csharp
var segments = "CamelCaseString".SplitCamelCase(); // ["Camel", "Case", "String"]
var segments2 = "Camel_CaseString".SplitCamelCaseAndUnderscore(); // ["Camel", "Case", "String"]
var segments3 = "A.B.C".SplitDotted(); // ["A", "B", "C"]
var segments4 = "A_B C.D".SplitIntoSegments(true, true, true, '_', '.'); // ["A", "B", "C", "D"]
```

## Remarks
These helpers simplify string tokenization and formatting. They are useful for display, parsing, and code generation scenarios.
