# BrightSword Monorepo

A collection of high-quality .NET libraries for utilities, testing, automated code generation, and advanced serialization.

[![CI Build](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml/badge.svg)](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml)
[![License: CC BY 4.0](https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by/4.0/)

## ðŸ“¦ Packages

This monorepo contains the following NuGet packages:

| Package | Version | Description |
|---------|---------|-------------|
| [BrightSword.SwissKnife](./BrightSword.SwissKnife) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.SwissKnife.svg)](https://www.nuget.org/packages/BrightSword.SwissKnife/) | Utility classes and extension methods for .NET development |
| [BrightSword.Crucible](./BrightSword.Crucible) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Crucible.svg)](https://www.nuget.org/packages/BrightSword.Crucible/) | Unit testing utilities for MSTest |
| [BrightSword.Feber](./BrightSword.Feber) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Feber.svg)](https://www.nuget.org/packages/BrightSword.Feber/) | Automated delegate generation using Expression trees |
| [BrightSword.Squid](./BrightSword.Squid) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Squid.svg)](https://www.nuget.org/packages/BrightSword.Squid/) | Runtime type emission utilities |

## 🔧 Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and `pwsh` (PowerShell) available.

### Canonical build entrypoint
- Use the repository MSBuild driver: `dotnet msbuild Build.proj` so local runs match CI.

## 🛠 Contributor Quick Commands

Use these copy-paste examples to perform common tasks locally (they match CI behavior).

- Full CI-equivalent (clean, restore, build, test, pack):

```bash
# From repo root (PowerShell: quote multi-target argument)
dotnet msbuild Build.proj /t:CI /p:Configuration=Release /v:minimal
```

- Restore, build and run tests (multi-target quoting required in PowerShell):

```bash
dotnet msbuild Build.proj /t:"Restore;BuildPackages;BuildTests;Test" /p:Configuration=Release
```

- Pack a single project into `artifacts/packages`:

```bash
dotnet msbuild Build.proj /t:PackSingle /p:Configuration=Release;Package=BrightSword.SwissKnife
```

- Generate dependency manifest (used by CI):

```bash
pwsh ./scripts/generate-package-dependencies.ps1
```

- Generate docs (placeholder generator used by GitHub Pages workflow):

```bash
# build required projects first
dotnet msbuild Build.proj /t:"Restore;BuildPackages" /p:Configuration=Release
pwsh ./scripts/generate-docs.ps1
ls artifacts/docs   # verify index.html
```

- Bump a project's version (MSBuild `IncrementVersion` target):

```bash
# Increment patch locally (no commit)
dotnet msbuild /t:IncrementVersion /p:ProjectName=BrightSword.SwissKnife /p:Level=Patch
# To increment and commit locally (be careful):
dotnet msbuild /t:IncrementVersion /p:ProjectName=BrightSword.SwissKnife /p:Level=Patch /p:Commit=true
```

- Publish (recommended via GitHub Actions):
  - Run the `Publish packages in dependency order` workflow manually from Actions and supply the `package` input, or push a tag containing the package id (example: `v-BrightSword.SwissKnife-1.1.1`).


## 📚 Documentation

- See per-project docs under each project's `docs/` folder (e.g., `BrightSword.SwissKnife/docs/`). The GH-Pages workflow publishes `artifacts/docs` to `gh-pages`.

## 🔁 CI / CD

- CI uses `Build.proj` (`/t:CI`) as the canonical pipeline and publishes packages using the dependency-aware `publish-packages.yml` workflow.

## Files of interest
- `Build.proj` — centralized MSBuild targets used by local and CI builds.
- `version.props`, `versioning.targets` — centralized versioning and `IncrementVersion` target.
- `Directory.Build.props` — packaging defaults and `LangVersion`.
- `scripts/generate-package-dependencies.ps1` — dependency graph generator.
- `scripts/generate-docs.ps1` — placeholder docs generator used by GH-Pages workflow.
- `.github/workflows/*` — CI, regen-deps, publish-packages and gh-pages workflows.

If you want this README expanded into a contributor quickstart page under `docs/`, I can add that in a follow-up change.

---

## Package Highlights

### BrightSword.SwissKnife
Utility classes and extension methods that provide robust, dependency-light helpers for common programming tasks like reflection, validation, collections, and string manipulation.

### BrightSword.Crucible
Testing utilities for MSTest that make exception testing more expressive with the `ExpectException<T>` extension method.

### BrightSword.Feber
Automated delegate generation using LINQ Expression trees. Compose property-based operations (copying, printing, validation) once per type and execute them efficiently with cached compiled delegates.

**Performance**: 
- First call: ~10-100ms (reflection + expression building + JIT)
- Subsequent calls: <0.001ms (cached compiled delegate)

### BrightSword.Squid
Runtime type emission utilities for creating types dynamically using Reflection.Emit. Includes support for data transfer objects, behaviors, and advanced type creation scenarios.

## Run Samples and Tests

This repository targets .NET 10. Use the SDK pinned in `global.json`.

### Run the Feber Sample

```bash
# Build and run the sample
dotnet run --project BrightSword.Feber.SamplesApp/BrightSword.Feber.SamplesApp.csproj
```

### Run Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test BrightSword.SwissKnife.Tests/BrightSword.SwissKnife.Tests.csproj
```

## ðŸ“„ License

This project is licensed under the **Creative Commons Legal Code (CC0 1.0 Universal)**.

You are free to:
- **Share** â€” copy and redistribute the material in any medium or format
- **Adapt** â€” remix, transform, and build upon the material for any purpose, even commercially

Under the following terms:
- **Attribution** â€” You must give appropriate credit to BrightSword Technologies Pte Ltd, provide a link to the license, and indicate if changes were made.

See the full license at [https://creativecommons.org/licenses/by/4.0/](https://creativecommons.org/licenses/by/4.0/)

## ðŸ™ Acknowledgments

- Developed and maintained by [BrightSword Technologies Pte Ltd](https://brightsword.com)
- Copyright Â© BrightSword Technologies Pte Ltd, Singapore

## ðŸ“ž Support

- **Documentation**: [GitHub Pages](https://brightsword.github.io/BrightSword/)
- **Issues**: [GitHub Issues](https://github.com/brightsword/BrightSword/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brightsword/BrightSword/discussions)

---

**Built with â¤ï¸ by BrightSword Technologies**
