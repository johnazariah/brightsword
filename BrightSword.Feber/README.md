# BrightSword.Feber

BrightSword.Feber is an expression-based builder library for .NET that automates the generation of compiled delegates (`Action<>`, `Func<>`) operating on object properties. It is designed to maximize runtime performance and developer productivity by eliminating repetitive boilerplate code and enabling fast, reusable property-based operations.

---

## Table of Contents
- [Overview](#overview)
- [When is this library useful?](#when-is-this-library-useful)
- [Why use this approach?](#why-use-this-approach)
- [Capabilities](#capabilities)
- [How to use (for library users)](#how-to-use-for-library-users)
- [Quick tasks](#quick-tasks)
- [Warm-up and Lazy<T> patterns](#warm-up-and-lazyt-patterns)
- [Try the sample app](#try-the-sample-app)
- [Documentation](#documentation)
- [Contributor Notes](#contributor-notes)
- [Publishing and Versioning](#publishing-and-versioning)

---

## Overview
BrightSword.Feber composes property-based operations into compiled delegates for high-performance, low-boilerplate code. It is ideal for scenarios where you need to perform repeated operations (printing, copying, mapping, validation, aggregation) on all properties of a type.

---

## When is this library useful?
- When you need to perform operations (printing, copying, mapping, validation, aggregation) on all properties of a type, and want to avoid writing repetitive code.
- When you want to maximize performance for repeated operations on many instances of the same type.
- When you want to automate delegate generation for property-based logic, making your codebase cleaner and easier to maintain.

## Why use this approach?
- **Performance:** Compiled delegates execute much faster than repeated reflection or dynamic code generation. The cost of building the operation is paid only once per type; subsequent invocations are as fast as a direct delegate call.
- **Developer Productivity:** This approach reduces code-bloat and improves maintainability by automating property-based boilerplate. Instead of writing custom logic for every property, you generate the operation once and reuse it.
- **Scalability:** Ideal for scenarios where the same operation will be applied to many instances of the same type (e.g., serialization, mapping, printing, copying).

## Capabilities
- Compose per-property operation expressions into a single expression block and compile it into a cached delegate.
- Generate both unary (`Action<T>`, `Func<T, TResult>`) and binary (`Action<TLeft, TRight>`, `Func<TLeft, TRight, TResult>`) delegates for property-based operations.
- Extensible builder types for custom logic: `ActionBuilder`, `FunctionBuilder`, `OperationBuilderBase`, and more.
- Utilities for warm-up and lazy initialization of compiled delegates.
- Per-class documentation and copyable examples in the `docs/` folder.

---

## How to use (for library users)
- Reference the library in your project and use the builder types to generate delegates for your property-based operations.
- See the `docs/` folder for per-class documentation and examples:
  - [ActionBuilder](docs/ActionBuilder.md) — compiles and caches `Action` delegates
  - [FunctionBuilder](docs/FunctionBuilder.md) — compiles and caches `Func` delegates
  - [OperationBuilders](docs/OperationBuilders.md) — low-level property scanning and expression generation

### Quick tasks
- **Build:**
  ```powershell
  dotnet build BrightSword.Feber.sln -c Release
  ```
- **Pack locally (produces .nupkg in `artifacts`):**
  ```powershell
  dotnet pack BrightSword.Feber\BrightSword.Feber.csproj -c Release -o ..\artifacts
  ```
- **Tests:**
  ```powershell
  dotnet test BrightSword.Feber.Tests -c Release
  ```

### Warm-up and Lazy<T> patterns
- Warm-up helper: `BrightSword.Feber.BuilderWarmup.Warmup(IEnumerable<Action>)` can be used at application startup to force builder compilation ahead of first request.
- Lazy<T> pattern: if you prefer thread-safe lazy initialization, create a Lazy-wrapped compiled delegate and return `_lazy.Value` from `Action`/`Function` properties. The repo includes a sample `BrightSword.Feber.Samples.LazyActionBuilderExample<TProto,TInstance>` demonstrating this approach.

Example (warm-up at startup)
```csharp
var warmups = new Action[] {
    () => new BrightSword.Feber.Samples.LazyActionBuilderExample<MyProto, MyProto>().Action(default!)
};
BrightSword.Feber.BuilderWarmup.Warmup(warmups);
```

Example (Lazy<T> pattern)
```csharp
private readonly Lazy<Action<TInstance>> _lazyAction = new Lazy<Action<TInstance>>(() => BuildAction(), isThreadSafe: true);
public Action<TInstance> Action => _lazyAction.Value;
```

### Try the sample app
Run the small console app that demonstrates the lazy-compiled action which prints property names/values:
```powershell
dotnet run --project .\BrightSword.Feber.SamplesApp\BrightSword.Feber.SamplesApp.csproj
```

---

## Documentation
This library includes per-class documentation in the `docs/` folder. Key topics:
- [ActionBuilder](docs/ActionBuilder.md)
- [FunctionBuilder](docs/FunctionBuilder.md)
- [OperationBuilders](docs/OperationBuilders.md)
- [DynamicExpressionUtilities](docs/DynamicExpressionUtilities.md)

See each file in `BrightSword.Feber/docs/` for detailed usage, implementation notes, and copyable examples.

---

## Contributor Notes
- Feber composes Expression trees; take care when changing API shapes — the library compiles delegates at runtime and subtle changes can affect consumers.
- Expression-building code is sensitive to ordering and parameter expressions. When refactoring builder code, preserve the structure used to create lambda expressions:
  - Example canonical pattern: `Expression.Lambda<Action<T>>(Expression.Block(OperationExpressions), InstanceParameterExpression).Compile();`
- Caching pattern: fields often cache compiled delegates. Look for `_field ??= Build...()` style; use null-coalescing assignment where appropriate but keep behavior identical.
- Project files set `GenerateAssemblyInfo` to false and include explicit assembly versioning in the csproj. Do not remove or silently change version fields without a deliberate versioning update.
- Prefer small, local refactors and run `dotnet build` after each change.
- See `.github/copilot-instructions.md` for more contributor guidance and architectural notes.

## Publishing and Versioning
- This project is packaged independently as `BrightSword.Feber` and depends on `BrightSword.SwissKnife`.
- Versioning is managed by the repo-level `versions.json` and the `tools/bump_versions.py` helper.
- Use the repo `build.prov` for orchestrated bump/pack/publish flows (see `PUBLISHING.md`).

