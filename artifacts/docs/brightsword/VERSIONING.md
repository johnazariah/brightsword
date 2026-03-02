# Versioning Strategy

This document describes the versioning strategy for the BrightSword monorepo.

## Overview

The BrightSword monorepo contains multiple independent NuGet packages, each with its own version number:

- **BrightSword.SwissKnife** - Currently: 1.0.19 (Utilities and extensions)
- **BrightSword.Crucible** - Currently: 1.0.16 (MSTest utilities)
- **BrightSword.Feber** - Currently: 2.0.3 (Expression-based delegate generation)
- **BrightSword.Squid** - Currently: 1.0.0 (Runtime type emission)

## Semantic Versioning

All packages follow [Semantic Versioning 2.0.0](https://semver.org/):

```
MAJOR.MINOR.PATCH
```

### Version Components

- **MAJOR** - Incremented for incompatible API changes
- **MINOR** - Incremented for new backwards-compatible functionality
- **PATCH** - Incremented for backwards-compatible bug fixes

### Examples

```
1.0.0 -> 1.0.1  (Patch: Bug fix)
1.0.1 -> 1.1.0  (Minor: New feature)
1.1.0 -> 2.0.0  (Major: Breaking change)
```

## Version Storage

### version.props Files

Each package has a `version.props` file in its directory:

```xml
<!-- BrightSword.SwissKnife/version.props -->
<Project>
  <PropertyGroup>
    <VersionPrefix>1.0.19</VersionPrefix>
    <PackageId>BrightSword.SwissKnife</PackageId>
    <Description>Utility classes and extension methods for .NET development</Description>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```

```xml
<!-- BrightSword.Crucible/version.props -->
<Project>
  <PropertyGroup>
    <VersionPrefix>1.0.16</VersionPrefix>
    <PackageId>BrightSword.Crucible</PackageId>
    <Description>Unit testing utilities for MSTest</Description>
    <IsPackable>true</IsPackable>
  </PropertyGroup>
</Project>
```

### Directory.Build.props

Common version properties are defined in the root `Directory.Build.props`:

```xml
<PropertyGroup>
  <VersionPrefix>1.0.0</VersionPrefix>
  <VersionSuffix Condition="'$(GITHUB_REF_TYPE)' != 'tag'">preview</VersionSuffix>
</PropertyGroup>
```

Project-specific `version.props` files override `VersionPrefix`.

## Version Management

### Manual Version Updates

Edit the `version.props` file for the package:

```bash
# Open the file
code BrightSword.SwissKnife/version.props

# Update VersionPrefix
<VersionPrefix>1.0.20</VersionPrefix>
```

### Automated Version Increment

Use the `increment-version.ps1` script:

```powershell
# Increment patch version (default)
./increment-version.ps1 -Package BrightSword.SwissKnife
# 1.0.19 -> 1.0.20

# Increment minor version
./increment-version.ps1 -Package BrightSword.Feber -Component Minor
# 2.0.3 -> 2.1.0

# Increment major version
./increment-version.ps1 -Package BrightSword.Squid -Component Major
# 1.0.0 -> 2.0.0

# Increment all packages
./increment-version.ps1 -Package All -Component Patch
```

### Preview Versions

Preview/pre-release versions are automatically generated for non-tag builds:

```
1.0.19-preview
1.0.16-preview
2.0.3-preview
```

This is controlled by `VersionSuffix` in `Directory.Build.props`.

## Dependency Management

### Package Dependencies

Packages in the monorepo depend on each other:

```
BrightSword.Crucible
??? (Independent - only depends on MSTest)

BrightSword.Feber
??? depends on BrightSword.SwissKnife

BrightSword.Squid
??? depends on BrightSword.Feber
??? depends on BrightSword.SwissKnife
```

### Version Pinning

Project references use the built assemblies directly, not NuGet packages:

```xml
<!-- BrightSword.Feber.csproj -->
<ItemGroup>
  <ProjectReference Include="..\BrightSword.SwissKnife\BrightSword.SwissKnife.csproj" />
</ItemGroup>
```

When packed, the NuGet package references the minimum version:

```xml
<!-- Generated in .nupkg -->
<dependency id="BrightSword.SwissKnife" version="1.0.19" />
```

### Updating Dependent Packages

When updating a base package, consider updating dependent packages:

**Example**: Updating SwissKnife

1. **Update SwissKnife**:
   ```powershell
   ./increment-version.ps1 -Package BrightSword.SwissKnife -Component Minor
   # 1.0.19 -> 1.1.0
   ```

2. **Consider updating Feber** (depends on SwissKnife):
   ```powershell
   ./increment-version.ps1 -Package BrightSword.Feber -Component Patch
   # 2.0.3 -> 2.0.4
   ```

3. **Consider updating Squid** (depends on both):
   ```powershell
   ./increment-version.ps1 -Package BrightSword.Squid -Component Patch
   # 1.0.0 -> 1.0.1
   ```

4. **Crucible is independent** - Update only if needed for its own changes

## Release Process

### 1. Prepare Release

```bash
# Create release branch
git checkout -b release/swissknife-1.1.0

# Increment version
./increment-version.ps1 -Package BrightSword.SwissKnife -Component Minor

# Commit version change
git add BrightSword.SwissKnife/version.props
git commit -m "chore: bump SwissKnife to 1.1.0"

# Push and create PR
git push origin release/swissknife-1.1.0
```

### 2. Merge to Main

After PR approval, merge to `main`.

### 3. Create Release Tag

```bash
# Tag the release
git tag swissknife-v1.1.0

# Push tag
git push origin swissknife-v1.1.0
```

### 4. Automated Publishing

GitHub Actions will:
1. Detect the tag
2. Build the package
3. Run tests
4. Create NuGet package
5. Publish to NuGet.org
6. Create GitHub Release

## Version Naming Conventions

### Git Tags

Tags follow this pattern:

```
{package}-v{version}
```

Examples:
- `swissknife-v1.0.20`
- `crucible-v1.0.17`
- `feber-v2.1.0`
- `squid-v1.0.1`

### NuGet Packages

Package files follow NuGet conventions:

```
{PackageId}.{Version}.nupkg
{PackageId}.{Version}.snupkg
```

Examples:
- `BrightSword.SwissKnife.1.0.20.nupkg`
- `BrightSword.Crucible.1.0.17.nupkg`
- `BrightSword.Feber.2.1.0.snupkg`

### Git Branches

Branch names should be descriptive:

```
feature/add-new-functionality
fix/resolve-bug-123
release/swissknife-1.1.0
release/crucible-1.0.17
hotfix/critical-security-fix
```

## Breaking Changes

### Guidelines

**Breaking changes require a MAJOR version increment.**

Breaking changes include:
- Removing public APIs
- Changing method signatures
- Changing return types
- Renaming public types or members
- Changing behavior that consumers depend on

### Communication

For breaking changes:

1. **Document in PR**: Clearly mark as breaking change
2. **Update CHANGELOG**: List all breaking changes
3. **Migration Guide**: Provide guidance for upgrading
4. **Announcement**: Post in Discussions before release

### Example

```markdown
## Breaking Changes in 2.0.0

### Removed Obsolete Methods

The following methods marked obsolete in 1.x have been removed:

- `StringExtensions.OldMethod()` - Use `NewMethod()` instead

### Changed Method Signatures

`ProcessData` now requires a cancellation token:

```csharp
// Old (1.x)
public void ProcessData(string data)

// New (2.0)
public Task ProcessData(string data, CancellationToken cancellationToken)
```

### Migration Guide

1. Replace calls to `OldMethod()` with `NewMethod()`
2. Add `CancellationToken.None` to `ProcessData()` calls
3. Consider making your calls async
```

## Preview Releases

### Creating Previews

Preview releases use the format:

```
1.1.0-preview.1
1.1.0-preview.2
1.1.0-rc.1
```

To create a preview:

```powershell
# Manually edit version.props
<VersionPrefix>1.1.0</VersionPrefix>
<VersionSuffix>preview.1</VersionSuffix>
```

Or push to a feature branch - CI will automatically add `-preview` suffix.

### Publishing Previews

Previews are published to:
- GitHub Packages (automatic for all branches)
- NuGet.org (manual, for official previews)

## Version Query

### Current Versions

View current versions:

```bash
# Check version.props files
Get-Content BrightSword.*/version.props | Select-String "VersionPrefix"

# Check NuGet.org
dotnet package search BrightSword --exact-match
```

### Version History

See version history:
- GitHub Releases: https://github.com/brightsword/BrightSword/releases
- NuGet.org: Package page version history
- Git tags: `git tag -l`

## Best Practices

### When to Increment

- **After each release**: Increment to next development version
- **Before release**: Set final release version
- **For hotfixes**: Increment patch version

### Commit Messages

Include version in commit messages:

```
chore(swissknife): bump version to 1.0.20
chore(crucible): bump version to 1.0.17
fix(feber): critical bug - bump to 2.0.4
feat(squid): major refactor - bump to 2.0.0
```

### Testing

Before incrementing MAJOR version:
1. Run all tests
2. Check for breaking changes
3. Update documentation
4. Create migration guide

### Documentation

Update documentation with version-specific information:
- When features were added
- When features were deprecated
- When features were removed
- Minimum version requirements

## Package-Specific Version Considerations

### SwissKnife
- Most stable package with infrequent updates
- Breaking changes are rare
- Usually patch or minor version increments

### Crucible
- Independent of other BrightSword packages
- Can be versioned independently
- Tied to MSTest framework versions

### Feber
- Breaking changes should be carefully considered
- Expression tree API is sensitive
- Performance characteristics documented per version

### Squid
- Type emission API may evolve
- Tied to .NET runtime capabilities
- May have major versions for runtime changes

---

## Related Documents

- [Contributing Guidelines](./CONTRIBUTING.md)
- [Build Guide](./BUILD.md)
- [CI/CD Pipeline](./CICD.md)
