---
layout: default
title: "VERSIONING"
parent: "Guides"
nav_order: 11
---

# Versioning Strategy

This document describes the versioning strategy for the BrightSword monorepo.

## Overview

The BrightSword monorepo ships all packages as a **unified suite** at a single shared version. All 5 packages are always released together at the same version number:

- **BrightSword.SwissKnife** — Utilities and extensions
- **BrightSword.Crucible** — MSTest utilities
- **BrightSword.Feber** — Expression-based delegate generation
- **BrightSword.Squid** — Runtime type emission
- **BrightSword.Packages** — Metapackage bundling all libraries

**Current version: 2.0.0**

## Semantic Versioning

All packages follow [Semantic Versioning 2.0.0](https://semver.org/):

```
MAJOR.MINOR.PATCH
```

### Version Components

- **MAJOR** — Incremented for incompatible API changes in any package
- **MINOR** — Incremented for new backwards-compatible functionality in any package
- **PATCH** — Incremented for backwards-compatible bug fixes in any package

### Examples

```
2.0.0 -> 2.0.1  (Patch: Bug fix in any package)
2.0.1 -> 2.1.0  (Minor: New feature in any package)
2.1.0 -> 3.0.0  (Major: Breaking change in any package)
```

## Version Storage

### Root version.props

All packages share a **single `VersionPrefix`** defined in the root `version.props`:

```xml
<!-- version.props (repo root) -->
<Project>
  <PropertyGroup>
    <VersionPrefix>2.0.0</VersionPrefix>
  </PropertyGroup>
</Project>
```

There are **no per-project `version.props` files**. Every package inherits its version from this single root file via `Directory.Build.props`.

### Preview Suffix

A `-preview` suffix is automatically added for non-tag builds:

```xml
<!-- Directory.Build.props -->
<VersionSuffix Condition="'$(GITHUB_REF_TYPE)' != 'tag'">preview</VersionSuffix>
```

This means:
- Tag builds (releases): `2.0.0`
- Branch/PR builds: `2.0.0-preview`

## Version Management

### Automated Version Increment (Recommended)

Use the MSBuild `IncrementVersion` target to bump the unified version:

```bash
# Increment patch (2.0.0 -> 2.0.1)
dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Patch

# Increment minor (2.0.0 -> 2.1.0)
dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Minor

# Increment major (2.0.0 -> 3.0.0)
dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Major

# Increment and commit locally (be careful):
dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Patch /p:Commit=true
```

This edits the single `VersionPrefix` in root `version.props`, which applies to all 5 packages.

### PowerShell Script

The `scripts/increment-version.ps1` script provides an alternative interface:

```powershell
./scripts/increment-version.ps1
```

### Manual Version Updates

Edit the root `version.props` directly:

```bash
code version.props
# Change <VersionPrefix>2.0.0</VersionPrefix> to <VersionPrefix>2.0.1</VersionPrefix>
```

## Dependency Management

### Package Dependencies

Packages in the monorepo depend on each other via project references:

```
BrightSword.Crucible
  (Independent - only depends on MSTest)

BrightSword.Feber
  depends on BrightSword.SwissKnife

BrightSword.Squid
  depends on BrightSword.Feber
  depends on BrightSword.SwissKnife

BrightSword.Packages (metapackage)
  depends on all 4 libraries
```

### Build Order

The dependency graph determines the **build and publish order**, but not version management (since all packages share the same version):

1. SwissKnife (base - no dependencies)
2. Crucible (independent - only depends on MSTest)
3. Feber (depends on SwissKnife)
4. Squid (depends on Feber and SwissKnife)
5. Packages (metapackage - depends on all)

### Version Pinning

Project references use built assemblies directly. When packed, the NuGet dependency version matches the unified suite version:

```xml
<!-- Generated in .nupkg -->
<dependency id="BrightSword.SwissKnife" version="2.0.0" />
```

## Release Process

### 1. Prepare Release

```bash
# Create release branch
git checkout -b release/v2.0.1

# Increment version
dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Patch

# Commit version change
git add version.props
git commit -m "chore: bump version to 2.0.1"

# Push and create PR
git push origin release/v2.0.1
```

### 2. Merge to Main

After PR approval, merge to `main`.

### 3. Create Release Tag

```bash
# Tag the release
git tag v2.0.1

# Push tag to trigger automated publish
git push origin v2.0.1
```

### 4. Automated Publishing

When a `v*` tag is pushed, the `publish-packages.yml` workflow:
1. Detects the tag
2. Builds all packages
3. Runs all tests
4. Packs all 5 NuGet packages
5. Publishes to NuGet.org in dependency order
6. Creates GitHub Release

## Version Naming Conventions

### Git Tags

Tags follow a simple unified pattern:

```
v{version}
```

Examples:
- `v2.0.0`
- `v2.0.1`
- `v2.1.0`

### NuGet Packages

All packages share the same version:

```
BrightSword.SwissKnife.2.0.1.nupkg
BrightSword.Crucible.2.0.1.nupkg
BrightSword.Feber.2.0.1.nupkg
BrightSword.Squid.2.0.1.nupkg
BrightSword.Packages.2.0.1.nupkg
```

### Git Branches

Branch names should be descriptive:

```
feature/add-new-functionality
fix/resolve-bug-123
release/v2.0.1
hotfix/critical-security-fix
```

## Breaking Changes

### Guidelines

**Breaking changes in any package require a MAJOR version increment for the entire suite.**

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
## Breaking Changes in 3.0.0

### Removed Obsolete Methods

The following methods marked obsolete in 2.x have been removed:

- `StringExtensions.OldMethod()` - Use `NewMethod()` instead

### Changed Method Signatures

`ProcessData` now requires a cancellation token:

`csharp
// Old (2.x)
public void ProcessData(string data)

// New (3.0)
public Task ProcessData(string data, CancellationToken cancellationToken)
`

### Migration Guide

1. Replace calls to `OldMethod()` with `NewMethod()`
2. Add `CancellationToken.None` to `ProcessData()` calls
3. Consider making your calls async
```

## Preview Releases

### Automatic Previews

Non-tag builds automatically receive a `-preview` suffix:

```
2.0.1-preview
```

This is controlled by `VersionSuffix` in `Directory.Build.props`. Pushing to any branch or creating a PR produces preview packages.

### Manual Previews

For numbered preview releases, edit `version.props`:

```xml
<VersionPrefix>2.1.0</VersionPrefix>
<!-- and in Directory.Build.props set: -->
<VersionSuffix>preview.1</VersionSuffix>
```

### Publishing Previews

Previews are published to:
- GitHub Packages (automatic for all branches)
- NuGet.org (manual, for official previews)

## Version Query

### Current Version

View the current unified version:

```bash
# Check root version.props
Get-Content version.props | Select-String "VersionPrefix"

# Check NuGet.org
dotnet package search BrightSword --exact-match
```

### Version History

See version history:
- GitHub Releases: https://github.com/johnazariah/brightsword/releases
- NuGet.org: Package page version history
- Git tags: `git tag -l "v*"`

## Best Practices

### When to Increment

- **After each release**: Increment to next development version
- **Before release**: Set final release version
- **For hotfixes**: Increment patch version

### Commit Messages

Include version in commit messages:

```
chore: bump version to 2.0.1
fix: critical bug - bump to 2.0.2
feat: new feature - bump to 2.1.0
```

### Testing

Before incrementing MAJOR version:
1. Run all 508 tests
2. Check for breaking changes across all packages
3. Update documentation
4. Create migration guide

### Documentation

Update documentation with version-specific information:
- When features were added
- When features were deprecated
- When features were removed
- Minimum version requirements

---

## Related Documents

- [Contributing Guidelines](./CONTRIBUTING.md)
- [Build Guide](./BUILD.md)
- [CI/CD Pipeline](./CICD.md)
