# BrightSword Monorepo

A collection of high-quality .NET libraries for utilities, testing, automated code generation, and advanced serialization.

[![CI Build](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml/badge.svg)](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml)
[![License: CC BY 4.0](https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by/4.0/)

## 📦 Packages

This monorepo contains the following NuGet packages:

| Package | Version | Description |
|---------|---------|-------------|
| [BrightSword.SwissKnife](./BrightSword.SwissKnife) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.SwissKnife.svg)](https://www.nuget.org/packages/BrightSword.SwissKnife/) | Utility classes and extension methods for .NET development |
| [BrightSword.Crucible](./BrightSword.Crucible) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Crucible.svg)](https://www.nuget.org/packages/BrightSword.Crucible/) | Unit testing utilities for MSTest |
| [BrightSword.Feber](./BrightSword.Feber) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Feber.svg)](https://www.nuget.org/packages/BrightSword.Feber/) | Automated delegate generation using Expression trees |
| [BrightSword.Squid](./BrightSword.Squid) | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Squid.svg)](https://www.nuget.org/packages/BrightSword.Squid/) | Runtime type emission utilities |

## 🚀 Quick Start

### Installation

Install the packages via NuGet Package Manager or .NET CLI:

```bash
# SwissKnife - Utilities and helpers
dotnet add package BrightSword.SwissKnife

# Crucible - MSTest utilities
dotnet add package BrightSword.Crucible

# Feber - Expression-based code generation
dotnet add package BrightSword.Feber

# Squid - Runtime type emission
dotnet add package BrightSword.Squid
```

### Basic Usage

```csharp
using BrightSword.SwissKnife;
using BrightSword.Crucible;
using BrightSword.Feber.Core;
using BrightSword.Squid;

// Use the libraries in your code
```

## 🏗️ Building from Source

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- MSBuild (included with .NET SDK)

### Build Commands

```bash
# Build all projects
./build.ps1

# Build specific package
./build.ps1 -Package BrightSword.SwissKnife

# Run tests
./build.ps1 -Target Test

# Create NuGet packages
./build.ps1 -Target Pack

# Full CI build (clean, build, test, pack)
./build.ps1 -Target CI

# Clean build artifacts
./build.ps1 -Target Clean
```

### Using MSBuild Directly

```bash
# Build
msbuild Build.proj /t:Build /p:Configuration=Release

# Test
msbuild Build.proj /t:Test

# Pack
msbuild Build.proj /t:Pack

# Pack specific package
msbuild Build.proj /t:PackSingle /p:Package=BrightSword.SwissKnife
```

## 📚 Documentation

- [Build and Development Guide](./docs/BUILD.md)
- [Contributing Guidelines](./docs/CONTRIBUTING.md)
- [Versioning Strategy](./docs/VERSIONING.md)
- [CI/CD Pipeline](./docs/CICD.md)
- [Architecture Overview](./docs/ARCHITECTURE.md)

### Package Documentation

- [BrightSword.SwissKnife Documentation](./BrightSword.SwissKnife/docs/)
- [BrightSword.Crucible Documentation](./BrightSword.Crucible/docs/)
- [BrightSword.Feber Documentation](./BrightSword.Feber/docs/)
- [BrightSword.Squid Documentation](./BrightSword.Squid/docs/)

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](./docs/CONTRIBUTING.md) for details.

### Development Workflow

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Versioning

We use [Semantic Versioning](https://semver.org/). To increment versions:

```bash
# Increment patch version (1.0.0 -> 1.0.1)
./increment-version.ps1 -Package BrightSword.SwissKnife

# Increment minor version (1.0.0 -> 1.1.0)
./increment-version.ps1 -Package BrightSword.Feber -Component Minor

# Increment major version (1.0.0 -> 2.0.0)
./increment-version.ps1 -Package BrightSword.Squid -Component Major
```

## 📋 Project Structure

```
BrightSword/
├── .github/
│   └── workflows/          # CI/CD pipelines
├── BrightSword.SwissKnife/ # Utilities package
├── BrightSword.Crucible/   # MSTest utilities package
├── BrightSword.Feber/      # Expression builder package
├── BrightSword.Squid/      # Type emission package
├── docs/                   # Monorepo documentation
├── Build.proj              # MSBuild build script
├── build.ps1               # PowerShell build script
├── increment-version.ps1   # Version management script
├── Directory.Build.props   # Common MSBuild properties
└── Directory.Build.targets # Common MSBuild targets
```

## 🔄 CI/CD

The repository uses GitHub Actions for continuous integration and deployment:

- **CI Build** - Runs on all pushes and pull requests
- **PR Validation** - Validates pull requests to main branch
- **Release** - Publishes packages to NuGet.org on version tags

See [CI/CD Documentation](./docs/CICD.md) for more details.

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

## 📄 License

This project is licensed under the **Creative Commons Legal Code (CC0 1.0 Universal)**.

You are free to:
- **Share** — copy and redistribute the material in any medium or format
- **Adapt** — remix, transform, and build upon the material for any purpose, even commercially

Under the following terms:
- **Attribution** — You must give appropriate credit to BrightSword Technologies Pte Ltd, provide a link to the license, and indicate if changes were made.

See the full license at [https://creativecommons.org/licenses/by/4.0/](https://creativecommons.org/licenses/by/4.0/)

## 🙏 Acknowledgments

- Developed and maintained by [BrightSword Technologies Pte Ltd](https://brightsword.com)
- Copyright © BrightSword Technologies Pte Ltd, Singapore

## 📞 Support

- **Documentation**: [GitHub Pages](https://brightsword.github.io/BrightSword/)
- **Issues**: [GitHub Issues](https://github.com/brightsword/BrightSword/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brightsword/BrightSword/discussions)

---

**Built with ❤️ by BrightSword Technologies**
