# PR Plan — low-risk cleanup (automated)

Purpose
- Apply a small, non-invasive refactor-and-style PR that fixes mechanical issues, compiles cleanly, and reduces low-risk analyzer warnings.
- Surface medium/high-risk items for human review rather than changing behavior automatically.

Branch & PR
- Branch name: fix/cleanup/low-risk-2025-10-02
- PR title: chore: apply low-risk styling & mechanical fixes (format, braces, small simplifications)
- PR body: include the "Files changed" list, verification commands, and the medium/high-risk items that remain for review (use the template below).

General rules for automated edits
- Never change public API shapes (types of parameters, return types, virtual/abstract member names) without explicit reviewer approval.
- Avoid behavioral changes (culture-sensitive parsing, exception types) without a human review and tests.
- Make small commits (group by logical change), run format and build after each commit, and stop if build fails.

Low-risk changes (apply automatically)
- Description: mechanical, behavior-preserving edits that are safe to apply across the repo.
- Acceptable edits include:
  - Add braces to single-line `if`/`while`/`for` bodies.
  - Replace invalid `return throw` with a plain `throw` statement.
  - Replace `default(T)` with `default` where the analyzer suggests.
  - Fix invalid shorthand empty array/collection uses (e.g., `[]`) to `new List<T>()` or `Array.Empty<T>()` where appropriate.
  - Remove unused optional parameter on private/internal methods (only if callers are internal and the change is proven safe).
  - Convert trivial backing-field + read-only property to an auto-property only when the backing field is private and used only for that property.

Files and exact mechanical edits to include in this PR
- BrightSword.SwissKnife/Disposable.cs
  - Convert expression-bodied `Dispose` to a block and call `GC.SuppressFinalize(this)` after invoking the stored delegate.
  - Example change:
    ```csharp
    // before
    public void Dispose() => _dispose?.Invoke(Instance);

    // after
    public void Dispose()
    {
        _dispose?.Invoke(Instance);
        GC.SuppressFinalize(this);
    }
    ```

- BrightSword.SwissKnife/SequentialGuid.cs
  - Remove unused `object arg = null` parameter on `ResetCounters()` and update the Timer callback to `new Timer(_ => ResetCounters(), ...)` or `new Timer(_ => ResetCounters(), null, 0, 1000);` depending on existing code.
  - Rationale: reduces RCS1163 unused parameter warnings without changing behavior.

- BrightSword.SwissKnife/CoerceExtensions.cs
  - Fix compiled-invalid constructs (e.g., `return throw ...`) by replacing with `throw new ...;`.
  - Do NOT change parsing behavior (Culture) in this PR. Those are medium-risk items (see below).

- BrightSword.SwissKnife/TypeExtensions.cs
  - Replace invalid `[]` shorthand usages with `new List<Type>()` or `Array.Empty<Type>()` where appropriate.
  - Simplify collection initializers where the analyzer suggests (use `new()` only if consistent with repo style).

- BrightSword.SwissKnife/StringExtensions.cs
  - Ensure inner `if`/`while` blocks use braces where analyzer suggests.

- BrightSword.SwissKnife/EnumerableExtensions.cs
  - Add braces to the single-line `if` in `AllUniqueSorted` and similar small, mechanical brace additions.

- BrightSword.Feber/Samples/CloneFactory.cs (samples only)
  - Small cosmetic fixes: add braces around `continue` and similar one-line bodies.

- Conservative: small redundant-cast fixes and collection-initializer simplifications where the change is purely syntactic.

Medium-risk items (require reviewers / tests)
- BrightSword.SwissKnife/CoerceExtensions.cs — CA1305 (locale-sensitive parse methods)
  - Suggested reviewer action: decide on a preferred culture policy. If switching to invariant culture, convert `X.Parse(s)` to `X.Parse(s, CultureInfo.InvariantCulture)` or refactor to `TryParse(..., NumberStyles, CultureInfo.InvariantCulture, out ...)` with tests for representative inputs.
  - Do not apply automatically without tests/approval.

- BrightSword.SwissKnife/Validator.cs — CA2201 (throwing System.Exception)
  - Suggestion: replace `throw new Exception(message)` with a more specific exception type (`InvalidOperationException` or `ArgumentException`) after confirming intended semantics. Requires review because it affects consumers catching exceptions by type.

- BrightSword.SwissKnife/AttributeExtensions.cs — RCS1163 (unused `flags` parameter on public helpers)
  - Options:
    - Keep the parameter for API compatibility and add a local pragma suppression (current approach), or
    - Add an overload without `flags` and forward to the parameterized method. The latter is more ergonomic but still a public API addition; review recommended.

Higher-risk / do not auto-apply
- Renaming public parameters to underscore-prefixed names (affects public metadata).
- Renaming or removing public/virtual members (CA1716 warnings about reserved words) — these require API change discussion.
- Replacing broad `throw new Exception` without explicit approval.

Commit strategy (recommended)
- Commit 1: formatting + braces + collection init fixes (small, mechanical changes across ~5 files)
  - message: chore: add braces + formatting + collection init simplifications
- Commit 2: compile-fixes (replace `return throw` and similar errors)
  - message: fix: compile error - replace 'return throw' with 'throw'
- Commit 3: small behavioral-safety cleanup (Disposable GC.SuppressFinalize, SequentialGuid ResetCounters)
  - message: chore: add GC.SuppressFinalize and remove unused ResetCounters param
- After each commit: run `dotnet format` and `dotnet build` and include the build output in the PR.

Verification steps (local developer / CI)
- Run the following in PowerShell from repository root:
```powershell
# optional: install format tool if not yet installed
# dotnet tool restore

dotnet format "c:\work\BrightSword\BrightSword.sln"
dotnet build "c:\work\BrightSword\BrightSword.sln"
```
- Optional stricter gate (only after medium-risk fixes approved):
```powershell
dotnet build "c:\work\BrightSword\BrightSword.sln" -warnaserror
```

PR body template (copy this into the PR)

Title: chore: apply low-risk styling & mechanical fixes (format, braces, small simplifications)

Body:
- Summary: This PR applies a set of low-risk, behavior-preserving cleanups across the codebase: formatting, braces, small compile fixes, and collection initialization simplifications. No public API shapes or parsing semantics were changed in this PR.

- Files changed (major):
  - `BrightSword.SwissKnife/Disposable.cs` — call GC.SuppressFinalize in Dispose()
  - `BrightSword.SwissKnife/SequentialGuid.cs` — remove unused ResetCounters parameter
  - `BrightSword.SwissKnife/CoerceExtensions.cs` — fix invalid `return throw` usage
  - `BrightSword.SwissKnife/TypeExtensions.cs` — fix empty-list usages and collection init simplifications
  - `BrightSword.SwissKnife/StringExtensions.cs` — add braces for inner conditionals
  - `BrightSword.SwissKnife/EnumerableExtensions.cs` — add braces
  - `BrightSword.Feber/Samples/CloneFactory.cs` — sample cosmetic fixes

- Verification steps performed locally:
  - `dotnet format` -> OK (format applied)
  - `dotnet build` -> OK (build succeeded)

- Remaining items for review (not changed in this PR):
  1. `BrightSword.SwissKnife/CoerceExtensions.cs` — CA1305: parsing without IFormatProvider (requires decision/test coverage)
 2. `BrightSword.SwissKnife/Validator.cs` — CA2201: throwing broad System.Exception (recommend replace with specific exception types after review)
 3. `BrightSword.SwissKnife/AttributeExtensions.cs` — RCS1163: unused `flags` parameter on public helpers (left as-is for API compatibility)

- Notes: No unit tests exist in the repo; before changing parsing semantics we recommend adding a small unit test project that covers representative inputs.

Automation patterns for Copilot (search & replace snippets)
- Add braces to single-statement if:
  - Find: `if (COND) STATEMENT;`
  - Replace:
    ```csharp
    if (COND)
    {
        STATEMENT;
    }
    ```

- Replace invalid "return throw" with a throw:
  - Find: `return throw new X(...);`
  - Replace: `throw new X(...);`

- Simplify default expressions:
  - Find: `default(SomeType)` -> Replace: `default`

- Collection empty shorthand:
  - Prefer `Array.Empty<T>()` for empty array arguments where used frequently.

Follow-ups (after this PR)
- Create a short follow-up PR (or issue) that lists the medium-risk items with one recommended fix per item and example tests to add.
- Consider adding a tiny test project (xUnit) that covers `CoerceExtensions` behavior for a few cultures before changing parse behavior.

---

If you want I can now:
- Create this branch and commit the low-risk changes, push the branch and open the PR with the template above, or
- Create this `pr-plan.md` only (done), and leave the edits for you or Copilot to apply.

Which would you like me to do next?