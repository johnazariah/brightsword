using System.Diagnostics.CodeAnalysis;

namespace BrightSword.SwissKnife.Tests;

#region Test helpers

/// <summary>
/// Simple mock for <see cref="ITimedOperationObserver"/> that records calls.
/// </summary>
internal sealed class MockTimedOperationObserver : ITimedOperationObserver
{
    public bool StartedCalled { get; private set; }
    public bool SucceededCalled { get; private set; }
    public bool CompletedCalled { get; private set; }
    public TimeSpan? CompletedElapsed { get; private set; }
    public Exception? FailedException { get; private set; }

    public void Started() => StartedCalled = true;
    public void Succeeded() => SucceededCalled = true;
    public void Completed(TimeSpan elapsedTimeFromStart)
    {
        CompletedCalled = true;
        CompletedElapsed = elapsedTimeFromStart;
    }
    public void FailedWithException(Exception exception) => FailedException = exception;
}

// Types used by attribute tests
internal enum TestEnum
{
    [Name("First Value")]
    First,

    [Name("Second Value")]
    Second,

    NoName
}

[Tag("category", "test")]
[Tag("priority", 1)]
internal class TaggedClass
{
    [Tag("column", "id")]
    public int Id { get; set; }
}

// Types used by TypeMemberDiscoverer additional tests
internal interface IBaseInterface
{
    string BaseProp { get; }
    void BaseMethod();
    event EventHandler BaseEvent;
}

internal interface IDerivedInterface : IBaseInterface
{
    string DerivedProp { get; }
}

internal interface IDiamondA : IBaseInterface
{
    string PropA { get; }
}

internal interface IDiamondB : IBaseInterface
{
    string PropB { get; }
}

internal interface IDiamondBottom : IDiamondA, IDiamondB { }

internal class ConcreteClass
{
    public string Foo { get; set; } = "";
    public int Bar { get; set; }
    public void DoWork() { }
}

internal struct TestValueType
{
    public double X { get; set; }
    public double Y { get; set; }
}

// Types used by CodeGenerationUtilities tests
internal interface IMyService { }
internal interface IGenericService<T> { }
internal class MyClass { }

#endregion

#region TimedResult<T> Tests

public class TimedResultTests
{
    [Fact]
    public void Constructor_SetsResultAndElapsedTime()
    {
        var elapsed = TimeSpan.FromMilliseconds(42);
        var result = new TimedResult<int>(123, elapsed);

        Assert.Equal(123, result.Result);
        Assert.Equal(elapsed, result.ElapsedTime);
    }

    [Fact]
    public void Constructor_WithReferenceType_SetsResult()
    {
        var elapsed = TimeSpan.FromSeconds(1);
        var result = new TimedResult<string>("hello", elapsed);

        Assert.Equal("hello", result.Result);
        Assert.Equal(elapsed, result.ElapsedTime);
    }

    [Fact]
    public void Default_HasDefaultValues()
    {
        var result = default(TimedResult<int>);

        Assert.Equal(0, result.Result);
        Assert.Equal(TimeSpan.Zero, result.ElapsedTime);
    }
}

#endregion

#region TimedOperationExtensions Tests

public class TimedOperationExtensionsTests
{
    [Fact]
    public void Time_Action_Success_ReturnsTimeSpanAndCallsObserver()
    {
        var observer = new MockTimedOperationObserver();
        Action action = () => { /* no-op */ };

        var elapsed = action.Time(observer);

        Assert.True(observer.StartedCalled);
        Assert.True(observer.SucceededCalled);
        Assert.True(observer.CompletedCalled);
        Assert.Null(observer.FailedException);
        Assert.True(elapsed >= TimeSpan.Zero);
        Assert.NotNull(observer.CompletedElapsed);
    }

    [Fact]
    public void Time_Action_Exception_RethrowsAndCallsObserver()
    {
        var observer = new MockTimedOperationObserver();
        var expectedException = new InvalidOperationException("test failure");
        Action action = () => throw expectedException;

        var thrown = Assert.Throws<InvalidOperationException>(() => action.Time(observer));

        Assert.Same(expectedException, thrown);
        Assert.True(observer.StartedCalled);
        Assert.False(observer.SucceededCalled);
        Assert.True(observer.CompletedCalled);
        Assert.Same(expectedException, observer.FailedException);
    }

    [Fact]
    public void Time_Action_WithoutObserver_UsesDefaultTraceObserver()
    {
        Action action = () => { };

        // Should not throw; uses default TraceObserver
        var elapsed = action.Time();

        Assert.True(elapsed >= TimeSpan.Zero);
    }

    [Fact]
    public void Time_Func_Success_ReturnsTimedResultAndCallsObserver()
    {
        var observer = new MockTimedOperationObserver();
        Func<int> func = () => 42;

        var timedResult = func.Time(observer);

        Assert.Equal(42, timedResult.Result);
        Assert.True(timedResult.ElapsedTime >= TimeSpan.Zero);
        Assert.True(observer.StartedCalled);
        Assert.True(observer.SucceededCalled);
        Assert.True(observer.CompletedCalled);
        Assert.Null(observer.FailedException);
    }

    [Fact]
    public void Time_Func_Exception_RethrowsAndCallsObserver()
    {
        var observer = new MockTimedOperationObserver();
        var expectedException = new ArgumentException("bad arg");
        Func<string> func = () => throw expectedException;

        var thrown = Assert.Throws<ArgumentException>(() => func.Time(observer));

        Assert.Same(expectedException, thrown);
        Assert.True(observer.StartedCalled);
        Assert.False(observer.SucceededCalled);
        Assert.True(observer.CompletedCalled);
        Assert.Same(expectedException, observer.FailedException);
    }

    [Fact]
    public void Time_Func_WithoutObserver_UsesDefaultTraceObserver()
    {
        Func<string> func = () => "result";

        var timedResult = func.Time();

        Assert.Equal("result", timedResult.Result);
        Assert.True(timedResult.ElapsedTime >= TimeSpan.Zero);
    }

    [Fact]
    public void Time_Func_ReturnsReferenceType()
    {
        var observer = new MockTimedOperationObserver();
        var list = new List<int> { 1, 2, 3 };
        Func<List<int>> func = () => list;

        var timedResult = func.Time(observer);

        Assert.Same(list, timedResult.Result);
    }
}

#endregion

#region UniqueSequenceGenerator Tests

public class UniqueSequenceGeneratorTests
{
    [Fact]
    public void NextAscendingUniqueValue_IsIncreasing()
    {
        var first = UniqueSequenceGenerator.NextAscendingUniqueValue;
        var second = UniqueSequenceGenerator.NextAscendingUniqueValue;

        Assert.True(second > first);
    }

    [Fact]
    public void NextDescendingUniqueValue_IsDecreasing()
    {
        var first = UniqueSequenceGenerator.NextDescendingUniqueValue;
        var second = UniqueSequenceGenerator.NextDescendingUniqueValue;

        Assert.True(second < first);
    }

    [Fact]
    public void GenerateIncreasingSequence_ReturnsCorrectLength()
    {
        var seq = UniqueSequenceGenerator.GenerateIncreasingSequence(5).ToList();

        Assert.Equal(5, seq.Count);
    }

    [Fact]
    public void GenerateIncreasingSequence_ValuesAreIncreasing()
    {
        var seq = UniqueSequenceGenerator.GenerateIncreasingSequence(5).ToList();

        for (int i = 1; i < seq.Count; i++)
        {
            Assert.True(seq[i] > seq[i - 1]);
        }
    }

    [Fact]
    public void GenerateIncreasingSequence_ZeroLength_ReturnsEmpty()
    {
        var seq = UniqueSequenceGenerator.GenerateIncreasingSequence(0).ToList();

        Assert.Empty(seq);
    }

    [Fact]
    public void GenerateIncreasingSequence_NegativeLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => UniqueSequenceGenerator.GenerateIncreasingSequence(-1).ToList());
    }

    [Fact]
    public void GenerateDecreasingSequence_ReturnsCorrectLength()
    {
        var seq = UniqueSequenceGenerator.GenerateDecreasingSequence(5).ToList();

        Assert.Equal(5, seq.Count);
    }

    [Fact]
    public void GenerateDecreasingSequence_ValuesAreDecreasing()
    {
        var seq = UniqueSequenceGenerator.GenerateDecreasingSequence(5).ToList();

        for (int i = 1; i < seq.Count; i++)
        {
            Assert.True(seq[i] < seq[i - 1]);
        }
    }

    [Fact]
    public void GenerateDecreasingSequence_ZeroLength_ReturnsEmpty()
    {
        var seq = UniqueSequenceGenerator.GenerateDecreasingSequence(0).ToList();

        Assert.Empty(seq);
    }

    [Fact]
    public void GenerateDecreasingSequence_NegativeLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => UniqueSequenceGenerator.GenerateDecreasingSequence(-1).ToList());
    }
}

#endregion

#region BitTwiddlerExtensions Tests

public class BitTwiddlerExtensionsTests
{
    [Fact]
    public void GetReversedBytes_Ulong_ReversesBytes()
    {
        // 0x0102030405060708 in little-endian memory: 08 07 06 05 04 03 02 01
        // reversed: 01 02 03 04 05 06 07 08
        ulong value = 0x0102030405060708UL;
        var reversed = value.GetReversedBytes();

        Assert.Equal(8, reversed.Length);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, reversed);
    }

    [Fact]
    public void GetReversedBytes_Ulong_Zero()
    {
        ulong value = 0UL;
        var reversed = value.GetReversedBytes();

        Assert.Equal(8, reversed.Length);
        Assert.All(reversed, b => Assert.Equal(0, b));
    }

    [Fact]
    public void GetReversedBytes_Ulong_MaxValue()
    {
        ulong value = ulong.MaxValue;
        var reversed = value.GetReversedBytes();

        Assert.Equal(8, reversed.Length);
        Assert.All(reversed, b => Assert.Equal(0xFF, b));
    }

    [Fact]
    public void GetReversedBytes_Long_ReversesBytes()
    {
        long value = 0x0102030405060708L;
        var reversed = value.GetReversedBytes();

        Assert.Equal(8, reversed.Length);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, reversed);
    }

    [Fact]
    public void GetReversedBytes_Long_NegativeValue()
    {
        long value = -1L; // All 0xFF bytes
        var reversed = value.GetReversedBytes();

        Assert.Equal(8, reversed.Length);
        Assert.All(reversed, b => Assert.Equal(0xFF, b));
    }

    [Fact]
    public void GetReversedBytes_Ulong_SingleByte()
    {
        // Only lowest byte set: memory is [0x42, 0, 0, 0, 0, 0, 0, 0]
        // reversed: [0, 0, 0, 0, 0, 0, 0, 0x42]
        ulong value = 0x42UL;
        var reversed = value.GetReversedBytes();

        Assert.Equal(0x00, reversed[0]);
        Assert.Equal(0x42, reversed[7]);
    }

    [Fact]
    public void GetReversedBytes_Long_IsConsistentWithBitConverter()
    {
        long value = 0x1122334455667788L;
        var reversed = value.GetReversedBytes();
        var originalBytes = BitConverter.GetBytes(value);
        Array.Reverse(originalBytes);

        Assert.Equal(originalBytes, reversed);
    }
}

#endregion

#region SequentialGuid Tests

public class SequentialGuidTests
{
    [Fact]
    public void NewSequentialGuid_ReturnsNonEmptyGuid()
    {
        var guid = SequentialGuid.NewSequentialGuid();

        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void NewReverseSequentialGuid_ReturnsNonEmptyGuid()
    {
        var guid = SequentialGuid.NewReverseSequentialGuid();

        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void NewSequentialGuid_TwoCallsReturnDifferentGuids()
    {
        var guid1 = SequentialGuid.NewSequentialGuid();
        var guid2 = SequentialGuid.NewSequentialGuid();

        Assert.NotEqual(guid1, guid2);
    }

    [Fact]
    public void NewReverseSequentialGuid_TwoCallsReturnDifferentGuids()
    {
        var guid1 = SequentialGuid.NewReverseSequentialGuid();
        var guid2 = SequentialGuid.NewReverseSequentialGuid();

        Assert.NotEqual(guid1, guid2);
    }

    [Fact]
    public void NewSequentialGuid_WithParameters_OverridesDefaults()
    {
        var guid = SequentialGuid.NewSequentialGuid(a: 0x12345678, b: 0x1234, c: 0x5678);

        var bytes = guid.ToByteArray();
        // First 4 bytes correspond to 'a' (little-endian in Guid)
        var a = BitConverter.ToInt32(bytes, 0);
        var b = BitConverter.ToInt16(bytes, 4);
        var c = BitConverter.ToInt16(bytes, 6);

        Assert.Equal(0x12345678, a);
        Assert.Equal(0x1234, b);
        Assert.Equal(0x5678, c);
    }

    [Fact]
    public void NewReverseSequentialGuid_WithParameters_OverridesDefaults()
    {
        var guid = SequentialGuid.NewReverseSequentialGuid(a: 0x11111111, b: 0x2222, c: 0x3333);

        var bytes = guid.ToByteArray();
        var a = BitConverter.ToInt32(bytes, 0);
        var b = BitConverter.ToInt16(bytes, 4);
        var c = BitConverter.ToInt16(bytes, 6);

        Assert.Equal(0x11111111, a);
        Assert.Equal(0x2222, b);
        Assert.Equal(0x3333, c);
    }
}

#endregion

#region NameAttribute Tests

public class NameAttributeTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var attr = new NameAttribute("Test Name");

        Assert.Equal("Test Name", attr.Value);
    }

    [Fact]
    public void Attribute_CanBeRetrievedViaReflection()
    {
        var field = typeof(TestEnum).GetField(nameof(TestEnum.First))!;
        var attr = field.GetCustomAttribute<NameAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("First Value", attr!.Value);
    }

    [Fact]
    public void Attribute_SecondField_HasCorrectValue()
    {
        var field = typeof(TestEnum).GetField(nameof(TestEnum.Second))!;
        var attr = field.GetCustomAttribute<NameAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("Second Value", attr!.Value);
    }

    [Fact]
    public void Field_WithoutAttribute_ReturnsNull()
    {
        var field = typeof(TestEnum).GetField(nameof(TestEnum.NoName))!;
        var attr = field.GetCustomAttribute<NameAttribute>();

        Assert.Null(attr);
    }

    [Fact]
    public void Attribute_IsAttributeUsageField()
    {
        var usage = typeof(NameAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Field, usage!.ValidOn);
    }
}

#endregion

#region TagAttribute Tests

public class TagAttributeTests
{
    [Fact]
    public void Constructor_SetsNameAndValue()
    {
        var attr = new TagAttribute("key", "val");

        Assert.Equal("key", attr.Name);
        Assert.Equal("val", attr.Value);
    }

    [Fact]
    public void Constructor_NullValue_Allowed()
    {
        var attr = new TagAttribute("flag", null);

        Assert.Equal("flag", attr.Name);
        Assert.Null(attr.Value);
    }

    [Fact]
    public void Attribute_CanBeRetrievedViaReflection_Class()
    {
        var attrs = typeof(TaggedClass).GetCustomAttributes<TagAttribute>().ToList();

        Assert.Equal(2, attrs.Count);
        Assert.Contains(attrs, a => a.Name == "category" && (string)a.Value! == "test");
        Assert.Contains(attrs, a => a.Name == "priority" && (int)a.Value! == 1);
    }

    [Fact]
    public void Attribute_CanBeRetrievedViaReflection_Property()
    {
        var prop = typeof(TaggedClass).GetProperty(nameof(TaggedClass.Id))!;
        var attr = prop.GetCustomAttribute<TagAttribute>();

        Assert.NotNull(attr);
        Assert.Equal("column", attr!.Name);
        Assert.Equal("id", attr.Value);
    }

    [Fact]
    public void Attribute_AllowsMultiple()
    {
        var usage = typeof(TagAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        Assert.NotNull(usage);
        Assert.True(usage!.AllowMultiple);
        Assert.True(usage.Inherited);
    }
}

#endregion

#region CodeGenerationUtilities Tests

public class CodeGenerationUtilitiesTests
{
    [Fact]
    public void RenameToConcreteType_Interface_StripsIPrefix()
    {
        var result = typeof(IMyService).RenameToConcreteType("", "");

        Assert.Equal("MyService", result);
    }

    [Fact]
    public void RenameToConcreteType_Class_KeepsName()
    {
        var result = typeof(MyClass).RenameToConcreteType("", "");

        Assert.Equal("MyClass", result);
    }

    [Fact]
    public void RenameToConcreteType_Primitive_KeepsName()
    {
        var result = typeof(int).RenameToConcreteType("", "");

        Assert.Equal("Int32", result);
    }

    [Fact]
    public void RenameToConcreteType_WithPrefixSuffix_Class()
    {
        var result = typeof(MyClass).RenameToConcreteType("Pre_", "_Suf");

        Assert.Equal("Pre_MyClass_Suf", result);
    }

    [Fact]
    public void RenameToConcreteType_WithPrefixSuffix_Primitive()
    {
        var result = typeof(int).RenameToConcreteType("Pre_", "_Suf");

        Assert.Equal("Pre_Int32_Suf", result);
    }

    [Fact]
    public void RenameToConcreteType_GenericTypeDefinition_Interface_StripsI()
    {
        // typeof(IGenericService<>) is a generic type definition and an interface
        var result = typeof(IGenericService<>).RenameToConcreteType("", "");

        // Should strip leading I from "IGenericService`1"
        Assert.Equal("GenericService`1", result);
    }

    [Fact]
    public void RenameToConcreteType_ClosedGenericType_Interface()
    {
        // typeof(IGenericService<int>) is a constructed generic type
        var result = typeof(IGenericService<int>).RenameToConcreteType("", "");

        // PrintableName with transformer is called; generic args get transformed
        Assert.Contains("GenericService", result);
    }

    [Fact]
    public void RenameToConcreteType_GenericTypeDefinition_Class()
    {
        // List<> is a generic type definition and a class
        var result = typeof(List<>).RenameToConcreteType("", "");

        Assert.Equal("List`1", result);
    }

    [Fact]
    public void RenameToConcreteType_ClosedGenericClass()
    {
        var result = typeof(List<int>).RenameToConcreteType("", "");

        Assert.Contains("List", result);
        Assert.Contains("Int32", result);
    }
}

#endregion

#region TypeMemberDiscoverer Additional Tests

public class TypeMemberDiscovererAdditionalTests
{
    [Fact]
    public void GetAllProperties_InheritedInterface_IncludesBaseProperties()
    {
        var props = typeof(IDerivedInterface).GetAllProperties().Select(p => p.Name).ToList();

        Assert.Contains("DerivedProp", props);
        Assert.Contains("BaseProp", props);
    }

    [Fact]
    public void GetAllMethods_Interface_IncludesBaseMethods()
    {
        var methods = typeof(IDerivedInterface).GetAllMethods().Select(m => m.Name).ToList();

        Assert.Contains("BaseMethod", methods);
    }

    [Fact]
    public void GetAllEvents_Interface_IncludesBaseEvents()
    {
        var events = typeof(IDerivedInterface).GetAllEvents().Select(e => e.Name).ToList();

        // IDerivedInterface inherits from IBaseInterface which has BaseEvent
        Assert.Contains("BaseEvent", events);
    }

    [Fact]
    public void GetAllProperties_DiamondInheritance_DoesNotDuplicate()
    {
        var props = typeof(IDiamondBottom).GetAllProperties().Select(p => p.Name).ToList();

        Assert.Contains("PropA", props);
        Assert.Contains("PropB", props);
        Assert.Contains("BaseProp", props);
    }

    [Fact]
    public void GetAllProperties_Class_ReturnsPublicProperties()
    {
        var props = typeof(ConcreteClass).GetAllProperties().Select(p => p.Name).ToList();

        Assert.Contains("Foo", props);
        Assert.Contains("Bar", props);
    }

    [Fact]
    public void GetAllMethods_Class_ReturnsPublicMethods()
    {
        var methods = typeof(ConcreteClass).GetAllMethods().Select(m => m.Name).ToList();

        Assert.Contains("DoWork", methods);
    }

    [Fact]
    public void GetAllProperties_Struct_ReturnsProperties()
    {
        var props = typeof(TestValueType).GetAllProperties().Select(p => p.Name).ToList();

        Assert.Contains("X", props);
        Assert.Contains("Y", props);
    }

    [Fact]
    public void GetProperty_WithWalk_FindsInheritedProperty()
    {
        var prop = typeof(IDerivedInterface).GetProperty("BaseProp", walkInterfaceInheritanceHierarchy: true);

        Assert.NotNull(prop);
        Assert.Equal("BaseProp", prop!.Name);
    }

    [Fact]
    public void GetProperty_WithoutWalk_DoesNotFindInheritedProperty()
    {
        var prop = typeof(IDerivedInterface).GetProperty("BaseProp", walkInterfaceInheritanceHierarchy: false);

        // Without walking hierarchy, GetProperty on interface won't find base property
        Assert.Null(prop);
    }

    [Fact]
    public void GetProperty_WithWalk_FindsOwnProperty()
    {
        var prop = typeof(IDerivedInterface).GetProperty("DerivedProp", walkInterfaceInheritanceHierarchy: true);

        Assert.NotNull(prop);
        Assert.Equal("DerivedProp", prop!.Name);
    }

    [Fact]
    public void GetProperty_WithoutWalk_FindsOwnProperty()
    {
        var prop = typeof(IDerivedInterface).GetProperty("DerivedProp", walkInterfaceInheritanceHierarchy: false);

        Assert.NotNull(prop);
    }

    [Fact]
    public void GetProperty_NonExistent_ReturnsNull()
    {
        var prop = typeof(IDerivedInterface).GetProperty("NonExistent", walkInterfaceInheritanceHierarchy: true);

        Assert.Null(prop);
    }
}

#endregion

#region TypeMemberDiscoverer<T> Generic Tests

public class TypeMemberDiscovererGenericTests
{
    [Fact]
    public void GetAllProperties_CachedVersion_ReturnsProperties()
    {
        var props = TypeMemberDiscoverer<IDerivedInterface>.GetAllProperties().Select(p => p.Name).ToList();

        Assert.Contains("DerivedProp", props);
        Assert.Contains("BaseProp", props);
    }

    [Fact]
    public void GetAllMethods_CachedVersion_ReturnsMethods()
    {
        var methods = TypeMemberDiscoverer<IBaseInterface>.GetAllMethods().Select(m => m.Name).ToList();

        Assert.Contains("BaseMethod", methods);
    }

    [Fact]
    public void GetAllEvents_CachedVersion_ReturnsEvents()
    {
        var events = TypeMemberDiscoverer<IBaseInterface>.GetAllEvents().Select(e => e.Name).ToList();

        Assert.Contains("BaseEvent", events);
    }

    [Fact]
    public void GetAllProperties_CachedVersion_SameInstanceOnSecondCall()
    {
        var first = TypeMemberDiscoverer<ConcreteClass>.GetAllProperties();
        var second = TypeMemberDiscoverer<ConcreteClass>.GetAllProperties();

        // Lazy<T> returns the same instance
        Assert.Same(first, second);
    }
}

#endregion

#region CommandLineArgumentHelper Tests

public class CommandLineArgumentHelperTests
{
    [Fact]
    public void Parse_KeyValuePair()
    {
        var helper = new CommandLineArgumentHelper("--name=John");

        Assert.Equal("John", helper["name"]);
    }

    [Fact]
    public void Parse_FlagOnly()
    {
        var helper = new CommandLineArgumentHelper("--verbose");

        Assert.True(helper.IsArgumentSpecified("verbose"));
        Assert.Null(helper["verbose"]);
    }

    [Fact]
    public void Parse_MultipleArgs()
    {
        var helper = new CommandLineArgumentHelper("--host=localhost", "--port=8080", "--debug");

        Assert.Equal("localhost", helper["host"]);
        Assert.Equal("8080", helper["port"]);
        Assert.Null(helper["debug"]);
        Assert.Equal(3, helper.Count);
    }

    [Fact]
    public void HelpRequested_Help()
    {
        var helper = new CommandLineArgumentHelper("--help");

        Assert.True(helper.HelpRequested);
    }

    [Fact]
    public void HelpRequested_Usage()
    {
        var helper = new CommandLineArgumentHelper("--usage");

        Assert.True(helper.HelpRequested);
    }

    [Fact]
    public void HelpRequested_False_WhenNotPresent()
    {
        var helper = new CommandLineArgumentHelper("--foo=bar");

        Assert.False(helper.HelpRequested);
    }

    [Fact]
    public void Count_ReturnsNumberOfArguments()
    {
        var helper = new CommandLineArgumentHelper("--a=1", "--b=2", "--c=3");

        Assert.Equal(3, helper.Count);
    }

    [Fact]
    public void Count_EmptyArgs()
    {
        var helper = new CommandLineArgumentHelper();

        Assert.Equal(0, helper.Count);
    }

    [Fact]
    public void Indexer_MissingArgument_Throws()
    {
        var helper = new CommandLineArgumentHelper("--a=1");

        Assert.Throws<ArgumentOutOfRangeException>(() => helper["missing"]);
    }

    [Fact]
    public void GetArgumentValue_ExistingKey_ConvertsValue()
    {
        var helper = new CommandLineArgumentHelper("--port=8080");

        var port = helper.GetArgumentValue("port", s => int.Parse(s!), 0);

        Assert.Equal(8080, port);
    }

    [Fact]
    public void GetArgumentValue_MissingKey_ReturnsDefault()
    {
        var helper = new CommandLineArgumentHelper("--other=x");

        var port = helper.GetArgumentValue("port", s => int.Parse(s!), 3000);

        Assert.Equal(3000, port);
    }

    [Fact]
    public void IsArgumentSpecified_True_WhenPresent()
    {
        var helper = new CommandLineArgumentHelper("--verbose");

        Assert.True(helper.IsArgumentSpecified("verbose"));
    }

    [Fact]
    public void IsArgumentSpecified_False_WhenAbsent()
    {
        var helper = new CommandLineArgumentHelper("--other");

        Assert.False(helper.IsArgumentSpecified("verbose"));
    }

    [Fact]
    public void Parse_CaseInsensitive()
    {
        var helper = new CommandLineArgumentHelper("--Name=Alice");

        Assert.True(helper.IsArgumentSpecified("name"));
        Assert.True(helper.IsArgumentSpecified("NAME"));
    }

    [Fact]
    public void Parse_WithoutDashPrefix()
    {
        // Args without -- prefix should still be parsed (name = parts[0])
        var helper = new CommandLineArgumentHelper("foo=bar");

        Assert.Equal("bar", helper["foo"]);
    }

    [Fact]
    public void GetArgumentValue_Flag_PassesNullToConverter()
    {
        var helper = new CommandLineArgumentHelper("--verbose");

        var result = helper.GetArgumentValue("verbose", s => s == null ? "flag" : s, "default");

        Assert.Equal("flag", result);
    }
}

#endregion

#region CommandLineProcessorExtensions Tests

#pragma warning disable CS0618 // Suppress obsolete warnings for testing
public class CommandLineProcessorExtensionsTests
{
    [Fact]
    public void GetArgumentValue_DelegatesToHelper()
    {
        var args = new[] { "--port=9090" };

        var port = args.GetArgumentValue("port", s => int.Parse(s!), 0);

        Assert.Equal(9090, port);
    }

    [Fact]
    public void GetArgumentValue_MissingKey_ReturnsDefault()
    {
        var args = new[] { "--other=x" };

        var port = args.GetArgumentValue("port", s => int.Parse(s!), 3000);

        Assert.Equal(3000, port);
    }

    [Fact]
    public void HelpRequested_DelegatesToHelper_True()
    {
        var args = new[] { "--help" };

        Assert.True(args.HelpRequested());
    }

    [Fact]
    public void HelpRequested_DelegatesToHelper_False()
    {
        var args = new[] { "--foo" };

        Assert.False(args.HelpRequested());
    }

    [Fact]
    public void IsArgumentSpecified_DelegatesToHelper_True()
    {
        var args = new[] { "--verbose" };

        Assert.True(args.IsArgumentSpecified("verbose"));
    }

    [Fact]
    public void IsArgumentSpecified_DelegatesToHelper_False()
    {
        var args = new[] { "--other" };

        Assert.False(args.IsArgumentSpecified("verbose"));
    }
}
#pragma warning restore CS0618

#endregion
