# Examples

This file contains short examples demonstrating common tasks with Squid.

1) Generate an instance of a simple interface
```csharp
public interface IMyDto
{
    string Name { get; set; }
    int Count { get; set; }
}

var creator = new BasicDataTransferObjectTypeCreator<IMyDto>();
var instance = creator.CreateInstance();
instance.Name = "hello";
instance.Count = 5;
```

2) Add a facet interface
```csharp
public interface ITimestamped
{
    DateTime CreatedAt { get; }
}

var creator = new BasicDataTransferObjectTypeCreator<IMyDto>
{
    FacetInterfaces = new[] { typeof(ITimestamped) }
};
var instance = creator.CreateInstance();
// the emitted type implements both IMyDto and ITimestamped
```

3) Extend generator to change backing field naming
```csharp
public class CustomCreator<T> : BasicDataTransferObjectTypeCreator<T> where T : class
{
    protected override string GetBackingFieldName(PropertyInfo propertyInfo) => "m_" + propertyInfo.Name;
}

var creator = new CustomCreator<IMyDto>();
var instance = creator.CreateInstance();
```

4) Facets + behaviours: full example

This example demonstrates adding a facet interface (e.g. `ICloneable` or a custom facet) and registering a `CloneBehaviour` so the emitted type implements the facet interface and exposes a `Clone()` method.

```csharp
public interface IMyDto
{
    string Name { get; set; }
    int Count { get; set; }
}

// choose a facet interface to indicate the clone behaviour should be applied
public interface IMyCloneFacet : ICloneable { }

// Setup the creator to emit the facet and register behaviour
var creator = new BasicDataTransferObjectTypeCreator<IMyDto>
{
    FacetInterfaces = new[] { typeof(IMyCloneFacet) }
};

// register the behaviour keyed by a type the creator inspects (this example uses ICloneable)
creator.SpecialBehaviours[typeof(ICloneable)] = new CloneBehaviour();

var instance = creator.CreateInstance();

// usage: you can cast the instance to the facet or ICloneable
if (instance is IMyCloneFacet cloneFacet)
{
    var copied = cloneFacet.Clone();
}

// Alternatively test the runtime method added by behaviour via reflection
var cloneMethod = instance.GetType().GetMethod("Clone");
var result = cloneMethod?.Invoke(instance, null);
```

Notes
- When combining facets and behaviours the creator applies behaviours after determining the full set of interfaces the emitted type will implement. Thus behaviours can rely on facet presence when adding members.
- Use facets to mark the desired capabilities; use behaviours to implement those capabilities on the emitted type.
