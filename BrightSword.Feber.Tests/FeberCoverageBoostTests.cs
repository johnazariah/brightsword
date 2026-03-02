using System.Dynamic;
using BrightSword.Feber.Samples;

namespace BrightSword.Feber.Tests;

// DTO with conventional backing fields for MapToBackingFields tests
public class DtoWithBackingFields
{
    private string _name;
    private int _age;

    public string Name => _name;
    public int Age => _age;

    // Allow test verification via reflection
    internal string GetName() => _name;
    internal int GetAge() => _age;
}

// ========== BuilderWarmup Tests ==========

public class BuilderWarmupTests
{
    [Fact]
    public void Warmup_ExecutesAllActions()
    {
        var executed = new List<int>();

        BuilderWarmup.Warmup(new Action[]
        {
            () => executed.Add(1),
            () => executed.Add(2),
            () => executed.Add(3),
        });

        Assert.Equal(new[] { 1, 2, 3 }, executed);
    }

    [Fact]
    public void Warmup_SwallowsExceptions_AndContinues()
    {
        var executed = new List<int>();

        BuilderWarmup.Warmup(new Action[]
        {
            () => executed.Add(1),
            () => throw new InvalidOperationException("boom"),
            () => executed.Add(3),
        });

        Assert.Equal(new[] { 1, 3 }, executed);
    }
}

// ========== CloneFactory 3-type-parameter overload ==========

public class CloneFactoryThreeTypeParamTests
{
    [Fact]
    public void Clone_WithExplicitBase_CopiesMatchingProperties()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };

        var clone = source.Clone<FeberTestDto, FeberTestDtoAlt, FeberTestDto>();

        Assert.Equal("Alice", clone.Name);
        Assert.Equal(30, clone.Age);
    }
}

// ========== FastMapper DynamicToDynamic extension method ==========

public class FastMapperDynamicToDynamicTests
{
    [Fact]
    public void MapDynamicToDynamic_ExtensionMethod_ReturnsDestination()
    {
        dynamic source = new ExpandoObject();
        source.Name = "Dave";
        source.Age = 50;

        dynamic dest = new ExpandoObject();
        dest.Name = "Old";
        dest.Age = 0;

        var result = FastMapper.MapDynamicToDynamic<FeberTestDto>((object)dest, (object)source);

        Assert.Same((object)dest, result);
    }
}

// ========== FastMapper MapToBackingFields ==========

public class FastMapperBackingFieldsTests
{
    [Fact]
    public void MapToBackingFields_ExtensionMethod_SetsBackingFields()
    {
        dynamic source = new ExpandoObject();
        source.Name = "Eve";
        source.Age = 35;

        var dest = new DtoWithBackingFields();

        var result = FastMapper.MapToBackingFields(dest, (object)source);

        Assert.Equal("Eve", dest.Name);
        Assert.Equal(35, dest.Age);
    }
}

// ========== LazyActionBuilderExample with real properties ==========

public class LazyActionBuilderExamplePropertyTests
{
    [Fact]
    public void Action_WithTypedDto_WritesPropertyNamesToConsole()
    {
        var builder = new LazyActionBuilderExample<FeberTestDto, FeberTestDto>();

        var action = builder.Action;

        Assert.NotNull(action);

        var writer = new System.IO.StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);
        try
        {
            action(new FeberTestDto { Name = "Test", Age = 42 });
            var output = writer.ToString();
            Assert.Contains("Name", output);
            Assert.Contains("Test", output);
            Assert.Contains("Age", output);
            Assert.Contains("42", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
