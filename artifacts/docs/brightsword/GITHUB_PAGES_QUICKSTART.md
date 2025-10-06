# GitHub Pages Quick Start

## ?? Quick Setup (5 minutes)

### Step 1: Enable GitHub Pages
1. Go to: https://github.com/brightsword/BrightSword/settings/pages
2. Under **Source**, select: **GitHub Actions**
3. Click **Save**

### Step 2: Trigger First Build
```bash
# Push to main branch to trigger automatic build
git push origin main

# OR manually trigger via GitHub Actions:
# Go to Actions ? Documentation ? Run workflow
```

### Step 3: View Your Docs
After 2-5 minutes, visit:
```
https://brightsword.github.io/BrightSword/
```

## ?? Update Documentation

### Update Code Documentation
```csharp
/// <summary>
/// Your updated documentation here.
/// </summary>
public void MyMethod()
{
    // ...
}
```

### Update Package Guides
```bash
# Edit package-specific documentation
code BrightSword.SwissKnife/docs/README.md
code BrightSword.Crucible/docs/README.md
code BrightSword.Feber/docs/README.md
code BrightSword.Squid/docs/README.md
```

### Update Monorepo Docs
```bash
# Edit top-level documentation
code docs/BUILD.md
code docs/CONTRIBUTING.md
code docs/ARCHITECTURE.md
```

### Commit and Deploy
```bash
git add .
git commit -m "docs: update documentation"
git push origin main

# Automatic deployment to GitHub Pages (2-5 minutes)
```

## ?? Local Preview

### Install DocFX (once)
```powershell
dotnet tool install -g docfx
```

### Build and Preview
```powershell
# Build and serve locally
.\build-docs.ps1 -Serve

# Open browser to:
# http://localhost:8080
```

## ?? Documentation Structure

```
Your Repository
??? XML Comments (in .cs files)
?   ? Generates API Reference
?
??? BrightSword.SwissKnife/docs/
?   ??? README.md (Package guide)
?       ? https://...github.io/.../swissknife/
?
??? BrightSword.Crucible/docs/
?   ??? README.md (Package guide)
?       ? https://...github.io/.../crucible/
?
??? BrightSword.Feber/docs/
?   ??? README.md (Package guide)
?       ? https://...github.io/.../feber/
?
??? BrightSword.Squid/docs/
?   ??? README.md (Package guide)
?       ? https://...github.io/.../squid/
?
??? docs/
?   ??? BUILD.md
?   ??? CONTRIBUTING.md
?   ??? ARCHITECTURE.md
?   ??? VERSIONING.md
?       ? https://...github.io/.../docs/
?
??? docfx.json (Configuration)
    ? Orchestrates everything
```

## ?? Configuration Files

### `docfx.json`
Main configuration for DocFX - controls:
- Which projects to extract API docs from
- Which markdown files to include
- Output directory and templates
- Search and navigation settings

### `toc.yml`
Table of contents - defines navigation structure

### `.github/workflows/docs.yml`
GitHub Actions workflow - builds and deploys automatically

## ?? Common Tasks

### Add New Package Documentation
1. Create `YourPackage/docs/README.md`
2. Add to `docfx.json`:
   ```json
   {
     "files": ["**.md"],
     "src": "YourPackage/docs",
     "dest": "yourpackage"
   }
   ```
3. Add to `toc.yml`:
   ```yaml
   - name: Your Package
     href: yourpackage/README.md
   ```

### Change Documentation Theme
Edit `docfx.json`:
```json
{
  "build": {
    "template": ["default", "modern"]
  }
}
```

### Enable/Disable Search
Edit `docfx.json`:
```json
{
  "build": {
    "globalMetadata": {
      "_enableSearch": true
    }
  }
}
```

## ?? Troubleshooting

### Docs Not Building?
1. Check GitHub Actions logs
2. Build locally: `.\build-docs.ps1`
3. Verify `docfx.json` syntax

### API Docs Missing?
1. Verify XML documentation enabled in projects
2. Check `Directory.Build.props`:
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   ```
3. Add XML comments to public APIs

### Links Not Working?
1. Use relative paths: `[Link](../docs/BUILD.md)`
2. Verify files are in `docfx.json`
3. Check file paths are correct

## ?? Useful Links

- **Your Docs**: https://brightsword.github.io/BrightSword/
- **DocFX Docs**: https://dotnet.github.io/docfx/
- **Full Setup Guide**: [docs/GITHUB_PAGES.md](GITHUB_PAGES.md)

## ? Checklist

- [ ] GitHub Pages enabled in repository settings
- [ ] DocFX installed locally (`dotnet tool install -g docfx`)
- [ ] Documentation builds locally (`.\build-docs.ps1 -Serve`)
- [ ] Documentation workflow runs successfully
- [ ] Live site is accessible
- [ ] All packages have documentation in their `docs/` folders
- [ ] XML comments added to public APIs
- [ ] Links between documents work correctly

---

**Need help?** See the full guide: [docs/GITHUB_PAGES.md](GITHUB_PAGES.md)
