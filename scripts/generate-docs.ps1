<#
.SYNOPSIS
Docs generator: collects project documentation into `artifacts/docs` as a Jekyll site.

This script produces a Jekyll-based documentation site using the just-the-docs theme.
It copies per-project docs, top-level docs, and XML API documentation into a structured
layout under artifacts/docs/ with proper Jekyll front matter for navigation.

GitHub Pages renders the markdown natively via Jekyll — no custom HTML conversion needed.
#>

param(
    [string]$Root = (Get-Location).Path,
    [string]$OutDir = "artifacts/docs"
)

Set-StrictMode -Version Latest

# Helper: add Jekyll front matter to a markdown file
function Add-FrontMatter {
    param(
        [string]$FilePath,
        [string]$Title,
        [string]$Parent = '',
        [int]$NavOrder = 100,
        [bool]$HasChildren = $false
    )
    $content = Get-Content -Raw -Path $FilePath -ErrorAction SilentlyContinue
    if (-not $content) { return }

    # Skip if front matter already exists
    if ($content.StartsWith('---')) { return }

    $fm = "---`nlayout: default`ntitle: `"$Title`"`n"
    if ($Parent) { $fm += "parent: `"$Parent`"`n" }
    $fm += "nav_order: $NavOrder`n"
    if ($HasChildren) { $fm += "has_children: true`n" }
    $fm += "---`n`n"

    [System.IO.File]::WriteAllText($FilePath, $fm + $content, [System.Text.Encoding]::UTF8)
}

Push-Location $Root
try {
    Write-Host "Generating Jekyll docs site into: $OutDir"
    if (Test-Path $OutDir) { Remove-Item -Recurse -Force $OutDir }
    New-Item -ItemType Directory -Path $OutDir | Out-Null

    $fullOutDir = (Resolve-Path -Path $OutDir).ProviderPath

    # --- 1. Jekyll configuration ---
    $configYml = @"
title: BrightSword Documentation
description: API documentation and guides for the BrightSword .NET libraries
remote_theme: just-the-docs/just-the-docs@v0.10.0
color_scheme: light

permalink: pretty

exclude:
  - "*.xml"

aux_links:
  "GitHub":
    - "https://github.com/johnazariah/brightsword"
  "NuGet":
    - "https://www.nuget.org/profiles/BrightSword"

nav_external_links:
  - title: GitHub Repository
    url: https://github.com/johnazariah/brightsword

footer_content: "BrightSword &copy; 2025. Distributed under <a href=\"https://creativecommons.org/licenses/by/4.0/\">CC BY 4.0</a>."
"@
    [System.IO.File]::WriteAllText((Join-Path $OutDir '_config.yml'), $configYml, [System.Text.Encoding]::UTF8)
    Write-Host "Wrote _config.yml"

    # --- 2. Landing page (index.md) ---
    $indexMd = @"
---
layout: default
title: Home
nav_order: 1
---

# BrightSword Documentation

A collection of high-quality .NET 10 libraries for utilities, testing, automated code generation, and advanced serialization.

[![CI Build](https://github.com/johnazariah/brightsword/actions/workflows/ci.yml/badge.svg)](https://github.com/johnazariah/brightsword/actions/workflows/ci.yml)
[![License: CC BY 4.0](https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by/4.0/)

---

## Packages

| Package | NuGet | Description |
|---------|-------|-------------|
| **SwissKnife** | [![NuGet](https://img.shields.io/nuget/v/BrightSword.SwissKnife.svg)](https://www.nuget.org/packages/BrightSword.SwissKnife/) | Utility classes and extension methods |
| **Crucible** | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Crucible.svg)](https://www.nuget.org/packages/BrightSword.Crucible/) | Unit testing utilities for MSTest |
| **Feber** | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Feber.svg)](https://www.nuget.org/packages/BrightSword.Feber/) | Automated delegate generation using Expression trees |
| **Squid** | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Squid.svg)](https://www.nuget.org/packages/BrightSword.Squid/) | Runtime type emission utilities |
| **Packages** | [![NuGet](https://img.shields.io/nuget/v/BrightSword.Packages.svg)](https://www.nuget.org/packages/BrightSword.Packages/) | Metapackage bundling all libraries |

## Quick Start

```bash
# Install the metapackage (includes all libraries)
dotnet add package BrightSword.Packages

# Or install individual packages
dotnet add package BrightSword.SwissKnife
```

## Test Coverage

The repository includes **508 tests** across all packages with **>85% line and branch coverage**.

## Browse Documentation

Use the sidebar to navigate per-package API docs and guides, or explore:

- [Architecture](guides/ARCHITECTURE) — system design and dependency graph
- [Build Guide](guides/BUILD) — building, testing, and packaging
- [CI/CD](guides/CICD) — continuous integration and release pipeline
- [Versioning](guides/VERSIONING) — unified suite versioning strategy
- [Contributing](guides/CONTRIBUTING) — how to contribute
"@
    [System.IO.File]::WriteAllText((Join-Path $OutDir 'index.md'), $indexMd, [System.Text.Encoding]::UTF8)
    Write-Host "Wrote index.md"

    # --- 3. Per-project documentation ---
    $projectOrder = @{
        'BrightSword.SwissKnife' = 10
        'BrightSword.Crucible' = 20
        'BrightSword.Feber' = 30
        'BrightSword.Squid' = 40
        'BrightSword.Packages' = 50
    }

    $friendlyNames = @{
        'BrightSword.SwissKnife' = 'SwissKnife'
        'BrightSword.Crucible' = 'Crucible'
        'BrightSword.Feber' = 'Feber'
        'BrightSword.Squid' = 'Squid'
        'BrightSword.Packages' = 'Packages'
    }

    foreach ($projName in $projectOrder.Keys | Sort-Object { $projectOrder[$_] }) {
        $projDocsDir = Join-Path $Root "$projName/docs"
        if (-not (Test-Path $projDocsDir)) { continue }

        $targetDir = Join-Path $OutDir $projName
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null

        $friendlyName = $friendlyNames[$projName]
        $order = $projectOrder[$projName]

        # Copy all markdown files
        $mdFiles = @(Get-ChildItem -Path $projDocsDir -Filter *.md -Recurse)
        $childOrder = 1
        foreach ($mdFile in $mdFiles) {
            $rel = $mdFile.FullName.Substring($projDocsDir.Length).TrimStart('\', '/')
            $dest = Join-Path $targetDir $rel
            $destDir = Split-Path $dest -Parent
            if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }
            Copy-Item -Path $mdFile.FullName -Destination $dest -Force

            $docTitle = [System.IO.Path]::GetFileNameWithoutExtension($mdFile.Name)
            if ($docTitle -eq 'README') {
                # README becomes the parent page
                Add-FrontMatter -FilePath $dest -Title $friendlyName -NavOrder $order -HasChildren ($mdFiles.Count -gt 1)
            } else {
                $childOrder++
                Add-FrontMatter -FilePath $dest -Title $docTitle -Parent $friendlyName -NavOrder $childOrder
            }
        }

        Write-Host "Copied docs for $projName ($($mdFiles.Count) files)"
    }

    # --- 4. Top-level guides ---
    $guidesDir = Join-Path $OutDir 'guides'
    New-Item -ItemType Directory -Path $guidesDir -Force | Out-Null

    # Create guides parent page
    $guidesIndex = @"
---
layout: default
title: Guides
nav_order: 60
has_children: true
---

# Guides

Repository-level documentation covering architecture, build process, CI/CD, versioning, and contributing guidelines.
"@
    [System.IO.File]::WriteAllText((Join-Path $guidesDir 'index.md'), $guidesIndex, [System.Text.Encoding]::UTF8)

    $topDocsDir = Join-Path $Root 'docs'
    if (Test-Path $topDocsDir) {
        $guideOrder = 1
        Get-ChildItem -Path $topDocsDir -Filter *.md -File | ForEach-Object {
            $dest = Join-Path $guidesDir $_.Name
            Copy-Item -Path $_.FullName -Destination $dest -Force
            $title = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
            $guideOrder++
            Add-FrontMatter -FilePath $dest -Title $title -Parent 'Guides' -NavOrder $guideOrder
            Write-Host "Copied guide: $($_.Name)"
        }
    }

    # --- 5. XML API docs (kept for reference, excluded from Jekyll nav via _config.yml) ---
    $xmls = Get-ChildItem -Recurse -File -Filter *.xml | Where-Object {
        ($_.FullName -match '[\\/]bin[\\/]Release[\\/]net10\.0[\\/]') -and ($_.FullName -notlike "$fullOutDir*")
    }
    foreach ($x in $xmls) {
        $dest = Join-Path $OutDir $x.Name
        Copy-Item -Path $x.FullName -Destination $dest -Force
        Write-Host "Copied XML: $($x.Name)"
    }

    Write-Host "Docs generation complete — Jekyll site ready in $OutDir"
} finally {
    Pop-Location
}
