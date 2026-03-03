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

`ash
# Install the metapackage (includes all libraries)
dotnet add package BrightSword.Packages

# Or install individual packages
dotnet add package BrightSword.SwissKnife
`

## Test Coverage

The repository includes **508 tests** across all packages with **>85% line and branch coverage**.

## Browse Documentation

Use the sidebar to navigate per-package API docs and guides, or explore:

- [Architecture](guides/ARCHITECTURE) — system design and dependency graph
- [Build Guide](guides/BUILD) — building, testing, and packaging
- [CI/CD](guides/CICD) — continuous integration and release pipeline
- [Versioning](guides/VERSIONING) — unified suite versioning strategy
- [Contributing](guides/CONTRIBUTING) — how to contribute