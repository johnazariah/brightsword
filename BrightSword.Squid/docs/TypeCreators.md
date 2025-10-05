# Type Creators

This document describes the runtime type creation pipeline implemented in `BrightSword.Squid.TypeCreators`.

Primary class
- `BasicDataTransferObjectTypeCreator<T>`
  - Purpose: generate a concrete, lightweight implementation for an interface `T` at runtime.
  - Typical scenario: unit tests, DTO factories, or dynamic composition when writing a concrete class is undesirable.

Highlights
- Scans interface `T` for properties, events and methods and emits corresponding members on a generated class.
- Supports facet interfaces (additional interfaces the generated type should implement).
- Supports mapping patterns for property backing types using `TypeMaps` (e.g., map `IList<T>` to `List<T>` backing field).
- Generates a default constructor that initializes mapped readonly properties and sets default values provided by `DefaultValueAttribute`.
- Optionally tracks initialization of readonly properties (use `TrackReadonlyPropertyInitialized`).
- Supports custom behaviours via `SpecialBehaviours` — behaviours may add methods, attributes or other modifications to the emitted type.

When to use facets vs behaviours
- Facets (use `FacetInterfaces`) are primarily markers: they declare extra interfaces the emitted type should implement. Use facets when you want the generated type to advertise additional capabilities or to have consumer code detect features by interface checks (e.g. `IMyFacet`).
- Behaviours (added via `SpecialBehaviours`) inject implementation details into the emitted type. Use behaviours when you need the generator to add members, methods, or custom IL to implement cross-cutting capabilities (e.g. a `Clone` method, change-tracking helpers, or validation helpers).

Common pattern
- Declare a facet interface to indicate a capability (for discovery or typing): e.g. `IMyCloneFacet : ICloneable`.
- Register a behaviour keyed to a marker type (or interface) so the generator adds the implementation only when the facet is present.

Controlling generation (configuration and extension points)
- Public configuration properties on the creator instance:
  - `FacetInterfaces` — list of additional interfaces the emitted type should implement.
  - `SpecialBehaviours` — dictionary mapping a key `Type` to an `IBehaviour` instance to apply behaviour operations.
  - `TypeMaps` — collection/dictionary used to map interface or abstract property types to concrete backing types (for example map `IList<T>` -> `List<T>`).
  - `PropertyFilter` — a delegate or predicate used to exclude certain properties from emission.
  - `TrackReadonlyPropertyInitialized` — whether to generate initialization tracking for get-only properties.
  - `ClassName` / `AssemblyName` — control naming of the emitted class and assembly.
- Virtual/override extension points (subclass to change behaviour):
  - `GetBackingFieldName(PropertyInfo)` — change the naming convention for backing fields.
  - `PropertyOperations`, `MethodOperations`, `EventOperations`, `ClassOperations` — override to inject custom IL or attributes as the generator composes the `TypeBuilder`.
  - `GetAssemblyBuilder` / `GetModuleBuilder` — influence how dynamic assemblies/modules are created (useful to enable persistence or debugging).
  - `PersistAssembly(AssemblyBuilder)` — override to save emitted assembly to disk on supported runtimes.

Examples

1) Configure a creator with a facet and behaviour (simple registration)

```csharp
var creator = new BasicDataTransferObjectTypeCreator<IMyDto>
{
    FacetInterfaces = new[] { typeof(IMyCloneFacet) }
};
creator.SpecialBehaviours[typeof(ICloneable)] = new CloneBehaviour();
var instance = creator.CreateInstance();
```

2) Subclassing to change backing field naming and TypeMaps

```csharp
public class MyCreator<T> : BasicDataTransferObjectTypeCreator<T> where T : class
{
    protected override string GetBackingFieldName(PropertyInfo p) => "_" + char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);

    protected override IDictionary<Type, Type> TypeMaps => new Dictionary<Type, Type>
    {
        { typeof(IList<>), typeof(List<>) }
    };
}

var creator = new MyCreator<IMyDto>();
var dto = creator.CreateInstance();
```

Performance and caching
- Generated `Type` is cached in the `Type` property on the creator; subsequent calls reuse emitted type.
- The factory used to create instances is compiled once and cached in `InstanceFactory` for fast instance creation. To obtain a fresh emitted type, create a new creator instance with the desired configuration.

Security and diagnostics
- Emitted methods are generated using `Reflection.Emit`. Debugging generated IL requires reading the IL at runtime or instrumenting the ILGenerator calls.
- Persisting assemblies to disk is runtime-dependent; override `PersistAssembly` to implement persistence when supported on your runtime.
