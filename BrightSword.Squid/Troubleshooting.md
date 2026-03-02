# Troubleshooting and Notes

Persistence
- Many runtimes (especially single-file or trimmed publish modes) do not support saving dynamic assemblies to disk. The default `PersistAssembly` is a no-op.
- If you need to save an assembly for debugging, override `PersistAssembly` and call the appropriate APIs on the `AssemblyBuilder` for your runtime.

Debugging generated IL
- Use `System.Reflection.Metadata` or third-party tools that can inspect dynamic assemblies at runtime.
- Alternatively, temporarily modify `GetAssemblyBuilder` and `GetModuleBuilder` to produce an assembly that can be saved and inspected.

Common errors
- `NotSupportedException` during `BuildType`: usually indicates a `DefaultValueAttribute` could not be converted to the expected property type. Check the attribute and supported TypeConverters.
- Missing members on emitted types: ensure `FacetInterfaces` includes interfaces that introduce the desired members, and that `PropertyFilter` isn't excluding them.

Platform differences
- Reflection.Emit APIs can differ across runtimes and versions. Keep the runtime SDK aligned with the one used for development (see repository `global.json`).

Performance
- Building types and compiling instance factories is relatively expensive; prefer caching creators or compiled factories in long-running applications.

Contact
- If you hit an issue with behaviour injection or IL generation, include a small reproduction showing the interface and the generator configuration so maintainers can reproduce quickly.
