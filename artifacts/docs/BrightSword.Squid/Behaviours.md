# Behaviours

Squid supports pluggable behaviours that can modify the emitted type during type creation. A behaviour implements `IBehaviour` and exposes operations applied to a `TypeBuilder`.

Key concept
- `IBehaviour` — provides an `IEnumerable<Func<TypeBuilder, TypeBuilder>> Operations` allowing behaviours to transform the `TypeBuilder` as part of the `BuildTypeBuilder` pipeline.

Built-in behaviours
- `CloneBehaviour` — adds a `Clone` method to the emitted type. The behaviour may add helper methods or attributes required for clone semantics. The repository includes a concrete `CloneBehaviour` implementation in `BrightSword.Squid.Behaviours.CloneBehaviour` that provides a portable deep-clone fallback used by generated `Clone()` methods.

When to use behaviours vs facets
- Use facets (via `FacetInterfaces`) when you want the emitted type to declare additional interfaces for consumers to detect capabilities. Facets are about typing and discovery.
- Use behaviours (via `SpecialBehaviours`) when you want the generator to implement concrete functionality on the emitted type. Behaviours are about implementation.

How behaviours are applied
- When building a type, the code enumerates `SpecialBehaviours` and for each behaviour and applicable interface applies `behaviour.Value.Operations` to the `TypeBuilder`.
- Behaviour selection occurs by testing whether the behaviour's key type is assignable from facet or primary interfaces. For example if you register a behaviour under `typeof(ICloneable)` and the effective set of interfaces includes `ICloneable` (either as primary or facet) then the behaviour will be applied.
- Behaviours execute after member emission so they can add methods that reference previously emitted fields and properties.

Using the provided `CloneBehaviour`

The repository already contains a production `CloneBehaviour` in `BrightSword.Squid.Behaviours`. It emits a public `Clone()` method on the generated type which calls a static helper `CloneBehaviour.FallbackClone(object)` to perform a deep copy of the instance. The behaviour is registered by default for `ICloneable` in the `BasicDataTransferObjectTypeCreator<T>` constructor, so you do not need to manually register it in most cases.

Example: enable clone support via a facet

```csharp
public interface IMyDto { string Name { get; set; } }
public interface ICloneFacet : ICloneable { }

var creator = new BasicDataTransferObjectTypeCreator<IMyDto>
{
    FacetInterfaces = new[] { typeof(ICloneFacet) }
};

var instance = creator.CreateInstance();
// The emitted type implements ICloneable and exposes Clone()
var clone = ((ICloneable)instance).Clone();
```

If you need to replace or extend the behaviour registration, subclass `BasicDataTransferObjectTypeCreator<T>` and override the `SpecialBehaviours` property (it is virtual) to supply your own mapping or behaviour instances.

Guidance for behaviour authors
- Keep operations idempotent — the builder pipeline may iterate behaviours in sequence and operations may be applied more than once in different build scenarios.
- Minimize assumptions about backing field names or existing emitted members; prefer to add independent helper members.
- Be conservative with public API surface added by behaviours — consumers will depend on generated members.
- Test behaviours using small, focused interfaces and assert the emitted members behave as expected.

Advanced tip: conditional behaviour application
- You can write behaviours that inspect the `TypeBuilder` and the set of emitted members and apply only when certain members are present. This allows behaviours to be robust when combined with other extensions.
