CI & Release: Packaging, Versioning, Dependency Graph, and Docs

Purpose
This document mirrors the guidance added to `.github/copilot-instructions.md` and is intended for maintainers and automated agents to consult before making changes that affect packaging, versioning, CI, or docs.

What was added
- Centralized versioning: `version.props` + `versioning.targets` (MSBuild `IncrementVersion` target).
- Packaging defaults and README handling centralized in `Directory.Build.props`.
- Package dependency graph generator: `scripts/generate-package-dependencies.ps1` producing `package-dependencies.json` with `dependencies`, `dependents`, `dependentsRecursive`, and `publishOrder`.
- CI workflows:
  - `.github/workflows/ci.yml`: build/test, collect XML docs, generate dependency manifest for PRs.
  - `.github/workflows/regenerate-package-deps.yml`: regenerate `package-dependencies.json` on PRs and push updates to the PR branch.
  - `.github/workflows/publish-packages.yml`: manual and tag-triggered package publish in dependency order with permission guard.
  - `.github/workflows/gh-pages.yml`: publish `artifacts/docs` to GitHub Pages on push to `main`.

Motivation
- Single source of truth for versions to make reviews simpler and reduce accidental mismatched versions.
- Prevent NU5039 packaging failures by ensuring a README is included in packages.
- Publish dependent packages automatically when an upstream package is released so consumers always resolve package references.
- Maintain up-to-date dependency manifest to speed CI decisions and reduce exploratory work.
- Publish docs automatically to GitHub Pages to keep user-facing documentation current.

How to use (quick commands)
- Local build and test (always run before PR):
  - `dotnet restore`
  - `dotnet build -c Release`
  - `dotnet test -c Release --no-build`
- Generate dependency manifest locally:
  - `pwsh ./scripts/generate-package-dependencies.ps1`
- Bump a project version locally (no commit):
  - `dotnet msbuild /t:IncrementVersion /p:ProjectName=BrightSword.SwissKnife /p:Level=Patch`
- Produce packages locally:
  - `dotnet pack BrightSword.SwissKnife/BrightSword.SwissKnife.csproj -c Release -o ./artifacts/packages`

CI publishing notes
- Tag-triggered releases: push `vX.Y.Z` tag and include the package id in the tag name (recommended) or ensure changed files are part of the tagged commit so the publish workflow can infer the package(s).
- Manual dispatch: use the `package` input to explicitly name the PackageId to publish.
- Permission guard: only collaborators (admin/write/maintain) may dispatch the publish workflow.
- Secrets required for publishing: `NUGET_API_KEY` (and optionally `NUGET_SOURCE`).

Docs publishing
- CI collects XML docs and `docs/` markdown into `artifacts/docs` during CI runs. `gh-pages` workflow publishes this folder to GitHub Pages.
- To improve HTML docs, add a doc generation script (DocFX, MkDocs) and wire it into `ci.yml` to produce `artifacts/docs`.

Where to look
- `version.props`, `versioning.targets`
- `Directory.Build.props`
- `scripts/generate-package-dependencies.ps1`
- `.github/workflows/ci.yml`, `.github/workflows/regenerate-package-deps.yml`, `.github/workflows/publish-packages.yml`, `.github/workflows/gh-pages.yml`

Validation and safety
- Prefer opening a PR for version bumps rather than committing from CI. Use `/p:Commit=true` only with governance.
- Always run local builds and tests; use `--skip-duplicate` when pushing to NuGet to avoid failures on republish attempts.

Contact
- Document changes in the PR description when you modify packaging, versioning, or workflows so reviewers can validate behavior.
