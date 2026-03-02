# Extensions

The `BrightSword.Squid.Extensions` namespace contains small helpers used by the Squid code generator.

Notable helpers
- `TypeExtensions.GetNonGenericPartOfClassName` — returns the non-generic portion of a type name used when creating class names for emitted types.
- `TypeExtensions.GetGenericMethodOnType` — finds and constructs a closed generic method on a type; used when generating IL that calls generic methods.

Guidance
- These helpers are small and intended to remain stable; change them carefully as other parts of Squid depend on the exact naming semantics.
- When changing `GetNonGenericPartOfClassName` semantics, ensure `ClassName` generation in creators remains human-friendly and deterministic.
