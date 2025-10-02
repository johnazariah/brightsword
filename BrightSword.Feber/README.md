# BrightSword.Feber

Expression-based builder library that composes Expression trees and compiles delegates at runtime.

Quick tasks

- Build:

```powershell
dotnet build BrightSword.Feber.sln -c Release
```

- Pack locally (produces .nupkg in `artifacts` when using the repo-level `build.proj`):

```powershell
dotnet pack BrightSword.Feber\BrightSword.Feber.csproj -c Release -o ..\artifacts
```

- Tests:

```powershell
dotnet test BrightSword.Feber.Tests -c Release
```

Publishing and versions
- This project is packaged independently as `BrightSword.Feber` and depends on `BrightSword.SwissKnife`.
- Versioning is managed by the repo-level `versions.json` and the `tools/bump_versions.py` helper.
- Use the repo `build.proj` for orchestrated bump/pack/publish flows (see `PUBLISHING.md`).

Notes for contributors
- Feber composes Expression trees; take care when changing API shapes — the library compiles delegates at runtime and subtle changes can affect consumers.

Documentation

This library includes per-class documentation in the `docs/` folder. Key topics:

- ActionBuilder — compiles and caches Action delegates (`docs/ActionBuilder.md`)
- FunctionBuilder — compiles and caches Func delegates (`docs/FunctionBuilder.md`)
- DynamicExpressionUtilities — expression-tree helpers (`docs/DynamicExpressionUtilities.md`)
- OperationBuilders — small expression fragment builders (`docs/OperationBuilders.md`)
- OperationBuilders — small expression fragment builders (`docs/OperationBuilders.md`)

See `BrightSword.Feber/docs/` for the full per-class documentation and copyable examples.

Read each file in `BrightSword.Feber/docs/` for detailed usage and notes about implementation details.

Warm-up and Lazy<T> patterns

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

Try the sample app

Run the small console app that demonstrates the lazy-compiled action which prints property names/values:

```powershell
dotnet run --project .\BrightSword.Feber.SamplesApp\BrightSword.Feber.SamplesApp.csproj
```

