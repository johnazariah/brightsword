# BrightSword.SwissKnife

SwissKnife is a lightweight set of utility and extension helpers used across BrightSword libraries. It contains non-domain-specific helpers intended for reuse.

Why use SwissKnife
- Centralizes small helpers and conventions (type naming, reflection helpers, enumerable/string helpers).
- Reduces duplicated utility code across projects.

Notable Features
- `TypeExtensions` — helpers for type names (`PrintableName`, `RenameToConcreteType`) and enumerating inherited interface members.
- `EnumerableExtensions`, `StringExtensions`, `Validator` — common helpers used throughout the codebase.

Guidance
- Keep utilities small and focused. Avoid introducing large dependencies here.
- Unit-test any helper that implements non-trivial logic.
- When changing public APIs, be mindful of cross-project dependencies (Feber and Squid reference SwissKnife code).

Example: Using `TypeExtensions` to get a printable name
```csharp
var t = typeof(Dictionary<int, string>);
Console.WriteLine(t.PrintableName()); // "Dictionary<Int32, String>"
```
