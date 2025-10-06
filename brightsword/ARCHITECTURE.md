# Architecture Overview

This document describes the architecture and design of the BrightSword monorepo and its packages.

## Monorepo Structure

### Repository Layout

```
BrightSword/
??? .github/                          # CI/CD and repository configuration
?   ??? workflows/                    # GitHub Actions workflows
?   ?   ??? ci.yml                   # Continuous Integration
?   ?   ??? pr-validation.yml        # Pull Request validation
?   ?   ??? release.yml              # Release and publishing
?   ?   ??? docs.yml                 # Documentation deployment
?   ??? copilot-instructions.md      # Development guidelines
?
??? BrightSword.SwissKnife/          # Utilities package
?   ??? docs/                        # Package documentation
?   ??? Properties/                  # Assembly info
?   ??? *.cs                         # Source files
?   ??? version.props                # Version configuration
?   ??? BrightSword.SwissKnife.csproj
?
??? BrightSword.Crucible/            # MSTest utilities package
?   ??? docs/                        # Package documentation
?   ??? *.cs                         # Source files
?   ??? version.props                # Version configuration
?   ??? BrightSword.Crucible.csproj
?
??? BrightSword.Feber/               # Expression builder package
?   ??? docs/                        # Package documentation
?   ??? Core/                        # Builder classes
?   ??? *.cs                         # Source files
?   ??? version.props                # Version configuration
?   ??? BrightSword.Feber.csproj
?
??? BrightSword.Squid/               # Type emission package
?   ??? docs/                        # Package documentation
?   ??? Properties/                  # Assembly info
?   ??? *.cs                         # Source files
?   ??? version.props                # Version configuration
?   ??? BrightSword.Squid.csproj
?
??? *.Tests/                         # Test projects
?   ??? *Tests.csproj
?
??? *.Samples/                       # Sample applications
?   ??? *.csproj
?
??? docs/                            # Monorepo documentation
?   ??? BUILD.md                     # Build guide
?   ??? CONTRIBUTING.md              # Contributing guidelines
?   ??? VERSIONING.md                # Versioning strategy
?   ??? CICD.md                      # CI/CD documentation
?   ??? ARCHITECTURE.md              # This file
?
??? artifacts/                       # Build outputs (gitignored)
?   ??? packages/                    # NuGet packages
?   ??? test-results/                # Test results
?
??? Build.proj                       # MSBuild orchestration
??? build.ps1                        # PowerShell build wrapper
??? increment-version.ps1            # Version management
??? Directory.Build.props            # Common MSBuild properties
??? Directory.Build.targets          # Common MSBuild targets
??? README.md                        # Main README
```

## Package Architecture

### BrightSword.SwissKnife

**Purpose**: Utility classes and extension methods for .NET development

**Key Components**:
- Extension methods for common types (strings, collections, enums)
- Reflection helpers and utilities
- Argument validation helpers
- Configuration management utilities

**Dependencies**:
- None (base package)
- .NET 10 BCL

**Design Principles**:
- Small, focused utilities
- Extension method pattern
- No external dependencies
- High test coverage
- Dependency-light and reusable

### BrightSword.Crucible

**Purpose**: Unit testing utilities for MSTest

**Key Components**:
- `ExceptionHelper.ExpectException<TException>()` - Fluent exception testing
- Extension methods for test assertions
- Helper methods for common test scenarios

**Dependencies**:
- `MSTest.TestFramework` (v2.2.10+)
- .NET 10 BCL

**Design Principles**:
- Expressive test syntax
- Fluent API for exception testing
- Clear error messages
- Minimal overhead

**Use Cases**:
- Testing exception scenarios
- Validating exception messages
- Inspecting exception properties

### BrightSword.Feber

**Purpose**: Automated delegate generation using LINQ Expression trees

**Key Components**:
- `ActionBuilder<TProto, TInstance>` - Unary action builders
- `ActionBuilder<TProto, TLeft, TRight>` - Binary action builders
- `FunctionBuilder<TProto, TInstance, TResult>` - Function builders for aggregation
- `OperationBuilder` base classes - Property scanning and expression composition

**Dependencies**:
- `BrightSword.SwissKnife`
- .NET 10 BCL (System.Linq.Expressions)

**Design Principles**:
- Performance through compilation
- Expression tree composition
- Lazy compilation with caching
- Property-based code generation
- One-time compilation cost, fast repeated execution

**Performance Characteristics**:
```
???????????????????
? First Call      ? ? Expensive (10-100ms: reflection, expression building, JIT)
???????????????????
? Compile & Cache ? ? One-time cost per type
???????????????????
? Subsequent Calls? ? Fast (<0.001ms: cached compiled delegate)
???????????????????
```

**Use Cases**:
- Property copying/cloning
- Object pretty-printing
- Property validation
- Hash code generation
- Property mapping and transformation

### BrightSword.Squid

**Purpose**: Runtime type emission utilities

**Key Components**:
- Type creators and builders
- Dynamic type generation using Reflection.Emit
- Behavior composition
- Data transfer object creation

**Dependencies**:
- `BrightSword.Feber`
- `BrightSword.SwissKnife`
- .NET 10 BCL (System.Reflection.Emit)

**Design Principles**:
- Runtime code generation
- Reflection.Emit for performance
- Extensible type creation pipeline
- Behavior-based composition

## Dependency Graph

```
????????????????????????
? BrightSword.Squid    ?
? (Type Emission)      ?
????????????????????????
           ? depends on
           ?
????????????????????????
? BrightSword.Feber    ?
? (Expression Builder) ?
????????????????????????
           ? depends on
           ?
????????????????????????
? BrightSword.         ?
? SwissKnife           ?
? (Utilities)          ?
????????????????????????

           (Independent)
????????????????????????
? BrightSword.         ?
? Crucible             ?
? (MSTest Utilities)   ?
????????????????????????
```

**Publishing Order**:
1. SwissKnife (base - no dependencies)
2. Crucible (independent - only depends on MSTest)
3. Feber (depends on SwissKnife)
4. Squid (depends on Feber and SwissKnife)

## Build System Architecture

### MSBuild Orchestration

```
Build.proj (orchestrator)
    ??? Defines targets (Build, Test, Pack, CI)
    ??? Discovers projects
    ??? Coordinates build order

Directory.Build.props
    ??? Common properties
    ??? NuGet metadata
    ??? Default values

Project/version.props
    ??? Package-specific versions
    ??? Package metadata
    ??? Override defaults

Directory.Build.targets
    ??? Common targets (post-build steps)

Project.csproj
    ??? Import version.props
    ??? Project-specific settings
    ??? Dependencies
```

### Build Flow

```
???????????
? Restore ? ? Download NuGet packages
???????????
     ?
???????????
?  Build  ? ? Compile source files
???????????
     ?
???????????
?  Test   ? ? Run unit/integration tests
???????????
     ?
???????????
?  Pack   ? ? Create NuGet packages
???????????
```

## CI/CD Architecture

### Workflow Triggers

```
Code Change
    ?
?????????????????????????????????
? Push to branch                ? ? CI Build
?????????????????????????????????
? Pull Request to main          ? ? PR Validation
?????????????????????????????????
? Tag (swissknife-v1.0.0)       ? ? Release
?????????????????????????????????
? Push to main                  ? ? Documentation
?????????????????????????????????
```

### Build Pipeline

```
????????????????????
? Checkout Code    ?
????????????????????
? Setup .NET SDK   ?
????????????????????
? Restore          ?
????????????????????
? Build            ?
????????????????????
? Test             ?
????????????????????
? Pack             ?
????????????????????
? Upload Artifacts ?
????????????????????
```

### Release Pipeline

```
Tag pushed (swissknife-v1.0.20)
    ?
????????????????????????????
? Extract version from tag ?
????????????????????????????
? Build & Test             ?
????????????????????????????
? Create NuGet packages    ?
????????????????????????????
? Publish to NuGet.org     ?
????????????????????????????
? Create GitHub Release    ?
????????????????????????????
```

## Version Management

### Version Flow

```
Developer
    ? ./increment-version.ps1
version.props
    ? MSBuild Import
Project.csproj
    ? MSBuild
Assembly (DLL)
    ? Pack
NuGet Package
    ? Publish
NuGet.org
```

### Version Storage

```xml
<!-- version.props -->
<VersionPrefix>1.0.19</VersionPrefix>

<!-- Directory.Build.props -->
<VersionSuffix Condition="'$(GITHUB_REF_TYPE)' != 'tag'">preview</VersionSuffix>

<!-- Result -->
Version: 1.0.19           (on tag)
Version: 1.0.19-preview   (on branch)
```

## Testing Strategy

### Test Organization

```
BrightSword.SwissKnife
    ? tests
BrightSword.SwissKnife.Tests
    ??? Unit Tests (fast, isolated)
    ??? Integration Tests (slower)
    ??? Test Utilities

BrightSword.Crucible
    ??? (Testing utilities - no separate test project)

BrightSword.Feber
    ? tests
BrightSword.Feber.Tests
    ??? Expression Builder Tests

BrightSword.Squid
    ? tests
BrightSword.Squid.Tests
    ??? Type Emission Tests
    ??? Remote Assembly Tests
    ??? Sample Usage Tests
```

### Test Pyramid

```
       ???????????
       ?   E2E   ? ? Few, slow, brittle
       ???????????
       ?  Integ  ? ? Some, medium speed
       ???????????
       ?  Unit   ? ? Many, fast, isolated
       ???????????
```

## Design Patterns

### Used Patterns

**Builder Pattern** (Feber)
- `ActionBuilder`, `FunctionBuilder`
- Expression tree composition
- Lazy compilation with caching

**Extension Method Pattern** (SwissKnife, Crucible)
- Extend existing types
- No inheritance required
- Discoverable via IntelliSense

**Factory Pattern** (Squid)
- Create types dynamically
- Type-specific handling
- Extensible registration

**Strategy Pattern** (All)
- Pluggable algorithms
- Runtime behavior selection
- Interface-based design

### SOLID Principles

**Single Responsibility**
- Each class has one reason to change
- Small, focused utilities

**Open/Closed**
- Open for extension via inheritance/interfaces
- Closed for modification via sealed implementations

**Liskov Substitution**
- Derived classes can replace base classes
- Interface implementations are substitutable

**Interface Segregation**
- Small, focused interfaces
- Clients depend only on what they use

**Dependency Inversion**
- Depend on abstractions, not concretions
- Use interfaces and abstract classes

## Performance Considerations

### BrightSword.Feber

**Expression Compilation**:
- First call per type: ~10-100ms (reflection + expression building + JIT compilation)
- Cached calls: ~0.001ms (direct delegate invocation)

**Memory Usage**:
- Expression tree: ~1-10KB per builder
- Compiled delegate: ~1KB
- Cache overhead: minimal

**Optimization Tips**:
```csharp
// Warm up at startup
var builder = new MyBuilder();
builder.Action(default!); // First call - expensive
// Subsequent calls - fast

// Or use Lazy<T>
private readonly Lazy<Action<T>> _action = new(() => BuildAction());
```

### BrightSword.Squid

**Type Generation**:
- One-time cost per type
- Reflection.Emit for high performance
- Cached type definitions

**Memory Usage**:
- Generated types: ~1-5KB per type
- Type cache: minimal overhead

### BrightSword.Crucible

**Exception Testing**:
- Negligible overhead
- Standard try-catch semantics
- No performance impact on production code

## Extension Points

### Adding New Packages

1. **Create project directory**:
   ```bash
   mkdir BrightSword.NewPackage
   ```

2. **Create version.props**:
   ```xml
   <Project>
     <PropertyGroup>
       <VersionPrefix>1.0.0</VersionPrefix>
       <PackageId>BrightSword.NewPackage</PackageId>
       <Description>Package description</Description>
       <IsPackable>true</IsPackable>
     </PropertyGroup>
   </Project>
   ```

3. **Create .csproj**:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <Import Project="version.props" />
     <!-- Project settings -->
   </Project>
   ```

4. **Update Build.proj**:
   ```xml
   <ItemGroup>
     <PackageProject Include="BrightSword.NewPackage\BrightSword.NewPackage.csproj" />
   </ItemGroup>
   ```

### Customizing Build

**Override properties in project**:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>false</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);CS1234</NoWarn>
</PropertyGroup>
```

**Add custom targets**:
```xml
<Target Name="CustomTarget" AfterTargets="Build">
  <Message Text="Custom build step" Importance="high" />
</Target>
```

## Future Architecture Considerations

### Planned Enhancements

1. **Modular Documentation**
   - DocFX integration
   - Auto-generated API docs
   - GitHub Pages publishing

2. **Performance Benchmarks**
   - BenchmarkDotNet integration
   - Automated performance testing
   - Historical performance tracking

3. **Code Analysis**
   - Additional analyzers
   - Custom Roslyn analyzers
   - Enforce coding standards

4. **Multi-targeting**
   - Support .NET Framework 4.8
   - Support .NET Standard 2.0
   - Cross-platform compatibility

---

## Related Documents

- [Build Guide](./BUILD.md)
- [Contributing Guidelines](./CONTRIBUTING.md)
- [Versioning Strategy](./VERSIONING.md)
- [CI/CD Pipeline](./CICD.md)
