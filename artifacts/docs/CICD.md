# CI/CD Pipeline Documentation

This document describes the Continuous Integration and Continuous Deployment (CI/CD) pipelines for the BrightSword monorepo.

## Overview

The CI/CD system uses **GitHub Actions** with three main workflows:

1. **CI Build** (`ci.yml`) - Builds and tests on every push
2. **Pull Request Validation** (`pr-validation.yml`) - Validates PRs to main
3. **Release** (`release.yml`) - Publishes packages to NuGet.org

## Workflows

### 1. CI Build Workflow

**File**: `.github/workflows/ci.yml`

**Triggers**:
- Push to any branch
- Pull requests to `main` or `develop`
- Manual trigger via workflow_dispatch

**Steps**:
1. Checkout code
2. Setup .NET 10 SDK
3. Restore dependencies
4. Build solution
5. Run tests
6. Create NuGet packages
7. Upload artifacts

**Outputs**:
- Test results (TRX files)
- NuGet packages (.nupkg files)
- Symbol packages (.snupkg files)

**Example Run**:
```bash
# Triggered automatically on push
git push origin feature/my-feature

# Or manually via GitHub UI:
# Actions -> CI Build -> Run workflow
```

### 2. Pull Request Validation

**File**: `.github/workflows/pr-validation.yml`

**Triggers**:
- Pull requests to `main` branch
- PR opened, synchronized, or reopened

**Steps**:
1. Checkout code with full history
2. Setup .NET 10 SDK
3. Restore dependencies
4. Build solution
5. Run tests with coverage
6. Check for breaking changes (placeholder)
7. Verify version updates (placeholder)
8. Create packages for validation
9. Post PR comment with results

**Purpose**:
- Ensure PRs don't break main
- Validate code quality
- Check for breaking changes
- Verify appropriate version increments

### 3. Release Workflow

**File**: `.github/workflows/release.yml`

**Triggers**:
- Tags matching `v*.*.*` pattern
- Manual workflow dispatch

**Steps**:
1. Checkout code
2. Extract version from tag
3. Setup .NET 10 SDK
4. Restore dependencies
5. Build solution
6. Run tests
7. Create packages
8. Publish to NuGet.org
9. Create GitHub Release

**Example**:
```bash
# Tag a release
git tag swissknife-v1.0.20
git push origin swissknife-v1.0.20

# This triggers the release workflow
```

## Branch Strategy

### Branch Types

```
main                  # Protected, requires PR, auto-deploys
??? develop           # Integration branch
??? feature/*         # New features
??? fix/*             # Bug fixes
??? release/*         # Release preparation
??? hotfix/*          # Production hotfixes
```

### Branch Protection Rules

**`main` branch**:
- Requires pull request reviews (1 approver)
- Requires status checks to pass
- Requires branches to be up to date
- No force pushes
- No deletions

**`develop` branch**:
- Allows direct pushes (for integration)
- Requires status checks to pass
- No force pushes

## Build Matrix

Currently, builds run on:
- **OS**: ubuntu-latest
- **.NET**: 10.0.x

To expand to multiple platforms:

```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet: ['10.0.x']
```

## Secrets and Variables

### Required Secrets

Configure these in GitHub repository settings:

**`NUGET_API_KEY`**
- **Purpose**: Publishing packages to NuGet.org
- **How to get**: Create API key at https://www.nuget.org/account/apikeys
- **Permissions**: Push packages
- **Scope**: Specific packages (BrightSword.*)

### Automatic Secrets

**`GITHUB_TOKEN`**
- **Purpose**: GitHub API access, creating releases
- **Provided**: Automatically by GitHub Actions
- **Permissions**: Read/write to repository

### Optional Secrets

**`CODECOV_TOKEN`** (if using Codecov)
- **Purpose**: Upload code coverage reports
- **How to get**: https://codecov.io/

## Environment Variables

Set in workflow files:

```yaml
env:
  DOTNET_VERSION: '10.0.x'
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true
  CI: true
```

## Artifacts

### Test Results

**Location**: `artifacts/test-results/`
**Format**: TRX (Visual Studio Test Results)
**Retention**: 30 days

View in GitHub Actions:
1. Go to workflow run
2. Click "Artifacts"
3. Download `test-results`

### NuGet Packages

**Location**: `artifacts/packages/`
**Format**: .nupkg and .snupkg files
**Retention**: 30 days

Download from:
- GitHub Actions artifacts
- GitHub Releases (for tagged releases)
- NuGet.org (for published packages)

## Publishing Packages

### To GitHub Packages

**Automatic**: On every push to feature branches

```yaml
- name: Publish to GitHub Packages
  run: |
    dotnet nuget push "artifacts/packages/*.nupkg" \
      --source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" \
      --api-key ${{ secrets.GITHUB_TOKEN }} \
      --skip-duplicate
```

**Consuming GitHub Packages**:

```bash
# Add package source
dotnet nuget add source https://nuget.pkg.github.com/brightsword/index.json \
  --name github \
  --username YOUR-GITHUB-USERNAME \
  --password YOUR-GITHUB-TOKEN \
  --store-password-in-clear-text

# Install package
dotnet add package BrightSword.SwissKnife --version 1.0.20-preview
```

### To NuGet.org

**Automatic**: On version tags (e.g., `swissknife-v1.0.20`)

```yaml
- name: Publish to NuGet.org
  run: |
    dotnet nuget push "artifacts/packages/*.nupkg" \
      --source https://api.nuget.org/v3/index.json \
      --api-key ${{ secrets.NUGET_API_KEY }} \
      --skip-duplicate
```

**Manual Publishing**:

```bash
# Build and pack
./build.ps1 -Target Pack

# Publish
dotnet nuget push "artifacts/packages/*.nupkg" \
  --source https://api.nuget.org/v3/index.json \
  --api-key YOUR-NUGET-API-KEY
```

## Handling Dependencies

### Package Dependency Chain

```
SwissKnife (base)
    ?
Feber (depends on SwissKnife)
    ?
Squid (depends on Feber and SwissKnife)
```

### Publishing Dependent Packages

When publishing a base package, dependent packages should be republished:

**Scenario**: Update SwissKnife

1. **Increment SwissKnife version**:
   ```powershell
   ./increment-version.ps1 -Package BrightSword.SwissKnife -Component Minor
   ```

2. **Tag and release SwissKnife**:
   ```bash
   git tag swissknife-v1.1.0
   git push origin swissknife-v1.1.0
   ```

3. **Update and release Feber**:
   ```powershell
   ./increment-version.ps1 -Package BrightSword.Feber -Component Patch
   git tag feber-v2.0.4
   git push origin feber-v2.0.4
   ```

4. **Update and release Squid**:
   ```powershell
   ./increment-version.ps1 -Package BrightSword.Squid -Component Patch
   git tag squid-v1.0.1
   git push origin squid-v1.0.1
   ```

### Future: Automated Cascade Publishing

A future enhancement could automate dependent package publishing:

```yaml
# Detect changes to base packages
# Automatically increment and publish dependents
- name: Publish dependent packages
  if: contains(github.ref, 'swissknife')
  run: |
    # Increment Feber
    # Tag and trigger Feber release
    # Increment Squid
    # Tag and trigger Squid release
```

## Monitoring and Logs

### Viewing Workflow Runs

1. Go to repository on GitHub
2. Click "Actions" tab
3. Select workflow (CI Build, PR Validation, Release)
4. Click on a specific run

### Understanding Logs

Each step shows:
- Start time
- Duration
- Exit code
- Full output

**Example**: Build logs show:
```
Restore succeeded
Building BrightSword.SwissKnife
Building BrightSword.Feber
Building BrightSword.Squid
Build succeeded
```

### Debugging Failures

**Build failure**:
1. Check "Build solution" step
2. Look for compilation errors
3. Review changed files

**Test failure**:
1. Check "Run tests" step
2. Download test-results artifact
3. Open TRX file in Visual Studio

**Publish failure**:
1. Check "Publish to NuGet.org" step
2. Verify NUGET_API_KEY is set
3. Check for duplicate version (use --skip-duplicate)

## Performance Optimization

### Caching

**NuGet packages**:
```yaml
- uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

**Build output**:
```yaml
- uses: actions/cache@v3
  with:
    path: |
      **/obj
      **/bin
    key: ${{ runner.os }}-build-${{ hashFiles('**/*.csproj') }}
```

### Parallel Execution

Matrix strategy for parallel builds:

```yaml
strategy:
  matrix:
    package: [SwissKnife, Feber, Squid]
steps:
  - name: Build ${{ matrix.package }}
    run: msbuild Build.proj /t:PackSingle /p:Package=BrightSword.${{ matrix.package }}
```

## Security

### Dependency Scanning

**Dependabot** (configured in `.github/dependabot.yml`):

```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
```

### Code Scanning

**CodeQL Analysis**:

```yaml
name: CodeQL
on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 0 * * 0'  # Weekly on Sunday
```

## Notifications

### Status Checks

Pull requests show status checks:
- ? CI Build passing
- ? Tests passing
- ? Code coverage adequate
- ?? Breaking changes detected

### Email Notifications

GitHub sends emails for:
- Workflow failures
- PR checks complete
- New releases published

### Badges

Add to README.md:

```markdown
[![CI Build](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml/badge.svg)](https://github.com/brightsword/BrightSword/actions/workflows/ci.yml)
[![Release](https://github.com/brightsword/BrightSword/actions/workflows/release.yml/badge.svg)](https://github.com/brightsword/BrightSword/actions/workflows/release.yml)
```

## Troubleshooting

### Common Issues

**Issue**: "No packages found to push"
```bash
# Solution: Check artifacts/packages/ directory exists
ls artifacts/packages/*.nupkg
```

**Issue**: "Package already exists"
```bash
# Solution: Increment version or use --skip-duplicate
./increment-version.ps1 -Package BrightSword.SwissKnife
```

**Issue**: "Authentication failed"
```bash
# Solution: Verify NUGET_API_KEY secret is set correctly
# Go to: Settings -> Secrets and variables -> Actions
```

**Issue**: "Tests failed"
```bash
# Solution: Run tests locally first
./build.ps1 -Target Test

# Review test output
code artifacts/test-results/*.trx
```

## Future Enhancements

### Planned Improvements

1. **Code Coverage Reporting**
   - Integrate Codecov or Coveralls
   - Set minimum coverage thresholds
   - Block PRs with coverage drop

2. **Performance Testing**
   - Benchmark tests in CI
   - Track performance over time
   - Alert on regressions

3. **Automated Dependency Updates**
   - Auto-merge Dependabot PRs
   - Test dependency updates
   - Create PRs for major updates

4. **Multi-platform Testing**
   - Test on Windows, Linux, macOS
   - Test on multiple .NET versions
   - Ensure cross-platform compatibility

5. **Release Notes Generation**
   - Auto-generate from commit messages
   - Include PR links
   - Categorize changes

---

## Related Documents

- [Build Guide](./BUILD.md)
- [Contributing Guidelines](./CONTRIBUTING.md)
- [Versioning Strategy](./VERSIONING.md)
