# BrightSword.SwissKnife

Small utility and extension helpers used across the BrightSword projects.

Quick tasks

- Build:

```powershell
dotnet build BrightSword.SwissKnife.sln -c Release
```

- Pack locally (produces .nupkg in `artifacts` when using the repo-level `build.proj`):

```powershell
dotnet pack BrightSword.SwissKnife\BrightSword.SwissKnife.csproj -c Release -o ..\artifacts
```

- Tests:

```powershell
dotnet test BrightSword.SwissKnife.Tests -c Release
```

Publishing and versions
- This project is packaged independently as `BrightSword.SwissKnife`.
- Versioning is managed by the repo-level `versions.json` and `tools/bump_versions.py` helper.
- For CI/publish flows use the root `build.proj` targets (for example `PublishSwissknife`). See the repo `PUBLISHING.md` for full details.

Notes
- This project is intended to be reusable and dependency-light. See the source files for public extension methods and helpers.

Documentation

This library includes API documentation in the `docs/` folder. Key topics:

- AttributeExtensions — helpers for reflection-based attribute access (`docs/AttributeExtensions.md`)
- EnumerableExtensions — helpers for collections (`docs/EnumerableExtensions.md`)
- Functional — small functional utilities (`docs/Functional.md`)
- StringExtensions — string helper methods (`docs/StringExtensions.md`)
- TypeExtensions — reflection helpers (`docs/TypeExtensions.md`)

Read each file in `BrightSword.SwissKnife/docs/` for per-class details, usage examples and notes.
