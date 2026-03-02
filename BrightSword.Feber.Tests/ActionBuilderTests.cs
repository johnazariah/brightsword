using System.Dynamic;
using BrightSword.Feber.Samples;

namespace BrightSword.Feber.Tests;

// Shared DTOs for action-builder tests
public class FeberTestDto
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class FeberTestDtoWithDate
{
    public string Name { get; set; }
    public DateTime Created { get; set; }
}

public class FeberTestDtoReadOnly
{
    public string Name { get; set; }
    public int Age { get; }
}

public class FeberTestDtoAlt
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Extra { get; set; }
}

// ========== FastMapper Tests (binary ActionBuilder) ==========

public class FastMapperTests
{
    [Fact]
    public void MapStaticToStatic_CopiesAllProperties()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };
        var dest = new FeberTestDto();

        FastMapper<FeberTestDto>.MapStaticToStatic(source, dest);

        Assert.Equal("Alice", dest.Name);
        Assert.Equal(30, dest.Age);
    }

    [Fact]
    public void MapStaticToStatic_ExtensionMethod_CopiesAllProperties()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };
        var dest = new FeberTestDto();

        var result = dest.MapStaticToStatic(source);

        Assert.Equal("Alice", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Same(dest, result);
    }

    [Fact]
    public void MapDynamicToStatic_CopiesFromExpando()
    {
        dynamic source = new ExpandoObject();
        source.Name = "Bob";
        source.Age = 25;

        var dest = new FeberTestDto();

        FastMapper<FeberTestDto>.MapDynamicToStatic(source, dest);

        Assert.Equal("Bob", dest.Name);
        Assert.Equal(25, dest.Age);
    }

    [Fact]
    public void MapDynamicToStatic_ExtensionMethod_CopiesFromExpando()
    {
        dynamic source = new ExpandoObject();
        source.Name = "Bob";
        source.Age = 25;

        var dest = new FeberTestDto();
        var result = FastMapper.MapDynamicToStatic<FeberTestDto>(dest, source);

        Assert.Equal("Bob", result.Name);
        Assert.Equal(25, result.Age);
        Assert.Same(dest, result);
    }

    [Fact]
    public void MapStaticToDynamic_CopiesToExpando()
    {
        var source = new FeberTestDto { Name = "Carol", Age = 40 };
        dynamic dest = new ExpandoObject();

        FastMapper<FeberTestDto>.MapStaticToDynamic(source, dest);

        Assert.Equal("Carol", (string)dest.Name);
        Assert.Equal(40, (int)dest.Age);
    }

    [Fact]
    public void MapStaticToDynamic_ExtensionMethod_CopiesToExpando()
    {
        var source = new FeberTestDto { Name = "Carol", Age = 40 };
        dynamic dest = new ExpandoObject();

        var result = ((object)dest).MapStaticToDynamic(source);

        Assert.Equal("Carol", (string)((dynamic)result).Name);
        Assert.Equal(40, (int)((dynamic)result).Age);
    }

    [Fact]
    public void MapStaticToStatic_OverwritesExistingValues()
    {
        var source = new FeberTestDto { Name = "New", Age = 99 };
        var dest = new FeberTestDto { Name = "Old", Age = 1 };

        FastMapper<FeberTestDto>.MapStaticToStatic(source, dest);

        Assert.Equal("New", dest.Name);
        Assert.Equal(99, dest.Age);
    }
}

// ========== CloneFactory Tests (binary ActionBuilder) ==========

public class CloneFactoryTests
{
    [Fact]
    public void Clone_SameType_CopiesAllProperties()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };

        var clone = source.Clone();

        Assert.Equal("Alice", clone.Name);
        Assert.Equal(30, clone.Age);
    }

    [Fact]
    public void Clone_SameType_ReturnsDifferentReference()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };

        var clone = source.Clone();

        Assert.NotSame(source, clone);
    }

    [Fact]
    public void Clone_CrossType_CopiesMatchingProperties()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };

        var clone = source.Clone<FeberTestDto, FeberTestDtoAlt>();

        Assert.Equal("Alice", clone.Name);
        Assert.Equal(30, clone.Age);
        Assert.Null(clone.Extra);
    }

    [Fact]
    public void Clone_ReadOnlyDestinationProperty_SkipsIt()
    {
        var source = new FeberTestDto { Name = "Alice", Age = 30 };

        var clone = source.Clone<FeberTestDto, FeberTestDtoReadOnly>();

        Assert.Equal("Alice", clone.Name);
        Assert.Equal(0, clone.Age); // readonly, not set
    }

    [Fact]
    public void Clone_PreservesPropertyValues_Exactly()
    {
        var date = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc);
        var source = new FeberTestDtoWithDate { Name = "Test", Created = date };

        var clone = source.Clone();

        Assert.Equal("Test", clone.Name);
        Assert.Equal(date, clone.Created);
        Assert.NotSame(source, clone);
    }

    [Fact]
    public void Clone_EmptyStringAndZero_CopiedCorrectly()
    {
        var source = new FeberTestDto { Name = "", Age = 0 };

        var clone = source.Clone();

        Assert.Equal("", clone.Name);
        Assert.Equal(0, clone.Age);
    }

    [Fact]
    public void Clone_NullStringProperty_CopiedCorrectly()
    {
        var source = new FeberTestDto { Name = null, Age = 42 };

        var clone = source.Clone();

        Assert.Null(clone.Name);
        Assert.Equal(42, clone.Age);
    }
}

// ========== PrettyPrinter Tests (unary ActionBuilder) ==========

public class PrettyPrinterTests
{
    [Fact]
    public void Print_WithStringAndIntProperties_DoesNotThrow()
    {
        var dto = new FeberTestDto { Name = "Alice", Age = 30 };

        var exception = Record.Exception(() => dto.Print());

        Assert.Null(exception);
    }

    [Fact]
    public void Print_WithDateTimeProperty_DoesNotThrow()
    {
        var dto = new FeberTestDtoWithDate { Name = "Test", Created = DateTime.UtcNow };

        var exception = Record.Exception(() => dto.Print());

        Assert.Null(exception);
    }

    [Fact]
    public void Print_WithNullStringProperty_DoesNotThrow()
    {
        var dto = new FeberTestDto { Name = null, Age = 0 };

        var exception = Record.Exception(() => dto.Print());

        Assert.Null(exception);
    }

    [Fact]
    public void Print_WritesToConsole()
    {
        var dto = new FeberTestDto { Name = "Alice", Age = 30 };

        var writer = new System.IO.StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);
        try
        {
            dto.Print();
            var output = writer.ToString();
            Assert.Contains("Name", output);
            Assert.Contains("Alice", output);
            Assert.Contains("Age", output);
            Assert.Contains("30", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}