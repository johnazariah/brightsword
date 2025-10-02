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
