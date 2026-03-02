using BrightSword.Squid.Extensions;
using Xunit;

namespace BrightSword.Squid.Tests;

public class TypeMapExtensionsTests
{
    #region MapGenericTypeIfPossible

    [Fact]
    public void MapGenericTypeIfPossible_MatchingGenericType_ReturnsRemappedType()
    {
        var result = typeof(IList<int>).MapGenericTypeIfPossible(
            typeof(List<>),
            typeof(IList<>));

        Assert.NotNull(result);
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void MapGenericTypeIfPossible_NonGenericType_ReturnsNull()
    {
        var result = typeof(string).MapGenericTypeIfPossible(
            typeof(List<int>),
            typeof(List<int>));

        Assert.Null(result);
    }

    [Fact]
    public void MapGenericTypeIfPossible_DifferentGenericArgCount_ReturnsNull()
    {
        var result = typeof(IDictionary<string, int>).MapGenericTypeIfPossible(
            typeof(IList<int>),
            typeof(List<int>));

        Assert.Null(result);
    }

    [Fact]
    public void MapGenericTypeIfPossible_WithMultipleMappableTypes_RemapsArguments()
    {
        var result = typeof(IList<int>).MapGenericTypeIfPossible(
            typeof(List<>),
            typeof(IList<>), typeof(ICollection<>), typeof(IEnumerable<>));

        Assert.NotNull(result);
        Assert.Equal(typeof(List<int>), result);
    }

    [Fact]
    public void MapGenericTypeIfPossible_NonAssignableMappableType_ReturnsNull()
    {
        var result = typeof(IList<int>).MapGenericTypeIfPossible(
            typeof(List<>),
            typeof(ISet<>));

        Assert.Null(result);
    }

    [Fact]
    public void MapGenericTypeIfPossible_IDictionaryToDict_Works()
    {
        var result = typeof(IDictionary<string, int>).MapGenericTypeIfPossible(
            typeof(Dictionary<,>),
            typeof(IDictionary<,>));

        Assert.NotNull(result);
        Assert.Equal(typeof(Dictionary<string, int>), result);
    }

    #endregion

    #region MapTypeIfPossible

    [Fact]
    public void MapTypeIfPossible_AssignableType_ReturnsMappedType()
    {
        var result = typeof(ICloneable).MapTypeIfPossible(
            typeof(object),
            typeof(string));

        Assert.Equal(typeof(object), result);
    }

    [Fact]
    public void MapTypeIfPossible_NonAssignableType_ReturnsNull()
    {
        var result = typeof(ICloneable).MapTypeIfPossible(
            typeof(object),
            typeof(int));

        Assert.Null(result);
    }

    [Fact]
    public void MapTypeIfPossible_MultipleMappableTypes_ReturnsFirstMatch()
    {
        var result = typeof(ICloneable).MapTypeIfPossible(
            typeof(object),
            typeof(int),
            typeof(string));

        Assert.Equal(typeof(object), result);
    }

    [Fact]
    public void MapTypeIfPossible_NoMappableTypes_ReturnsNull()
    {
        var result = typeof(ICloneable).MapTypeIfPossible(typeof(object));

        Assert.Null(result);
    }

    [Fact]
    public void MapTypeIfPossible_EmptyMappableTypes_ReturnsNull()
    {
        var result = typeof(ICloneable).MapTypeIfPossible(
            typeof(object),
            Array.Empty<Type>());

        Assert.Null(result);
    }

    #endregion
}
