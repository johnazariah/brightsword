PR Checklist: Packaging, Versioning, CI, Docs

Before submitting a PR that changes code, packaging, or docs, run this checklist:

1. Build & Test
- [ ] Run `dotnet restore` at repo root
- [ ] Run `dotnet build -c Release`
- [ ] Run `dotnet test -c Release --no-build`

2. Versioning (if changing public API or package content)
- [ ] Bump the package version using `dotnet msbuild /t:IncrementVersion /p:ProjectName=<ProjectName> /p:Level=Patch` or edit `version.props` and explain the change in PR.
- [ ] Include the `version.props` change in your PR if you bumped versions.

3. Packaging
- [ ] Run `dotnet pack -c Release -o ./artifacts/packages <project.csproj>` for changed projects and inspect `.nupkg` for XML docs and README.md

4. Dependency graph
- [ ] Run `pwsh ./scripts/generate-package-dependencies.ps1` and verify `package-dependencies.json` reflects your changes (include updated manifest in PR if applicable)

5. Documentation
- [ ] Update XML comments for public APIs
- [ ] Update `docs/` markdown files under the project and run local doc generation if present
- [ ] Verify `artifacts/docs` contains updated documentation files

6. CI & Publish
- [ ] If this PR should trigger a preview package, document how CI should publish (preview feed, tag, or manual)
- [ ] For releases, ensure tags follow the convention and include package id if you expect tag-triggered publish to infer the package

7. PR description
- [ ] Document build and test commands you ran
- [ ] List files changed and any decisions about versioning/publishing
- [ ] Note if any CI secrets or environment updates are required

Notes
- Avoid committing package files or build artifacts under version control.
- Prefer to open a PR for version bumps so reviewers can validate and approve releases.

