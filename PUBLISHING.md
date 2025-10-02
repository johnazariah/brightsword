# Publishing packages (maintainers)

This repository publishes two independent NuGet packages:

- BrightSword.SwissKnife
- BrightSword.Feber (depends on SwissKnife)

We use an MSBuild orchestration file (`build.proj`) that runs `tools/bump_versions.py`, packs the projects, and (optionally) publishes the resulting artifacts.

Top-level targets

- `PublishSwissknife` — bump SwissKnife (or set an explicit version), pack SwissKnife and Feber, publish artifacts, and optionally commit bumped files.
- `PublishFeber` — bump Feber (or set an explicit version), pack Feber, publish artifacts, and optionally commit bumped files.
- `PackSwissKnife` / `PackFeber` — pack-only targets.
- `Publish` — publish `.nupkg` files in the `artifacts/` folder (uses `tools/publish_nupkgs.sh` and `tools/publish_nupkgs.ps1`).
- `CommitBumped` — commits `versions.json`, `bumped_versions.json`, and csproj changes when `DoCommit=true`.

Quick rules

- `publish-swissknife` (workflow dispatch): increments SwissKnife's patch version, bumps Feber's patch version, packs both, and optionally publishes both packages.
- `publish-feber` (workflow dispatch): increments Feber's patch version, packs Feber, and optionally publishes Feber only.

When to use each flow

- Quick publish (recommended): use the workflow dispatch UI (`publish=publish-swissknife` or `publish=publish-feber`).
- Tag-based explicit-version publish: push a tag `swissknife-vX.Y.Z` or `feber-vX.Y.Z`. The workflow forwards the version to MSBuild (`-p:VersionFromTag`) and the bump script sets that exact version.

Local manual publish (developer)

Bump versions locally using the helper script, then pack and (optionally) push:

```powershell
python .\tools\bump_versions.py --project BrightSword.SwissKnife --new-version 1.0.12

## How to run locally

- Pack SwissKnife only:

```powershell
dotnet msbuild build.proj -t:PackSwissKnife
```

- Full publish flow for SwissKnife without committing or pushing to NuGet:

```powershell
dotnet msbuild build.proj -t:PublishSwissknife -p:DoCommit=false -p:NUGET_API_KEY=
```

- Full publish flow for Feber with an explicit version (tag-style):

```powershell
dotnet msbuild build.proj -t:PublishFeber -p:DoCommit=false -p:NUGET_API_KEY= -p:VersionFromTag=2.0.0
```

## CI behavior (GitHub Actions)

- The workflow `.github/workflows/ci-pack.yml` runs MSBuild targets and chooses the target to run based on workflow_dispatch inputs or pushed tags.
- For dispatch flows the workflow sets `DoCommit=true` so `CommitBumped` may attempt to push bumped files back to the branch. If branch protection blocks pushes, the workflow logs the failure but the rest of the pipeline continues.

## Commit behavior

- `CommitBumped` runs only when `DoCommit=true`.
- If pushing bumped files from CI is undesirable in your repo, we can change the flow to open a PR instead of pushing; I can implement that on request.

## Secrets and publishing

- To publish to NuGet.org set the repository secret `NUGET_API_KEY`.
- The workflow provides `NUGET_API_KEY` in step env so MSBuild and the `tools/` publish helpers can access it at runtime. CI uses `GITHUB_TOKEN` for commit-back operations.

## Debugging and logs

- `tools/bump_versions.py` writes `bumped_versions.json` with the exact versions it set; CI prints this file to the logs for traceability.
- If a pipeline run fails, inspect the workflow logs for `bumped_versions.json` and the MSBuild output.

## Recommended workflow

1. For routine publishes use the workflow dispatch with `publish=publish-swissknife` or `publish=publish-feber` from Actions → CI - Pack and optional publish → Run workflow.
2. For release-tagged builds push tags `swissknife-vX.Y.Z` or `feber-vX.Y.Z` to set explicit versions.

If you'd like, I can update `CommitBumped` to open a PR with bumped files instead of pushing directly or add a `build.ps1` convenience wrapper for Windows maintainers. Tell me which one you prefer and I'll add it.
