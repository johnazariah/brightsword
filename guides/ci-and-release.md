---
layout: default
title: "ci-and-release"
parent: "Guides"
nav_order: 4
---

CI & Release: Packaging, Versioning, Dependency Graph, and Docs

Purpose
This document mirrors the guidance added to `.github/copilot-instructions.md` and is intended for maintainers and automated agents to consult before making changes that affect packaging, versioning, CI, or docs.

What was added
- Centralized versioning: `version.props` + `versioning.targets` (MSBuild `IncrementVersion` target).
- Packaging defaults and README handling centralized in `Directory.Build.props`.
- Package dependency graph generator: `scripts/generate-package-dependencies.ps1` producing `package-dependencies.json` with `dependencies`, `dependents`, `dependentsRecursive`, and `publishOrder`.
- CI workflows:
  - `.github/workflows/ci.yml`: build/test, collect XML docs, generate dependency manifest for PRs.
  - Reusable workflows: `.github/workflows/build-and-test.yml` (build/test and produce artifacts) and `.github/workflows/pack.yml` (pack packages and upload artifacts). These are `workflow_call`-style workflows intended to be invoked by other top-level workflows to reduce duplication.
  - `.github/workflows/publish-packages.yml`: manual and tag-triggered package publish in dependency order with permission guard (now calls the reusable `pack.yml`).
  - `.github/workflows/gh-pages.yml`: publish `artifacts/docs` to GitHub Pages on push to `main` (now calls the reusable `build-and-test.yml`).

Motivation
- Single source of truth for versions to make reviews simpler and reduce accidental mismatched versions.
- Prevent NU5039 packaging failures by ensuring a README is included in packages.
- Publish dependent packages automatically when an upstream package is released so consumers always resolve package references.
- Maintain up-to-date dependency manifest to speed CI decisions and reduce exploratory work.
- Publish docs automatically to GitHub Pages to keep user-facing documentation current.

Reusable workflows (summary and recommended usage)
- `build-and-test.yml` (path: `.github/workflows/build-and-test.yml`)
  - Purpose: restore, build packages, build tests, run tests, collect XML docs and per-project `docs/` markdown, and upload a `ci-artifacts` artifact containing `artifacts/docs` and test results.
  - Inputs: `configuration` (default `Release`), `dotnet-version` (default `10.0.x`).
  - How to call: top-level workflows use a `uses: ./.github/workflows/build-and-test.yml` job with `with` inputs. Example in `gh-pages.yml` uses the job to produce docs.

- `pack.yml` (path: `.github/workflows/pack.yml`)
  - Purpose: run MSBuild `Pack` target to produce nupkgs into `artifacts/packages` and upload a `packages` artifact.
  - Inputs: `configuration` (default `Release`).
  - How to call: top-level publish workflows should call this reusable workflow (example: `publish-packages.yml` calls it as `uses: ./.github/workflows/pack.yml`) so packing logic is consistent across CI and publish flows.

How to use (quick commands)
- Local build and test (always run before PR):
  - `dotnet restore`
  - `dotnet build -c Release`
  - `dotnet test -c Release --no-build`
- Generate dependency manifest locally:
  - `pwsh ./scripts/generate-package-dependencies.ps1`
- Bump a project version locally (no commit):
  - `dotnet msbuild Build.proj /t:IncrementVersion /p:Level=Patch`
  - This bumps the unified `VersionPrefix` in the root `version.props` for all packages.
- Produce packages locally:
  - `dotnet pack BrightSword.SwissKnife/BrightSword.SwissKnife.csproj -c Release -o ./artifacts/packages`

CI publishing notes
- All five packages (SwissKnife, Crucible, Feber, Squid, BrightSword.Packages) share a single unified version from the root `version.props`. There are no per-project version files.
- Tag-triggered releases: push a `v*` tag (e.g., `v2.0.1`). The `publish-packages.yml` workflow publishes all packages together — no dependency ordering or cascade is needed.
- Manual dispatch: use the GitHub Actions UI to trigger `publish-packages.yml`.
- Permission guard: only collaborators (admin/write/maintain) may dispatch the publish workflow.
- Secrets required for publishing: `NUGET_API_KEY` (and optionally `NUGET_SOURCE`).

Docs publishing
- CI collects XML docs and `docs/` markdown into `artifacts/docs` during CI runs. `gh-pages` workflow publishes this folder to GitHub Pages. When using the reusable `build-and-test.yml`, the produced artifact name is `ci-artifacts` and contains `artifacts/docs`.
- To improve HTML docs, add a doc generation script (DocFX, MkDocs) and wire it into `ci.yml` to produce `artifacts/docs`.

Where to look
- `version.props`, `versioning.targets`
- `Directory.Build.props`
- `scripts/generate-package-dependencies.ps1`
- `.github/workflows/ci.yml`, `.github/workflows/build-and-test.yml`, `.github/workflows/pack.yml`, `.github/workflows/publish-packages.yml`, `.github/workflows/gh-pages.yml`

Validation and safety
- Prefer opening a PR for version bumps rather than committing from CI. Use `/p:Commit=true` only with governance.
- Always run local builds and tests; use `--skip-duplicate` when pushing to NuGet to avoid failures on republish attempts.

Contact
- Document changes in the PR description when you modify packaging, versioning, or workflows so reviewers can validate behavior.
