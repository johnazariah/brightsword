# BrightSword.Packages

A single composite NuGet package that bundles all BrightSword libraries:

| Package | Description |
|---------|-------------|
| **BrightSword.SwissKnife** | Utility classes and extension methods for .NET |
| **BrightSword.Crucible** | Unit testing utilities for MSTest |
| **BrightSword.Feber** | Fast Expression-Based Extensions for Reflection |
| **BrightSword.Squid** | Runtime type emission via `System.Reflection.Emit` |

## Installation

```bash
dotnet add package BrightSword.Packages
```

Installing this single package brings in all BrightSword libraries as transitive dependencies.

## When to use this package

Use `BrightSword.Packages` when you want the full BrightSword toolkit. If you only need a specific library, install it directly instead (e.g. `dotnet add package BrightSword.SwissKnife`).
