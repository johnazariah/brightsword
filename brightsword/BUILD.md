# Build and Development Guide

This guide covers building, testing, and developing the BrightSword monorepo.

## Prerequisites

### Required Tools

- **.NET 10 SDK** or later
  - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0)
  - Verify installation: `dotnet --version`

- **MSBuild** (included with .NET SDK)
  - Verify installation: `msbuild -version`

- **Git** for version control
  - Download from [git-scm.com](https://git-scm.com/)

### Optional Tools

- **Visual Studio 2024** or later (Windows)
- **Visual Studio Code** with C# extension (cross-platform)
- **JetBrains Rider** (cross-platform)

## Repository Structure

```
BrightSword/
??? .github/workflows/       # CI/CD GitHub Actions workflows
??? BrightSword.SwissKnife/  # Utilities package source
?   ??? docs/                # Package-specific documentation
?   ??? version.props        # Version configuration
?   ??? *.csproj             # Project file
??? BrightSword.Crucible/    # MSTest utilities package
?   ??? docs/                # Package-specific documentation
?   ??? version.props        # Version configuration
?   ??? *.csproj             # Project file
??? BrightSword.Feber/       # Expression builder package
?   ??? docs/                # Package-specific documentation
?   ??? Core/                # ActionBuilder, FunctionBuilder
?   ??? version.props        # Version configuration
?   ??? *.csproj             # Project file
??? BrightSword.Squid/       # Type emission package
?   ??? docs/                # Package-specific documentation
?   ??? version.props        # Version configuration
?   ??? *.csproj             # Project file
??? *.Tests/                 # Test projects
??? *.Samples/               # Sample applications
??? docs/                    # Monorepo documentation (this folder)
??? artifacts/               # Build output (gitignored)
?   ??? packages/            # NuGet packages
?   ??? test-results/        # Test results
??? Build.proj               # MSBuild orchestration script
??? build.ps1                # PowerShell build wrapper
??? increment-version.ps1    # Version management script
??? Directory.Build.props    # Common MSBuild properties
??? Directory.Build.targets  # Common MSBuild targets
```

## Building the Solution

### Quick Build

The easiest way to build is using the PowerShell script:

```powershell
# Build all projects (Release configuration)
./build.ps1

# Build with Debug configuration
./build.ps1 -Configuration Debug

# Clean build artifacts
./build.ps1 -Target Clean

# Rebuild everything
./build.ps1 -Target Rebuild
```

### Using MSBuild Directly

```bash
# Restore dependencies
msbuild Build.proj /t:Restore

# Build all packages
msbuild Build.proj /t:Build /p:Configuration=Release

# Build specific package
msbuild Build.proj /t:BuildPackages /p:Configuration=Release

# Clean
msbuild Build.proj /t:Clean
```

### Using .NET CLI

```bash
# Restore all projects
dotnet restore

# Build entire solution
dotnet build -c Release

# Build specific project
dotnet build BrightSword.SwissKnife/BrightSword.SwissKnife.csproj -c Release
dotnet build BrightSword.Crucible/BrightSword.Crucible.csproj -c Release
```

## Running Tests

### All Tests

```powershell
# Using build script
./build.ps1 -Target Test

# Using MSBuild
msbuild Build.proj /t:Test

# Using .NET CLI
dotnet test
```

### Specific Test Project

```bash
# SwissKnife tests
dotnet test BrightSword.SwissKnife.Tests/BrightSword.SwissKnife.Tests.csproj

# Feber tests
dotnet test BrightSword.Feber.Tests/BrightSword.Feber.Tests.csproj

# Squid tests
dotnet test BrightSword.Squid.Tests/BrightSword.Squid.Tests.csproj
```

### Test Results

Test results are saved to `artifacts/test-results/` in TRX format.

## Creating NuGet Packages

### Pack All Packages

```powershell
# Using build script
./build.ps1 -Target Pack

# Using MSBuild
msbuild Build.proj /t:Pack /p:Configuration=Release
```

### Pack Specific Package

```powershell
# Using build script
./build.ps1 -Target Pack -Package BrightSword.SwissKnife
./build.ps1 -Target Pack -Package BrightSword.Crucible

# Using MSBuild
msbuild Build.proj /t:PackSingle /p:Package=BrightSword.Feber
```

### Package Output

NuGet packages are created in `artifacts/packages/` with the naming convention:
- `PackageId.Version.nupkg` - Main package
- `PackageId.Version.snupkg` - Symbol package

## Development Workflow

### 1. Clone the Repository

```bash
git clone https://github.com/brightsword/BrightSword.git
cd BrightSword
```

### 2. Create a Feature Branch

```bash
git checkout -b feature/my-new-feature
```

### 3. Make Changes

Edit code, add tests, update documentation.

### 4. Build and Test Locally

```powershell
# Build and run tests
./build.ps1 -Target CI
```

### 5. Commit Changes

```bash
git add .
git commit -m "feat: add new feature"
```

### 6. Push and Create Pull Request

```bash
git push origin feature/my-new-feature
```

Then create a Pull Request on GitHub.

## Build Configuration

### Directory.Build.props

Common properties for all projects:

- **TargetFramework**: `net10.0`
- **LangVersion**: `latest`
- **Nullable**: `enable`
- **Documentation**: Enabled with XML comments
- **SourceLink**: Enabled for debugging

### version.props Files

Each package has its own `version.props` file:

```xml
<Project>
  <PropertyGroup>
    <VersionPrefix>1.0.0</VersionPrefix>
    <PackageId>BrightSword.PackageName</PackageId>
    <Description>Package description</Description>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```

### Build Targets

The `Build.proj` file defines these targets:

- **Restore**: Restore NuGet packages
- **Build**: Build all package projects
- **BuildTests**: Build test projects
- **Test**: Run all tests
- **Pack**: Create NuGet packages
- **PackSingle**: Create a specific package
- **Clean**: Remove build artifacts
- **Rebuild**: Clean + Build
- **CI**: Full CI pipeline (Clean + Restore + Build + Test + Pack)

## Package-Specific Build Notes

### BrightSword.SwissKnife
- Base package with no dependencies
- Builds quickly
- High test coverage

### BrightSword.Crucible
- Depends only on MSTest.TestFramework
- Independent of other BrightSword packages
- No separate test project (it is a testing utility)

### BrightSword.Feber
- Depends on SwissKnife
- Uses System.Linq.Expressions heavily
- Expression tree compilation tested extensively
- Sample app available: `BrightSword.Feber.SamplesApp`

### BrightSword.Squid
- Depends on both Feber and SwissKnife
- Uses Reflection.Emit
- Multiple test assemblies for remote type scenarios
- Build order is important

## Running Sample Applications

### Feber Sample App

```bash
# Run the expression builder demo
dotnet run --project BrightSword.Feber.SamplesApp/BrightSword.Feber.SamplesApp.csproj
```

This demonstrates:
- Lazy action builder compilation
- Property-based expression generation
- Performance characteristics

## Troubleshooting

### Build Fails with Missing SDK

```
error : The project file requires .NET SDK version 10.0.0
```

**Solution**: Install .NET 10 SDK or update `global.json` to match your installed SDK.

### Tests Fail to Run

```
The test source file "...dll" provided was not found.
```

**Solution**: Build the test projects first:

```powershell
./build.ps1 -Target BuildTests
./build.ps1 -Target Test
```

### Package References Not Resolved

```
error NU1101: Unable to find package
```

**Solution**: Restore NuGet packages:

```powershell
./build.ps1 -Target Restore
# or
dotnet restore
```

### Clean Build Issues

If you encounter strange build issues:

```powershell
# Clean everything
./build.ps1 -Target Clean

# Delete bin and obj directories
Get-ChildItem -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force

# Rebuild
./build.ps1 -Target Rebuild
```

### Expression Compilation Errors (Feber)

If you get errors related to expression parameter binding:

- Ensure you're using the correct `ParameterExpression` instances
- Check that lambda expressions reference the correct parameters
- Review the builder base class documentation

## Performance Tips

### Parallel Builds

MSBuild supports parallel builds by default. To control:

```bash
# Use maximum parallelism
msbuild Build.proj /m

# Limit to 4 parallel processes
msbuild Build.proj /m:4
```

### Incremental Builds

MSBuild performs incremental builds automatically. To force a full rebuild:

```powershell
./build.ps1 -Target Rebuild
```

### Binary Log

For detailed build diagnostics:

```bash
msbuild Build.proj /bl:build.binlog
```

View the binary log with the [MSBuild Structured Log Viewer](http://msbuildlog.com/).

## Testing Best Practices

### Writing Tests

- **SwissKnife**: Test extension methods with various inputs including edge cases
- **Crucible**: Use `ExpectException<T>()` for exception testing
- **Feber**: Test expression building separately from compilation
- **Squid**: Test type emission with various interface scenarios

### Test Organization

```
MyTests.cs
??? Constructor Tests
??? Method Tests
?   ??? Happy Path
?   ??? Edge Cases
?   ??? Error Cases
??? Property Tests
```

### Using Crucible in Tests

```csharp
using BrightSword.Crucible;

[TestMethod]
public void Method_WithInvalidInput_ShouldThrow()
{
    var exception = new Action(() => myObject.Method(null))
        .ExpectException<ArgumentNullException>("Expected message");
    
    Assert.AreEqual("paramName", exception.ParamName);
}
```

## Next Steps

- [Contributing Guidelines](./CONTRIBUTING.md)
- [Versioning Strategy](./VERSIONING.md)
- [CI/CD Pipeline](./CICD.md)
- [Architecture Overview](./ARCHITECTURE.md)
