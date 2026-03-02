using System.Reflection;
using BrightSword.Squid.Extensions;
using Xunit;

namespace BrightSword.Squid.Tests;

public class TypeExtensionsTests
{
    #region GetNonGenericPartOfClassName

    [Fact]
    public void GetNonGenericPartOfClassName_NonGenericInterface_ReturnsNameWithoutI()
    {
        var result = typeof(ITestSimple).GetNonGenericPartOfClassName();

        Assert.Equal("TestSimple", result);
    }

    [Fact]
    public void GetNonGenericPartOfClassName_GenericType_ReturnsWithoutGenericPart()
    {
        var result = typeof(List<int>).GetNonGenericPartOfClassName();

        Assert.Equal("List", result);
    }

    [Fact]
    public void GetNonGenericPartOfClassName_GenericInterface_ReturnsWithoutGenericPart()
    {
        var result = typeof(IList<string>).GetNonGenericPartOfClassName();

        Assert.Equal("List", result);
    }

    [Fact]
    public void GetNonGenericPartOfClassName_DictionaryGenericType_ReturnsWithoutGenericPart()
    {
        var result = typeof(IDictionary<string, int>).GetNonGenericPartOfClassName();

        Assert.Equal("Dictionary", result);
    }

    #endregion

    #region GetGenericMethodOnType

    [Fact]
    public void GetGenericMethodOnType_FindsAndInstantiatesMethod()
    {
        var method = typeof(System.Threading.Interlocked).GetGenericMethodOnType("CompareExchange", typeof(EventHandler));

        Assert.NotNull(method);
        Assert.False(method.IsGenericMethodDefinition);
        Assert.Equal(typeof(EventHandler), method.ReturnType);
    }

    [Fact]
    public void GetGenericMethodOnType_WithDifferentTypeParam_Works()
    {
        var method = typeof(System.Threading.Interlocked).GetGenericMethodOnType("CompareExchange", typeof(string));

        Assert.NotNull(method);
        Assert.Equal(typeof(string), method.ReturnType);
    }

    #endregion

    #region GetAllNonExcludedProperties

    [Fact]
    public void GetAllNonExcludedProperties_NoExclusions_ReturnsAll()
    {
        var props = typeof(IExtendedTestInterface).GetAllNonExcludedProperties().ToList();

        Assert.Contains(props, p => p.Name == "BaseName");
        Assert.Contains(props, p => p.Name == "ExtendedValue");
    }

    [Fact]
    public void GetAllNonExcludedProperties_WithExclusion_ExcludesBaseProperties()
    {
        var props = typeof(IExtendedTestInterface)
            .GetAllNonExcludedProperties(typeof(IBaseTestInterface))
            .ToList();

        Assert.DoesNotContain(props, p => p.Name == "BaseName");
        Assert.Contains(props, p => p.Name == "ExtendedValue");
    }

    #endregion

    #region GetAllNonExcludedMethods

    [Fact]
    public void GetAllNonExcludedMethods_NoExclusions_ReturnsAll()
    {
        var methods = typeof(IMethodTestInterface).GetAllNonExcludedMethods().ToList();

        Assert.Contains(methods, m => m.Name == "DoWork");
    }

    [Fact]
    public void GetAllNonExcludedMethods_WithExclusion_ExcludesBaseMethods()
    {
        var methods = typeof(ICloneableMethodInterface)
            .GetAllNonExcludedMethods(typeof(ICloneable))
            .ToList();

        Assert.DoesNotContain(methods, m => m.Name == "Clone");
    }

    #endregion

    #region GetAllNonExcludedEvents

    [Fact]
    public void GetAllNonExcludedEvents_NoExclusions_ReturnsAll()
    {
        var events = typeof(IEventTestInterface).GetAllNonExcludedEvents().ToList();

        Assert.Contains(events, e => e.Name == "Changed");
    }

    [Fact]
    public void GetAllNonExcludedEvents_WithExclusion_ExcludesBaseEvents()
    {
        var events = typeof(IEventTestInterface)
            .GetAllNonExcludedEvents(typeof(IEventTestInterface))
            .ToList();

        Assert.Empty(events);
    }

    #endregion

    #region Test Interfaces

    public interface ITestSimple
    {
        string Name { get; set; }
    }

    public interface IBaseTestInterface
    {
        string BaseName { get; set; }
    }

    public interface IExtendedTestInterface : IBaseTestInterface
    {
        int ExtendedValue { get; set; }
    }

    public interface IMethodTestInterface
    {
        void DoWork();
    }

    public interface ICloneableMethodInterface : ICloneable
    {
        string Name { get; set; }
    }

    public interface IEventTestInterface
    {
        event EventHandler Changed;
    }

    #endregion
}
