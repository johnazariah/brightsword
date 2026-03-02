using System.Dynamic;
using System.Linq.Expressions;
using BrightSword.Feber.Samples;

namespace BrightSword.Feber.Tests;

// ========== FastComparer Tests (binary FunctionBuilder) ==========

public class FastComparerTests
{
    [Fact]
    public void AllPropertiesAreEqual_EqualObjects_ReturnsTrue()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Alice", Age = 30 };

        Assert.True(FastComparer<FeberTestDto>.AllPropertiesAreEqual(a, b));
    }

    [Fact]
    public void AllPropertiesAreEqual_DifferentName_ReturnsFalse()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Bob", Age = 30 };

        Assert.False(FastComparer<FeberTestDto>.AllPropertiesAreEqual(a, b));
    }

    [Fact]
    public void AllPropertiesAreEqual_DifferentAge_ReturnsFalse()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Alice", Age = 25 };

        Assert.False(FastComparer<FeberTestDto>.AllPropertiesAreEqual(a, b));
    }

    [Fact]
    public void AllPropertiesAreEqual_AllDifferent_ReturnsFalse()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Bob", Age = 25 };

        Assert.False(FastComparer<FeberTestDto>.AllPropertiesAreEqual(a, b));
    }

    [Fact]
    public void AllPropertiesAreEqualWith_ExtensionMethod_EqualObjects_ReturnsTrue()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Alice", Age = 30 };

        Assert.True(a.AllPropertiesAreEqualWith(b));
    }

    [Fact]
    public void AllPropertiesAreEqualWith_ExtensionMethod_DifferentObjects_ReturnsFalse()
    {
        var a = new FeberTestDto { Name = "Alice", Age = 30 };
        var b = new FeberTestDto { Name = "Bob", Age = 25 };

        Assert.False(a.AllPropertiesAreEqualWith(b));
    }

    [Fact]
    public void AllPropertiesAreEqual_WithDateTimeProperty_EqualDates_ReturnsTrue()
    {
        var date = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var a = new FeberTestDtoWithDate { Name = "Alice", Created = date };
        var b = new FeberTestDtoWithDate { Name = "Alice", Created = date };

        Assert.True(FastComparer<FeberTestDtoWithDate>.AllPropertiesAreEqual(a, b));
    }

    [Fact]
    public void AllPropertiesAreEqual_WithDateTimeProperty_DifferentDates_ReturnsFalse()
    {
        var a = new FeberTestDtoWithDate { Name = "Alice", Created = new DateTime(2024, 1, 1) };
        var b = new FeberTestDtoWithDate { Name = "Alice", Created = new DateTime(2024, 6, 1) };

        Assert.False(FastComparer<FeberTestDtoWithDate>.AllPropertiesAreEqual(a, b));
    }
}

// ========== SimpleSerializer Tests (unary FunctionBuilder) ==========

public class SimpleSerializerTests
{
    [Fact]
    public void Serialize_ProducesExpectedFormat()
    {
        var dto = new FeberTestDto { Name = "Alice", Age = 30 };

        var result = SimpleSerializer<FeberTestDto>.Serialize(dto);

        Assert.Contains("Name:Alice,", result);
        Assert.Contains("Age:30,", result);
        Assert.StartsWith("{", result);
        Assert.EndsWith("}", result);
    }

    [Fact]
    public void Serialize_ExtensionMethod_ProducesExpectedFormat()
    {
        var dto = new FeberTestDto { Name = "Bob", Age = 25 };

        var result = dto.Serialize();

        Assert.Contains("Name:Bob,", result);
        Assert.Contains("Age:25,", result);
    }

    [Fact]
    public void Serialize_WithDateTimeProperty_UsesRoundTripFormat()
    {
        var date = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc);
        var dto = new FeberTestDtoWithDate { Name = "Test", Created = date };

        var result = dto.Serialize();

        Assert.Contains("Name:Test,", result);
        var expectedDateStr = date.ToString("O");
        Assert.Contains($"Created:{expectedDateStr},", result);
    }

    [Fact]
    public void Serialize_EmptyString_IncludesPropertyName()
    {
        var dto = new FeberTestDto { Name = "", Age = 0 };

        var result = dto.Serialize();

        Assert.Contains("Name:,", result);
        Assert.Contains("Age:0,", result);
    }

    [Fact]
    public void Serialize_Result_WrappedInBraces()
    {
        var dto = new FeberTestDto { Name = "X", Age = 1 };

        var result = dto.Serialize();

        Assert.StartsWith("{", result);
        Assert.EndsWith("}", result);
    }
}

// ========== DynamicExpressionUtilities Tests ==========

public class DynamicExpressionUtilitiesTests
{
    [Fact]
    public void GetDynamicPropertyAccessorExpression_CompilesAndReadsValue()
    {
        dynamic source = new ExpandoObject();
        source.Name = "Alice";

        var param = Expression.Parameter(typeof(object), "_instance");
        var propertyInfo = typeof(FeberTestDto).GetProperty("Name")!;
        var accessorExpr = param.GetDynamicPropertyAccessorExpression<FeberTestDto>(propertyInfo);

        var lambda = Expression.Lambda<Func<object, string>>(accessorExpr, param);
        var func = lambda.Compile();

        var result = func((object)source);

        Assert.Equal("Alice", result);
    }

    [Fact]
    public void GetDynamicPropertyAccessorExpression_WithNameAndType_CompilesAndReadsValue()
    {
        dynamic source = new ExpandoObject();
        source.Age = 42;

        var param = Expression.Parameter(typeof(object), "_instance");
        var accessorExpr = param.GetDynamicPropertyAccessorExpression<FeberTestDto>("Age", typeof(int));

        var lambda = Expression.Lambda<Func<object, int>>(accessorExpr, param);
        var func = lambda.Compile();

        var result = func((object)source);

        Assert.Equal(42, result);
    }

    [Fact]
    public void GetDynamicPropertyMutatorExpression_CompilesAndSetsValue()
    {
        dynamic target = new ExpandoObject();

        var param = Expression.Parameter(typeof(object), "_instance");
        var propertyInfo = typeof(FeberTestDto).GetProperty("Name")!;
        var valueExpr = Expression.Constant("Bob", typeof(string));
        var mutatorExpr = param.GetDynamicPropertyMutatorExpression<FeberTestDto>(propertyInfo, valueExpr);

        var lambda = Expression.Lambda<Action<object>>(mutatorExpr, param);
        var action = lambda.Compile();

        action((object)target);

        Assert.Equal("Bob", (string)target.Name);
    }

    [Fact]
    public void GetDynamicPropertyMutatorExpression_WithName_CompilesAndSetsValue()
    {
        dynamic target = new ExpandoObject();

        var param = Expression.Parameter(typeof(object), "_instance");
        var valueExpr = Expression.Constant(99, typeof(int));
        var mutatorExpr = param.GetDynamicPropertyMutatorExpression<FeberTestDto>("Age", Expression.Convert(valueExpr, typeof(object)));

        var lambda = Expression.Lambda<Action<object>>(mutatorExpr, param);
        var action = lambda.Compile();

        action((object)target);

        Assert.Equal(99, (int)target.Age);
    }
}