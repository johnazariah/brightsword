This monorepo currently contains the following projects, and associated test projects:

* BrightSword.Crucible
* BrightSword.Feber
* BrightSword.SwissKnife
* BrightSword.Squid

Each of these listed projects generates a nuget package with its own version number.

The following tasks need to be done:

* Develop a coherent build strategy that builds and packages each individual package and MSBuild scripts to implement the strategy
* A sensible versioning strategy that maintains existing version numbers of each package in an appropriate location, and tools to automatically increment the version numbers at the appropriate time
* A CI/CD pipeline that supports branch development, a protected main which only accepts PRs, and revs/publishes a specified package and all other packages that depend on it automatically
* Consistent, thorough XML documentation of the classes and methods in each project with examples, remarks and usage guidelines
* Consistent documentation in the `docs` folder of each project which should be kept in sync with code changes
* A top level `docs` folder with information about the monorepo itself, the components in the monorepo, and guidelines for building and contributing to the monorepo
* Extraction of class and method documentation to GitHub Pages, in addition to the documents in the project-specific `docs` folders

# Copilot Onboarding Instructions

Purpose
- This repository is a .NET monorepo containing several small libraries and sample apps that produce NuGet packages: `BrightSword.Crucible`, `BrightSword.Feber`, `BrightSword.SwissKnife`, and `BrightSword.Squid`, plus associated test and sample projects.
- Primary language: C# targeting .NET 10 (net10.0). Projects are regular SDK-style projects with individual `.csproj` files.

High-level repo facts (quick reference)
- Target runtime / SDK: .NET 10 (install the .NET 10 SDK)
- Languages: C# (primary)
- Project layout: multiple projects at repository root level in subfolders (see Project Layout)
- Tests: xUnit/NUnit/other test projects exist alongside libraries (folders named `*Tests`)
- Repo size: small-to-medium (dozens of projects/files). No global.json or centralized Directory.Build.* files detected in the repo root.

Build and validation (always follow these exact steps first)
- Preconditions: ensure the .NET 10 SDK is installed and on PATH. Confirm with: `dotnet --version` (should report a 10.x SDK).
- Centralized MSBuild entrypoint: this repository provides `Build.proj` at the repo root. Use `dotnet msbuild` against `Build.proj` so local development and CI run the same targets.

- Recommended local commands (exact):
  - Full CI-equivalent run (clean, restore, build, test, pack):
    - `dotnet msbuild Build.proj /t:CI /p:Configuration=Release /v:minimal`
  - Run only restore/build/test (faster iteration):
    - `dotnet msbuild Build.proj /t:Restore;Build;Test /p:Configuration=Release`
  - Pack all packages:
    - `dotnet msbuild Build.proj /t:Pack /p:Configuration=Release`
  - Pack a single project (msbuild PackSingle target):
    - `dotnet msbuild Build.proj /t:PackSingle /p:Configuration=Release;Package=BrightSword.SwissKnife`
  - Pack a single csproj directly via MSBuild (equivalent to dotnet pack but uses MSBuild plumbing):
    - `dotnet msbuild BrightSword.SwissKnife/BrightSword.SwissKnife.csproj /t:Pack /p:Configuration=Release;PackageOutputPath=./artifacts/packages /v:minimal`

- Notes on using `Build.proj`:
  - `Build.proj` centralizes outputs under `artifacts/` (packages, docs, test-results). Use these paths when consuming artifacts in CI.
  - The `CI` target runs Clean, Restore, BuildPackages, BuildTests, Test, and Pack so it is the preferred CI entrypoint.

- Tests and test results:
  - Test results (TRX) are written to `artifacts/test-results` when running the MSBuild `Test`/`CI` targets.

- Pack and publish:
  - The CI publish workflow (`.github/workflows/publish-packages.yml`) calls MSBuild `Pack` on individual csproj files (via `dotnet msbuild <csproj> /t:Pack`) and publishes the resulting nupkgs in dependency order.

Notes on commands and common issues observed
- Always run the MSBuild `Restore` before `Build` when using `Build.proj`; the `CI` target handles this automatically.
- If you see stale or confusing errors, delete `bin/` and `obj/` directories under the affected project(s) and re-run the MSBuild `CI` target.
- There is no top-level solution file discovered by the agent scan; `dotnet msbuild Build.proj` is the canonical repository build entrypoint.

Trust these instructions: when validating or preparing a PR, run the same `dotnet msbuild Build.proj /t:CI` command locally that CI will run. Only search the repository when the information here is insufficient or inconsistent with the current state of the repository.

# Project-level instructions

BrightSword.Crucible
- Purpose: low-level utilities used by other BrightSword libraries. Small, focused API surface.
- Key files: `BrightSword.Crucible/BrightSword.Crucible.csproj`, sources under `BrightSword.Crucible/`, docs in `BrightSword.Crucible/docs/`.
- Build: `dotnet build BrightSword.Crucible/BrightSword.Crucible.csproj -c Release`.
- Test: if tests exist run `dotnet test <test-csproj> -c Release` (no dedicated test project listed by default).
- Pack: `dotnet pack BrightSword.Crucible/BrightSword.Crucible.csproj -c Release`.
- Versioning: check `Version` / `PackageVersion` properties in the project file. Keep per-project versioning in the `.csproj` unless a repo-wide strategy is introduced.
- Docs: maintain markdown under `BrightSword.Crucible/docs/`. Enable XML docs via `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in the csproj if not present.
- Tips: run dependent project tests after changes because other libraries may depend on this package.

BrightSword.Feber
- Purpose: higher-level helpers and a sample app (`BrightSword.Feber.SamplesApp`) demonstrating usage.
- Key files: `BrightSword.Feber/BrightSword.Feber.csproj`, sample app `BrightSword.Feber.SamplesApp/BrightSword.Feber.SamplesApp.csproj`, tests `BrightSword.Feber.Tests/BrightSword.Feber.Tests.csproj`, docs in `BrightSword.Feber/docs/`.
- Build: `dotnet build BrightSword.Feber/BrightSword.Feber.csproj -c Release` or build samples/tests as needed.
- Run sample app: `dotnet run --project BrightSword.Feber.SamplesApp/BrightSword.Feber.SamplesApp.csproj -c Release`.
- Test: `dotnet test BrightSword.Feber.Tests/BrightSword.Feber.Tests.csproj -c Release` (recommended to run after any change to `BrightSword.Feber`).
- Pack: `dotnet pack BrightSword.Feber/BrightSword.Feber.csproj -c Release`.
- Versioning: check the `Version`/`PackageVersion` properties in `BrightSword.Feber.csproj` and in `BrightSword.Feber.SamplesApp` if it references package versions.
- Docs & XML comments: maintain docs under `BrightSword.Feber/docs/` and ensure XML docs are generated by the library project.
- Tips: changes to the shared API must be validated by `BrightSword.Feber.Tests` and any downstream projects using `Feber`.

BrightSword.SwissKnife
- Purpose: collection of small, reusable extension methods and helpers (e.g., `AttributeExtensions`, `EnumerableExtensions`, `StringExtensions`).
- Key files: `BrightSword.SwissKnife/BrightSword.SwissKnife.csproj`, docs under `BrightSword.SwissKnife/docs/`, tests in `BrightSword.SwissKnife.Tests/BrightSword.SwissKnife.Tests.csproj`.
- Build: `dotnet build BrightSword.SwissKnife/BrightSword.SwissKnife.csproj -c Release`.
- Test: `dotnet test BrightSword.SwissKnife.Tests/BrightSword.SwissKnife.Tests.csproj -c Release`.
- Pack: `dotnet pack BrightSword.SwissKnife/BrightSword.SwissKnife.csproj -c Release`.
- Versioning: project-level `Version` / `PackageVersion` in the `.csproj` controls NuGet output. Preserve existing numbers when making small fixes.
- Docs: keep `docs/` files (many exist: `AttributeExtensions.md`, `EnumerableExtensions.md`, etc.) in sync with XML comments.
- Tips: these are commonly re-used helpers — add thorough XML comments and examples; run tests after any API change.

BrightSword.Squid
- Purpose: a more complex component with tests that use small helper test assemblies under `BrightSword.Squid.Tests/Remote*Assembly/`.
- Key files: `BrightSword.Squid/BrightSword.Squid.csproj`, test project `BrightSword.Squid.Tests/BrightSword.Squid.Tests.csproj`, helper assemblies under `BrightSword.Squid.Tests/Remote*Assembly/`, docs under `BrightSword.Squid/docs/`.
- Build: `dotnet build BrightSword.Squid/BrightSword.Squid.csproj -c Release`.
- Test: `dotnet test BrightSword.Squid.Tests/BrightSword.Squid.Tests.csproj -c Release` (these tests reference helper assemblies — build the test project which will compile them automatically).
- Pack: `dotnet pack BrightSword.Squid/BrightSword.Squid.csproj -c Release`.
- Versioning: check `Version`/`PackageVersion` in `BrightSword.Squid.csproj`.
- Docs: keep `BrightSword.Squid/docs/` synchronized with code; tests sometimes document expected behavior — update docs accordingly.
- Tips: when running `BrightSword.Squid.Tests`, ensure no leftover `obj/` artifacts in the helper projects cause spurious test behavior; if tests fail inconsistently, delete `obj/` and `bin/` in `BrightSword.Squid.Tests` and re-run.

General per-project rules
- Always run `dotnet restore` then `dotnet build -c Release` before running `dotnet test`.
- Run the specific project's tests after changes to that project.
- Update the project's `.csproj` `Version`/`PackageVersion` when you intentionally bump versions; prefer per-project versioning unless a repo-wide strategy is introduced.
- Ensure XML documentation generation is enabled on library projects that should publish docs (add `<GenerateDocumentationFile>true</GenerateDocumentationFile>` and a stable `PackageId`/`Version`).
- Keep `docs/` markdown files per project in sync with code comments; use scripts or CI to publish docs to GitHub Pages if required.

Release and cross-package publishing policy (important)
- Independent package versions: publish each top-level project as an independently versioned NuGet package. Do not attempt to keep a single global version across packages — each library has its own lifecycle.
- Dependency graph and dependent revs: maintain a dependency graph (either a checked-in manifest such as `package-dependencies.json` or compute it dynamically from project references) that maps which packages depend on which. This graph is used by CI to determine which additional packages must be re-released when a package changes.

How CI should handle a package publish
1. Identify the package(s) that changed in the PR or the release tag.
2. For each changed package:
   - Run `dotnet msbuild /t:IncrementVersion /p:ProjectName=<ProjectName> /p:Level=Patch` (or Minor/Major) locally or from a release job to produce the next `VersionPrefix` in `version.props`. Use `/p:Commit=true` only in trusted automation with git push credentials; otherwise capture the new version and apply it via PR or release commits.
   - Build, test, then `dotnet pack -c Release` and publish the package.
3. Using the dependency graph, compute the set of dependent packages (recursively). For each dependent package:
   - Bump its version (typically Patch), update any package references if you publish to an internal feed, run build/tests, pack and publish. This ensures downstream consumers receive updated packages with updated dependency constraints.
4. Publish order: publish the changed package(s) first, then their dependents in dependency-order to ensure published packages can resolve upstream dependencies.

Recommended CI implementation notes
- Compute dependency graph from `project.assets.json`/`dotnet list package` or maintain `package-dependencies.json` for faster lookup on CI. Maintain the manifest under source control and update it when references change.
- Use the MSBuild `IncrementVersion` target added to `versioning.targets` to bump versions centrally (it edits `version.props`). Prefer making a follow-up PR that contains version changes unless you have secure CI credentials to commit and push from CI.
- When publishing from CI, use authenticated `dotnet nuget push` with `--skip-duplicate` to avoid failures if multiple jobs attempt to publish the same version.
- Always run `dotnet restore`, `dotnet build -c Release`, and `dotnet test -c Release --no-build` for every package you will publish.

Summary checklist for agents
- Treat each top-level project as an independently versioned package.
- Maintain or compute a dependency graph and use it to identify and publish dependent packages when an upstream package is released.
- Use the provided `IncrementVersion` MSBuild target to bump `VersionPrefix` values centrally in `version.props`.
- Prefer issuing a PR with version changes for review; only commit/push from CI when credentials and governance allow it.

Trust these instructions and consult the repository's `version.props`, `versioning.targets`, and `Directory.Build.props` when implementing publishing automation.

Recent repository changes and motivation

To help future agents and maintainers, the following changes were added to this repository to standardize packaging, versioning, dependency-aware publishing, and documentation publishing. Each item below lists what was added, why it was added, and how an agent should use or validate it.

1) Centralized versioning (`version.props` + `versioning.targets`)
- What: `version.props` now contains per-project `VersionPrefix`, `PackageId`, and metadata. `versioning.targets` provides an MSBuild `IncrementVersion` target that supports `Level=Patch|Minor|Major` and optionally commits the change (`/p:Commit=true`).
- Why: single source of truth for package versions simplifies reviews and makes coordinated version bumps easier. `IncrementVersion` enables scripted bumps without manual edits.
- How to use locally: run `dotnet msbuild /t:IncrementVersion /p:ProjectName=BrightSword.SwissKnife /p:Level=Patch` (omit `Commit=true` unless you want the file committed by the local Git client). Validate by opening `version.props` to see the updated `VersionPrefix`.

2) Packaging standardization (`Directory.Build.props`) and README handling
- What: `Directory.Build.props` now sets common packaging properties (authors, license, repository, `GenerateDocumentationFile`, `PackageReadmeFile=README.md`), and includes any `docs/README.md` into packages by default. Per-project `PackageReadmeFile` entries were removed so the behavior is centralized.
- Why: avoid NU5039 packaging errors and ensure consistent package metadata across projects.
- How to validate: `dotnet pack <project>.csproj -c Release -o ./artifacts/packages` and inspect the produced `.nupkg` to confirm `README.md` and XML docs are included.

3) Package dependency graph generator (`scripts/generate-package-dependencies.ps1`)
- What: PowerShell script that scans `*.csproj`, maps each project to its `PackageId`, computes `dependencies`, `dependents`, `dependentsRecursive`, and `publishOrder`, and writes `package-dependencies.json` at repo root.
- Why: CI needs a quick way to compute what dependent packages must be re-published when an upstream package changes.
- How to run: `pwsh ./scripts/generate-package-dependencies.ps1` — check `package-dependencies.json` for `publishOrder` and `dependentsRecursive` outputs.

4) CI job to regenerate dependency manifest (`.github/workflows/regenerate-package-deps.yml`)
- What: Workflow runs the generator on PRs and pushes the updated `package-dependencies.json` back to the PR branch if it changes.
- Why: keep dependency manifest up-to-date automatically for PR reviewers and subsequent CI steps.
- Notes: pushing to forked PR branches may be blocked in some repos; the workflow will fail to push in that case. The workflow uses the checkout action with persisted credentials to push.

5) Publish workflow for dependency-ordered package publishing (`.github/workflows/publish-packages.yml`)
- What: Workflow computes packages to publish (manual dispatch or tag-triggered), computes a final `publishOrder` using `package-dependencies.json`, packs, and publishes packages in dependency order to the specified NuGet source. It enforces a permission guard so only repo collaborators with admin/write/maintain can dispatch.
- Why: ensures that when a package is released, dependents are re-published in the correct order so consumers can resolve upstream dependencies.
- How tag-triggering works: push a tag `v*`. The workflow will attempt to infer package(s) from the tag name or from files changed by the tagged commit; otherwise manual `package` input is required.
- Security: requires `NUGET_API_KEY` secret; the permission check uses the repository API to prevent external fork contributors from dispatching the workflow.

6) CI harmonization (`.github/workflows/ci.yml`)
- What: Consolidated build-and-test workflow to run on branch pushes and PRs; it produces XML docs and artifacts and runs the dependency generator on PRs. It uploads artifacts for downstream jobs.
- Why: single, predictable CI for build/test and artifacts reduces duplicated configuration and provides a standard place to run validations.
- How to validate: open Actions ? run the workflow (manual dispatch) or push to a feature branch / open a PR.

7) GitHub Pages publishing (`.github/workflows/gh-pages.yml`)
- What: Workflow to publish `artifacts/docs` to the `gh-pages` branch on `main` push. It uses `peaceiris/actions-gh-pages` and expects `artifacts/docs` to contain consolidated documentation (XML docs + markdown).
- Why: automatic docs publication keeps documentation in sync with code.
- Next step: add a `scripts/generate-docs.ps1` or DocFX pipeline if you want richer HTML docs; the current job will publish whatever is in `artifacts/docs`.

8) Preview publishing (recommended)
- What: CI can be extended to publish preview packages for non-main branches (preview suffix or separate feed). A preview workflow template exists conceptually; if you want it implemented I can add a non-main publish workflow that uses the dependency graph and publishes to a preview feed.
- Why: provides consumers a way to test branch changes without touching production NuGet feed.

General validation and safety notes
- Always test locally: run `dotnet restore`, `dotnet build -c Release`, `dotnet test -c Release` before packing/publishing.
- Use `--skip-duplicate` when pushing packages to avoid transient failures for re-published versions.
- Prefer PR-driven version bumps: use `dotnet msbuild /t:IncrementVersion` locally and produce a PR containing the `version.props` change rather than committing from CI, unless you have governance and credentials to allow CI commits.
- Secrets and permissions: ensure `NUGET_API_KEY` and `NUGET_SOURCE` are set in repository secrets and that only trusted workflows can use them. Publishing workflows enforce a collaborator permission check.

Where to look for related files
- `version.props`, `versioning.targets` — centralized versioning and IncrementVersion target.
- `Directory.Build.props` — packaging defaults and readme handling.
- `scripts/generate-package-dependencies.ps1` — package graph generator.
- `.github/workflows/regenerate-package-deps.yml` — regenerates dependency manifest on PRs.
- `.github/workflows/publish-packages.yml` — tag/manual/dispatch package publisher.
- `.github/workflows/ci.yml` — CI build/test, docs collection.
- `.github/workflows/gh-pages.yml` — publishes `artifacts/docs` to GitHub Pages.

Trust these documented changes: follow the guidance in this file before making publishing or versioning changes. Only search the repository if the file locations or behaviors described here deviate from the current state.
