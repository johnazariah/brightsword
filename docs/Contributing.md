# Contributing to BrightSword

Guidance for making changes, running builds and creating PRs.

Development setup
- Use the .NET SDK pinned in `global.json`.
- Restore and build the solution: `dotnet restore` then `dotnet build`.
- Run project-specific tests with `dotnet test`.

Style and formatting
- Use `dotnet format` to apply consistent code formatting.
- Keep changes small and focused; prefer incremental PRs.

Testing
- Add unit tests for any non-trivial logic or public API changes.
- Run the test project associated with changes.

Documentation
- Add or update content in the `docs` folder when adding notable features or changing usage patterns.

Merging
- Preserve public API compatibility unless intentionally introducing a breaking change.
- If releasing a breaking change, update versioning metadata and document the migration path.

Contact
- Include context in PR descriptions to help reviewers understand the rationale for changes.
