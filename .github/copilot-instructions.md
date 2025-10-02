# Copilot instructions — BrightSword

This file gives targeted, actionable knowledge for an AI coding agent working in this repo.

Overview
- Two primary projects in the solution:
  - `BrightSword.Feber` — an expression-based operation/builder library (see `BrightSword.Feber/Core`).
  - `BrightSword.SwissKnife` — a collection of utility/extension methods and small helpers used by Feber.
- `BrightSword.Feber` has a project reference to `BrightSword.SwissKnife` (see `BrightSword.Feber.csproj`).
- `global.json` pins a .NET SDK (currently a .NET 10 RC). Use the same SDK when building.

Big-picture architecture and patterns
- Feber implements builder types that compose `Expression` trees and compile delegates at runtime.
  - Key pattern: builders expose cached delegates, e.g. `Action<T>` compiled from `Expression.Block(OperationExpressions)` and parameter expressions.
  - Look in `BrightSword.Feber/Core` for `*Builder` types and the `OperationExpressions`, `InstanceParameterExpression` / `LeftInstanceParameterExpression` patterns.
- SwissKnife is a pure helper/extension library (lots of extension methods in `*.cs` files at project root). Treat it as a utility dependency — changes here can affect many consumers.

Developer workflows (build / debug / quick checks)
- Use the pinned SDK (global.json) and build the solution:
  - dotnet build "c:\work\BrightSword\BrightSword.sln"
- No test projects detected in the repository root; run any sample code in `BrightSword.Feber/Samples` by adding a small console project that references the libraries or by loading the solution into Visual Studio.
- Binaries appear under `bin/Debug/net10.0` (or `Release`). Use those outputs for quick smoke runs.

Project-specific conventions and cautions
- Project files set `GenerateAssemblyInfo` to false and include explicit assembly versioning in the csproj. Do not remove or silently change version fields without a deliberate versioning update.
- `BrightSword.SwissKnife.csproj` sets `AllowUnsafeBlocks` to true. Be careful when editing unsafe code.
- Expression-building code is sensitive to ordering and parameter expressions. When refactoring builder code, preserve the structure used to create lambda expressions:
  - Example canonical pattern: `Expression.Lambda<Action<T>>(Expression.Block(OperationExpressions), InstanceParameterExpression).Compile();`
- Caching pattern: fields often cache compiled delegates. Look for `_field ??= Build...()` style; use null-coalescing assignment where appropriate but keep behavior identical.

Integration points & external dependencies
- `BrightSword.Feber` references `BrightSword.SwissKnife` as a project reference.
- NuGet dependency in SwissKnife: `System.Configuration.ConfigurationManager` (see `BrightSword.SwissKnife.csproj`). No other external services detected.

Patterns to follow when making changes
- Preserve public API shapes unless intentionally bumping versions. The projects track assembly/file versions in csproj files.
- Avoid modifying expression construction order; unit tests are sparse, so subtle semantics can be lost.
- Prefer small, local refactors (use Roslyn/code-fix tools) and run `dotnet build` after each change.

Recommended tools and commands for code cleanup
- Use the repository SDK (global.json) and these commands from PowerShell:
  - dotnet restore "c:\work\BrightSword\BrightSword.sln"
  - dotnet build "c:\work\BrightSword\BrightSword.sln"
  - dotnet tool install -g dotnet-format; dotnet format "c:\work\BrightSword\BrightSword.sln"
- Consider adding analyzers (`Microsoft.CodeAnalysis.NetAnalyzers`) to surface modern C# suggestions; run with `dotnet build`.

Examples (what to look at)
- `BrightSword.Feber/Core/ActionBuilder.cs` — compiles expression blocks into `Action<>` delegates and caches them.
- `BrightSword.SwissKnife/StringExtensions.cs` and `EnumerableExtensions.cs` — common extension patterns used across the codebase.

If you need more context
- Open `BrightSword.Feber/Core` and the `Samples` folder to see how the builders are used in practice.
- When in doubt about a refactor touching expression trees, create a tiny console project that exercises the changed API to validate runtime behaviour.

Feedback
- If any sections are unclear or missing specifics you want included (build flavors, CI steps, versioning policy), tell me and I will iterate.
