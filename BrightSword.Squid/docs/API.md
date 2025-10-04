# Public API and Workflows

This document covers the primary public-facing types and how you typically interact with the Squid library.

Primary entry points

- `BasicDataTransferObjectTypeCreator<T>`
  - `Type` — returns the emitted `System.Type` for `T`. Accessing it builds the type (if `T` is an interface).
  - `CreateInstance(dynamic source = null)` — constructs an instance of the generated type; if `source` provided, values are mapped into the instance via `FastMapper<T>`.
  - `AssemblyName`, `ClassName`, `FacetInterfaces` — configuration points to control naming and additional interfaces.
  - `SaveAssemblyToDisk` and `PersistAssembly` — control whether emitted assemblies are persisted (default no-op for portability).

Control points and order of operations

When you request `Type` (or call `CreateInstance`) the typical build pipeline executes in this order:
1. Validate configuration (primary interface `T` must be an interface for emission to occur).
2. Resolve effective interface set: primary interface `T` plus any `FacetInterfaces` provided.
3. Create a dynamic assembly and module (via `GetAssemblyBuilder` / `GetModuleBuilder`).
4. Create a `TypeBuilder` for the emitted class with the configured `ClassName` in the configured assembly.
5. Execute `PropertyOperations` to emit fields and properties for each property passing `PropertyFilter`.
6. Execute `EventOperations` and `MethodOperations` for remaining members.
7. Apply `SpecialBehaviours` operations where the behaviour's key indicates applicability (behaviours run after member emission so they can add methods that reference existing fields/members).
8. Execute `ClassOperations` for any class-level customizations.
9. Create the type via `TypeBuilder.CreateType()` and cache the resulting `Type`.
10. Compile an `InstanceFactory` (constructor delegate) and cache for fast instance creation.

Example: control emission with `PropertyFilter` and `TypeMaps`

```csharp
// Only emit writable properties and map IList<T> to List<T>
var creator = new BasicDataTransferObjectTypeCreator<IMyDto>
{
    PropertyFilter = (PropertyInfo p) => p.CanWrite, // only emit writable properties
    TypeMaps = new Dictionary<Type, Type>
    {
        { typeof(IList<>), typeof(List<>) }
    }
};

var dtoType = creator.Type; // triggers build
var instance = creator.CreateInstance();
```

Advanced usage
- To add additional interfaces: `creator.FacetInterfaces = new[] { typeof(IMyFacet) };`
- To provide custom property backing type mapping: subclass and override `TypeMaps`.
- To alter generated constructor logic: override `DefaultConstructorInstructions*` properties to inject IL instructions.

Error handling
- If a property has a `DefaultValueAttribute` that cannot be converted to the backing field type, the generator throws `NotSupportedException` during type creation. This is by-design so that invalid default-value declarations fail fast.

Notes
- If `typeof(T).IsClass` then `BuildType` returns `typeof(T)` unchanged; the generator only emits types for interface `T`.
- `CreateInstance` uses `FastMapper<T>` for copying from dynamic sources — review `FastMapper` documentation in `BrightSword.Feber` for mapping rules.
