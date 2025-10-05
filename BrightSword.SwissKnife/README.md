# BrightSword.SwissKnife

BrightSword.SwissKnife is a collection of small, reusable utility and extension helpers for .NET, used across the BrightSword projects. It is designed to maximize developer productivity and code clarity by providing robust, dependency-light helpers for common programming tasks.

---

## Table of Contents
- [Overview](#overview)
- [When is this library useful?](#when-is-this-library-useful)
- [Why use this approach?](#why-use-this-approach)
- [Capabilities](#capabilities)
- [How to use (for library users)](#how-to-use-for-library-users)
- [Quick tasks](#quick-tasks)
- [Documentation](#documentation)
- [Contributor Notes](#contributor-notes)
- [Publishing and Versioning](#publishing-and-versioning)

---

## Overview
BrightSword.SwissKnife provides a set of extension methods and helpers for reflection, validation, collections, string manipulation, and more. It is intended to be reusable, dependency-light, and suitable for use in any .NET project.

---

## When is this library useful?
- When you need robust, well-tested extension methods for common .NET types (collections, strings, reflection, etc.).
- When you want to avoid reinventing utility code for argument validation, type inspection, or attribute handling.
- When you want to maximize productivity and code clarity by using proven helpers.

## Why use this approach?
- **Developer Productivity:** SwissKnife reduces code-bloat and improves maintainability by centralizing common helpers and extension methods. Instead of writing custom logic for every project, you reuse the tested utilities.
- **Reusability:** The library is designed to be dependency-light and broadly applicable, making it easy to adopt in any .NET solution.
- **Reliability:** Helpers are used across multiple BrightSword projects and are tested in real-world scenarios.

## Capabilities
- Reflection helpers for type/property inspection (`TypeExtensions`, `AttributeExtensions`).
- Argument/parameter guard helpers (`Validator`).
- Collection and string extension methods (`EnumerableExtensions`, `StringExtensions`).
- Utility classes for common patterns (disposable, monads, bit twiddling, etc.).
- Per-class documentation and copyable examples in the `docs/` folder.

---

## How to use (for library users)
- Reference the library in your project and use the extension methods and helpers as needed.
- See the `docs/` folder for per-class documentation and examples:
  - [TypeExtensions](docs/TypeExtensions.md) â€” reflection helpers
  - [Validator](docs/Validator.md) â€” argument/parameter guard helpers
  - [AttributeExtensions](docs/AttributeExtensions.md) â€” attribute handling helpers
  - [EnumerableExtensions](docs/EnumerableExtensions.md) â€” collection helpers
  - [StringExtensions](docs/StringExtensions.md) â€” string manipulation helpers

### Quick tasks
- **Build:**
  ```powershell
  dotnet build BrightSword.SwissKnife.sln -c Release
  ```
- **Pack locally (produces .nupkg in `artifacts`):**
  ```powershell
  dotnet pack BrightSword.SwissKnife\BrightSword.SwissKnife.csproj -c Release -o ..\artifacts
  ```
- **Tests:**
  ```powershell
  dotnet test BrightSword.SwissKnife.Tests -c Release
  ```

---

## Documentation
This library includes per-class documentation in the `docs/` folder. Key topics:
- [TypeExtensions](docs/TypeExtensions.md)
- [Validator](docs/Validator.md)
- [AttributeExtensions](docs/AttributeExtensions.md)
- [EnumerableExtensions](docs/EnumerableExtensions.md)
- [StringExtensions](docs/StringExtensions.md)

See each file in `BrightSword.SwissKnife/docs/` for detailed usage, implementation notes, and copyable examples.

---

## Contributor Notes
- SwissKnife is intended to be dependency-light and broadly reusable. Avoid introducing heavy dependencies or project-specific logic.
- Extension methods should be well-tested and follow .NET conventions for naming and behavior.
- When refactoring, preserve public API shapes unless intentionally bumping versions. The projects track assembly/file versions in csproj files.
- Prefer small, local refactors and run `dotnet build` after each change.
- See `.github/copilot-instructions.md` for more contributor guidance and architectural notes.

## Publishing and Versioning
- This project is packaged independently as `BrightSword.SwissKnife`.
- Versioning is managed by the repo-level `versions.json` and `tools/bump_versions.py` helper.
- For CI/publish flows use the root `build.proj` targets (for example `PublishSwissknife`). See the repo `PUBLISHING.md` for full details.
