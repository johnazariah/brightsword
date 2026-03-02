# GitHub Pages Documentation Setup Guide

This guide explains how to set up and use GitHub Pages documentation for the BrightSword monorepo.

## Overview

The BrightSword monorepo uses **DocFX** to generate comprehensive documentation that includes:
- API documentation extracted from XML comments
- Package-specific guides and tutorials
- Monorepo-level documentation
- Cross-referenced documentation with links to Microsoft docs

## Architecture

```
Documentation Sources:
??? XML Comments (in source code)
??? Package docs/ folders
?   ??? BrightSword.SwissKnife/docs/
?   ??? BrightSword.Crucible/docs/
?   ??? BrightSword.Feber/docs/
?   ??? BrightSword.Squid/docs/
??? Monorepo docs/ folder

        ? DocFX Processing

Generated Site (_site/):
??? API Reference (from XML)
??? Guides and Tutorials
??? Package Documentation

        ? GitHub Actions

GitHub Pages:
https://brightsword.github.io/BrightSword/
```

## Setup Instructions

### 1. Enable GitHub Pages

**In your GitHub repository:**

1. Navigate to **Settings** ? **Pages**
2. Under **Source**, select **GitHub Actions**
3. Click **Save**

That's it! GitHub Pages is now configured.

### 2. Trigger Documentation Build

The documentation builds automatically when you:

- **Push to main branch** - Updates live documentation
- **Manually trigger** - Use GitHub Actions UI

**To manually trigger:**

1. Go to **Actions** tab in GitHub
2. Select **Documentation** workflow
3. Click **Run workflow**
4. Select branch: `main`
5. Click **Run workflow**

### 3. View Your Documentation

After the workflow completes (usually 2-5 minutes):

**Your documentation will be available at:**
```
https://brightsword.github.io/BrightSword/
```

## Local Development

### Install DocFX

```powershell
# Install DocFX as a global tool
dotnet tool install -g docfx
```

### Build Documentation Locally

```powershell
# Build documentation
.\build-docs.ps1

# Build and preview locally
.\build-docs.ps1 -Serve
```

Then open: http://localhost:8080

### Manual Build Commands

```bash
# Build only
docfx docfx.json

# Build and serve
docfx docfx.json --serve
```

## Documentation Structure

### Configuration Files

**`docfx.json`** - Main DocFX configuration
```json
{
  "metadata": [...],    // API extraction settings
  "build": {
    "content": [...],   // Content sources
    "output": "_site",  // Output directory
    "template": [...]   // Theme settings
  }
}
```

**`toc.yml`** - Table of Contents
```yaml
- name: Home
- name: Packages
  items:
    - name: SwissKnife
    - name: Crucible
    - name: Feber
    - name: Squid
- name: Documentation
- name: API Reference
```

### Content Sources

#### 1. XML Documentation
Generated from code comments in `.cs` files:

```csharp
/// <summary>
/// Brief description of the method.
/// </summary>
/// <param name="input">Description of parameter.</param>
/// <returns>Description of return value.</returns>
/// <example>
/// <code>
/// var result = MyMethod("test");
/// </code>
/// </example>
public string MyMethod(string input)
{
    // ...
}
```

#### 2. Package Documentation
Markdown files in package `docs/` folders:

```
BrightSword.SwissKnife/docs/
??? README.md          - Package overview
??? TypeExtensions.md  - Class-specific docs
??? Validator.md       - Class-specific docs
```

#### 3. Monorepo Documentation
Markdown files in root `docs/` folder:

```
docs/
??? BUILD.md           - Build guide
??? CONTRIBUTING.md    - Contributing guidelines
??? VERSIONING.md      - Versioning strategy
??? CICD.md           - CI/CD documentation
??? ARCHITECTURE.md    - Architecture overview
```

## Updating Documentation

### 1. Update Code Documentation

Edit XML comments in `.cs` files:

```csharp
/// <summary>
/// Updated description of the method.
/// </summary>
public void MyMethod()
{
    // ...
}
```

### 2. Update Package Documentation

Edit markdown files in package `docs/` folders:

```bash
# Edit package documentation
code BrightSword.SwissKnife/docs/README.md
```

### 3. Update Monorepo Documentation

Edit markdown files in root `docs/` folder:

```bash
# Edit contributing guide
code docs/CONTRIBUTING.md
```

### 4. Preview Changes Locally

```powershell
# Build and preview
.\build-docs.ps1 -Serve
```

### 5. Commit and Push

```bash
git add .
git commit -m "docs: update documentation"
git push
```

### 6. Automatic Deployment

GitHub Actions automatically:
1. Builds documentation from your changes
2. Deploys to GitHub Pages
3. Updates live site (2-5 minutes)

## Customization

### Change Theme

Edit `docfx.json`:

```json
{
  "build": {
    "template": ["default", "modern"]
  }
}
```

Available templates:
- `default` - Standard DocFX theme
- `modern` - Modern flat theme
- Custom templates (see DocFX docs)

### Add Custom Content

Create new markdown files and add to `docfx.json`:

```json
{
  "build": {
    "content": [
      {
        "files": ["my-custom-content.md"]
      }
    ]
  }
}
```

Update `toc.yml` to add navigation:

```yaml
- name: My Custom Section
  href: my-custom-content.md
```

### Configure Search

Search is enabled by default. To customize:

```json
{
  "build": {
    "globalMetadata": {
      "_enableSearch": true,
      "_enableNewTab": true
    }
  }
}
```

## Troubleshooting

### Documentation Not Building

**Problem**: Workflow fails with build errors

**Solutions**:
1. Check workflow logs in GitHub Actions
2. Build locally to see errors: `.\build-docs.ps1`
3. Verify `docfx.json` syntax
4. Ensure all referenced files exist

### API Documentation Missing

**Problem**: API reference is empty or incomplete

**Solutions**:
1. Verify XML documentation is enabled in `.csproj` files
2. Check `Directory.Build.props`:
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   ```
3. Add XML comments to public APIs
4. Rebuild project to generate XML files

### Links Not Working

**Problem**: Internal links return 404

**Solutions**:
1. Use relative paths in markdown: `[Link](../docs/BUILD.md)`
2. Use DocFX cross-reference syntax: `[Link](xref:NamespaceName.ClassName)`
3. Verify linked files are included in `docfx.json`

### Styles Not Applied

**Problem**: Documentation looks plain or unstyled

**Solutions**:
1. Check template configuration in `docfx.json`
2. Clear browser cache
3. Verify `_site` directory contains CSS files
4. Check workflow logs for template errors

## GitHub Actions Workflow

### Workflow File

`.github/workflows/docs.yml`:

```yaml
name: Documentation

on:
  push:
    branches: [ main ]
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

jobs:
  build-docs:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    - name: Install DocFX
      run: dotnet tool install -g docfx
    - name: Build documentation
      run: docfx docfx.json
    - name: Upload artifact
      uses: actions/upload-pages-artifact@v3
      with:
        path: '_site'

  deploy:
    needs: build-docs
    runs-on: ubuntu-latest
    steps:
    - name: Deploy to GitHub Pages
      uses: actions/deploy-pages@v4
```

### Workflow Triggers

The workflow runs:
- **Automatically** on push to `main`
- **Manually** via GitHub Actions UI

### Workflow Outputs

- **Build logs** - See in GitHub Actions
- **Artifact** - Downloadable `_site` directory
- **Deployment** - Live at GitHub Pages URL

## Best Practices

### 1. Keep Documentation Current

- Update docs when changing code
- Review docs in pull requests
- Use conventional commits: `docs: update API documentation`

### 2. Write Good XML Comments

```csharp
/// <summary>
/// Clear, concise summary (one sentence).
/// </summary>
/// <param name="input">Purpose of the parameter.</param>
/// <returns>What the method returns.</returns>
/// <exception cref="ArgumentNullException">
/// When this exception is thrown.
/// </exception>
/// <example>
/// <code>
/// // Real-world usage example
/// var result = MyMethod("example");
/// </code>
/// </example>
/// <remarks>
/// Additional notes, warnings, or context.
/// Performance considerations.
/// Thread-safety notes.
/// </remarks>
```

### 3. Use Consistent Structure

Each package `docs/README.md` should include:
- Overview
- Installation
- Quick Start
- API Reference
- Examples
- Best Practices
- Troubleshooting

### 4. Cross-Reference Content

Use links between related documentation:

```markdown
See [Contributing Guidelines](../../docs/CONTRIBUTING.md).
```

### 5. Include Code Examples

Always include runnable examples:

```csharp
// Bad - just API signature
public void DoSomething(string input);

// Good - with example
/// <example>
/// <code>
/// var service = new MyService();
/// service.DoSomething("test");
/// </code>
/// </example>
public void DoSomething(string input);
```

## Resources

- **DocFX Documentation**: https://dotnet.github.io/docfx/
- **GitHub Pages Documentation**: https://docs.github.com/en/pages
- **Markdown Guide**: https://www.markdownguide.org/
- **XML Documentation Comments**: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/

## Support

If you encounter issues:

1. **Check workflow logs** in GitHub Actions
2. **Build locally** to reproduce issues
3. **Review this guide** for solutions
4. **Open an issue** on GitHub

---

**Your documentation site**: https://brightsword.github.io/BrightSword/

**Last updated**: When you push to main
