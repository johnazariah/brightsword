# BrightSword — quick runtime note

Short summary
- The Feber builders (ActionBuilder / FunctionBuilder and their OperationBuilder bases) compose LINQ Expression fragments for each property and then compile a delegate from the composed expression block.

Performance characteristics
- Building the expression blocks is relatively expensive: it involves reflection (scanning properties), creating Expression nodes, and JIT/compiling the resulting lambda. This cost is paid once when the builder composes and compiles the delegate.
- To avoid repeating that cost, builders cache the compiled delegate (for example a private field populated with `_action ??= BuildAction()` or `_function ??= BuildFunction()`). After the first build, running the compiled delegate is very fast — comparable to a hand-written method call.

Practical guidance
- Expect a one-time setup/compile cost at first use; measure in your scenario and consider warming (call the Action/Function once at app startup) if low-latency first requests are important.
- If compilation cost is a concern in multi-threaded server environments, consider initializing the delegate at startup or replace the simple null-coalescing cache with a thread-safe Lazy<T> or explicit synchronization to avoid duplicate compilation work.
- Avoid creating new ParameterExpression instances in overrides. Use the supplied `InstanceParameterExpression`, `LeftInstanceParameterExpression` and `RightInstanceParameterExpression` properties so that the compiled lambda parameter identity matches expressions in the body; mismatched parameter identity causes runtime compilation errors.

Testing and maintainability
- Prefer stable, introspective tests that validate `FilteredProperties` and `OperationExpressions` content rather than attempting to compile large composed blocks in tests. If you must compile the block in tests, obtain the exact ParameterExpression instance used by the builder.

Customization points
- Override `BuildAction()` / `BuildFunction()` to change compilation or caching strategy (for example to use different compilation options or to emit an interpreted delegate).
- Consider exposing a customizable field-discovery or mapping strategy if you rely on convention-based behaviors (backing-field lookup, naming conventions).

Where to look next
- Package-level docs and per-sample pages live under `BrightSword.Feber/docs/` and `BrightSword.SwissKnife/docs/`.
- See `BrightSword.Feber/docs/FunctionBuilder.md` and `BrightSword.Feber.Tests/NullCheckBuilderTests.cs` for concrete examples (NullChecker) and tests.
# BrightSword — build & publishing

This repository uses an MSBuild orchestration file (`build.proj`) to bump versions, pack projects, and optionally publish NuGet packages.

Quick overview
- High-level MSBuild targets (run with `dotnet msbuild build.proj -t:<Target>`):
  - `PublishSwissknife` — bump SwissKnife (or set explicit version), pack SwissKnife and Feber, publish if configured, and commit bumped files when requested.
  - `PublishFeber` — bump Feber (or set explicit version), pack Feber, publish if configured, and commit bumped files when requested.
  - `PackSwissKnife`, `PackFeber` — pack-only targets.
  - `Publish` — publish any `.nupkg` files created under `artifacts/` (uses helpers in `tools/`).

Files of interest
- `build.proj` — MSBuild orchestration (root).
- `tools/bump_versions.py` — version bump helper (updates `versions.json` and csproj `<Version>` fields, writes `bumped_versions.json`).
- `versions.json` — canonical per-project versions tracked in repo.
- `bumped_versions.json` — machine-readable output from bump script (created during bump runs).
- `tools/publish_nupkgs.sh` and `tools/publish_nupkgs.ps1` — cross-platform helpers used by the `Publish` target.
- `.github/workflows/ci-pack.yml` — CI workflow now invokes `build.proj` targets.

Local examples
- Pack SwissKnife only:

```powershell
dotnet msbuild build.proj -t:PackSwissKnife
```

- Full publish flow for SwissKnife (no commit, no publish to NuGet):

```powershell
dotnet msbuild build.proj -t:PublishSwissknife -p:DoCommit=false -p:NUGET_API_KEY=
```

- Full publish flow for Feber with an explicit version (like a tag-driven release):

```powershell
dotnet msbuild build.proj -t:PublishFeber -p:DoCommit=false -p:NUGET_API_KEY= -p:VersionFromTag=2.0.0
```

CI invocation (what the workflow does)
- The GitHub Actions workflow calls `dotnet msbuild build.proj` and invokes `PublishSwissknife` or `PublishFeber` depending on the dispatch input or pushed tag.
- When dispatching the workflow with `publish=publish-swissknife`, the workflow sets `DoCommit=true` and runs the `PublishSwissknife` target. That target will call the bump script, pack, publish (if `NUGET_API_KEY` is present), and attempt to commit bumped files back to the branch.

Commit behavior
- `CommitBumped` runs only when `DoCommit=true`. The workflow sets this flag for explicit dispatch runs so bump commits are attempted automatically.
- If your branch has protection rules that prevent pushes from the workflow token, the push may fail. The workflow is tolerant and logs the outcome. If you prefer, we can alter the workflow to open a PR with bumped files instead of pushing directly.

Tagging conventions
- To publish an explicit version for SwissKnife or Feber, push a tag using the form `swissknife-vX.Y.Z` or `feber-vX.Y.Z`. The workflow will pass the version to MSBuild and `bump_versions.py` will set that explicit version.

# BrightSword — build & publishing

This repository uses an MSBuild-based orchestration (`build.proj`) to bump versions, pack the projects, and optionally publish NuGet packages.

Overview
- Packages:
  - BrightSword.SwissKnife
  - BrightSword.Feber (depends on SwissKnife)
- Orchestration: `build.proj` is the canonical entry point used by local maintainers and the GitHub Actions workflow (`.github/workflows/ci-pack.yml`).
- Helper script: `tools/bump_versions.py` updates `versions.json` and project `<Version>` metadata and writes `bumped_versions.json` (used for traceability in CI logs).

Common targets
- `PackSwissKnife` / `PackFeber` — pack only.
- `PublishSwissknife` — bump SwissKnife (or set explicit version), pack SwissKnife and Feber, publish artifacts, and optionally commit bumped files when `DoCommit=true`.
- `PublishFeber` — bump Feber (or set explicit version), pack Feber, publish artifacts, and optionally commit bumped files.
- `Publish` — publish `.nupkg` files in `artifacts/` using the cross-platform helpers in `tools/`.

Local examples
- Pack SwissKnife only:

```powershell
dotnet msbuild build.proj -t:PackSwissKnife
```

- Run the full SwissKnife publish flow locally without committing or pushing to NuGet:

```powershell
dotnet msbuild build.proj -t:PublishSwissknife -p:DoCommit=false -p:NUGET_API_KEY=
```

- Publish Feber with a specific version (tag-style):

```powershell
dotnet msbuild build.proj -t:PublishFeber -p:DoCommit=false -p:NUGET_API_KEY= -p:VersionFromTag=2.0.0
```

CI behavior
- The GitHub Actions workflow `.github/workflows/ci-pack.yml` runs the MSBuild targets and decides which target to invoke based on:
  - workflow_dispatch inputs (`publish=publish-swissknife` or `publish=publish-feber`), or
  - pushed tags of the form `swissknife-vX.Y.Z` or `feber-vX.Y.Z` (the workflow extracts the version and passes it to MSBuild).
- For explicit dispatch flows the workflow sets `DoCommit=true` so `CommitBumped` can attempt to push bumped files back to the branch. If branch protection blocks the push, the workflow logs the failure.

Secrets
- To publish packages to NuGet.org set the repository secret `NUGET_API_KEY`.
- The workflow will place `NUGET_API_KEY` in step env so MSBuild and the `tools/` publish helpers can access it at runtime. CI uses `GITHUB_TOKEN` for any commit-back operations.

Troubleshooting
- `bumped_versions.json` is printed to CI logs and contains the exact versions applied by the bump script. Use it to confirm what was published.
- If `CommitBumped` push fails, convert the flow to open a PR with bumped files instead of pushing directly (I can implement that change on request).

Want me to open a PR with these docs updates or add a small `build.ps1` wrapper for Windows contributors? Say which and I'll do it next.
