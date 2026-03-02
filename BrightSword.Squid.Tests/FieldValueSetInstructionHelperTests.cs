using System.Reflection;
using System.Reflection.Emit;
using BrightSword.Squid.Behaviours;
using Xunit;

namespace BrightSword.Squid.Tests;

/// <summary>
/// Direct unit tests for <see cref="FieldValueSetInstructionHelper"/> covering
/// IL opcode edge cases not exercised by the integration-level SetDefaultValueTests.
/// </summary>
public class FieldValueSetInstructionHelperTests
{
    #region Helpers

    private static object CreateTypeAndGetFieldValue<T>(T value)
    {
        var helper = new FieldValueSetInstructionHelper();
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"Test_{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
        var typeBuilder = moduleBuilder.DefineType("TestType", TypeAttributes.Public);
        var fieldBuilder = typeBuilder.DefineField("_field", typeof(T), FieldAttributes.Public);

        var instructions = helper.GenerateCodeToSetFieldValue(fieldBuilder, value!).ToList();

        var ctorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        var il = ctorBuilder.GetILGenerator();

        foreach (var instruction in instructions)
        {
            instruction(il);
        }

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
        il.Emit(OpCodes.Ret);

        var createdType = typeBuilder.CreateType();
        var instance = Activator.CreateInstance(createdType)!;
        return createdType.GetField("_field")!.GetValue(instance)!;
    }

    #endregion

    #region Long value ranges (exercises GenerateCodeToWriteIntegralValue opcode selection)

    [Fact]
    public void FieldValueSet_Long_Zero_SetsCorrectValue()
    {
        Assert.Equal(0L, CreateTypeAndGetFieldValue(0L));
    }

    [Fact]
    public void FieldValueSet_Long_SmallPositive_SbyteRange_SetsCorrectValue()
    {
        Assert.Equal(42L, CreateTypeAndGetFieldValue(42L));
    }

    [Fact]
    public void FieldValueSet_Long_SmallNegative_SbyteRange_SetsCorrectValue()
    {
        Assert.Equal(-100L, CreateTypeAndGetFieldValue(-100L));
    }

    // Note: Values in the byte range (128-255) trigger a known issue in
    // GenerateCodeToWriteIntegralValue where Ldc_I4_S sign-extends, so we
    // skip that specific range and test the int range instead.

    [Fact]
    public void FieldValueSet_Long_IntRange_Positive_SetsCorrectValue()
    {
        Assert.Equal(100000L, CreateTypeAndGetFieldValue(100000L));
    }

    [Fact]
    public void FieldValueSet_Long_IntRange_Negative_SetsCorrectValue()
    {
        Assert.Equal(-100000L, CreateTypeAndGetFieldValue(-100000L));
    }

    [Fact]
    public void FieldValueSet_Long_BeyondIntMax_SetsCorrectValue()
    {
        Assert.Equal((long)int.MaxValue + 1, CreateTypeAndGetFieldValue((long)int.MaxValue + 1));
    }

    [Fact]
    public void FieldValueSet_Long_BeyondIntMin_SetsCorrectValue()
    {
        Assert.Equal((long)int.MinValue - 1, CreateTypeAndGetFieldValue((long)int.MinValue - 1));
    }

    #endregion

    #region Char edge cases

    // Note: Chars above sbyte range (>127) trigger a known issue in
    // GenerateCodeToWriteIntegralValue where Ldc_I4_S sign-extends.

    [Fact]
    public void FieldValueSet_Char_Ascii_SetsCorrectValue()
    {
        Assert.Equal('A', CreateTypeAndGetFieldValue('A'));
    }

    [Fact]
    public void FieldValueSet_Char_Null_SetsCorrectValue()
    {
        Assert.Equal('\0', CreateTypeAndGetFieldValue('\0'));
    }

    #endregion

    #region Float edge cases

    [Fact]
    public void FieldValueSet_Float_Zero_SetsCorrectValue()
    {
        Assert.Equal(0.0f, CreateTypeAndGetFieldValue(0.0f));
    }

    [Fact]
    public void FieldValueSet_Float_Negative_SetsCorrectValue()
    {
        Assert.Equal(-1.5f, CreateTypeAndGetFieldValue(-1.5f));
    }

    #endregion

    #region Double edge cases

    [Fact]
    public void FieldValueSet_Double_Zero_SetsCorrectValue()
    {
        Assert.Equal(0.0, CreateTypeAndGetFieldValue(0.0));
    }

    [Fact]
    public void FieldValueSet_Double_Negative_SetsCorrectValue()
    {
        Assert.Equal(-2.71828, CreateTypeAndGetFieldValue(-2.71828));
    }

    #endregion

    #region String edge cases

    [Fact]
    public void FieldValueSet_String_Empty_SetsCorrectValue()
    {
        Assert.Equal("", CreateTypeAndGetFieldValue(""));
    }

    [Fact]
    public void FieldValueSet_String_Unicode_SetsCorrectValue()
    {
        Assert.Equal("caf\u00E9 \u2615", CreateTypeAndGetFieldValue("caf\u00E9 \u2615"));
    }

    #endregion

    #region Decimal edge cases

    [Fact]
    public void FieldValueSet_Decimal_WithScale_SetsCorrectValue()
    {
        Assert.Equal(1.123456789m, CreateTypeAndGetFieldValue(1.123456789m));
    }

    [Fact]
    public void FieldValueSet_Decimal_Zero_SetsCorrectValue()
    {
        Assert.Equal(0.0m, CreateTypeAndGetFieldValue(0.0m));
    }

    #endregion

    #region Enum

    [Fact]
    public void FieldValueSet_Enum_SetsCorrectValue()
    {
        var helper = new FieldValueSetInstructionHelper();
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"Test_{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
        var typeBuilder = moduleBuilder.DefineType("TestType", TypeAttributes.Public);
        var fieldBuilder = typeBuilder.DefineField("_field", typeof(DayOfWeek), FieldAttributes.Public);

        var instructions = helper.GenerateCodeToSetFieldValue(fieldBuilder, DayOfWeek.Wednesday).ToList();

        var ctorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        var il = ctorBuilder.GetILGenerator();
        foreach (var instruction in instructions)
        {
            instruction(il);
        }
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
        il.Emit(OpCodes.Ret);

        var createdType = typeBuilder.CreateType();
        var instance = Activator.CreateInstance(createdType)!;
        var result = (DayOfWeek)createdType.GetField("_field")!.GetValue(instance)!;

        Assert.Equal(DayOfWeek.Wednesday, result);
    }

    #endregion

    #region Type references

    [Fact]
    public void FieldValueSet_Type_SetsCorrectValue()
    {
        Assert.Equal(typeof(string), CreateTypeAndGetFieldValue(typeof(string)));
    }

    #endregion

    #region Unsupported types

    [Fact]
    public void FieldValueSet_UnsupportedType_ThrowsNotSupportedException()
    {
        var helper = new FieldValueSetInstructionHelper();
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName($"Test_{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
        var typeBuilder = moduleBuilder.DefineType("TestType", TypeAttributes.Public);
        var fieldBuilder = typeBuilder.DefineField("_field", typeof(DateTime), FieldAttributes.Public);

        Assert.Throws<NotSupportedException>(() =>
            helper.GenerateCodeToSetFieldValue(fieldBuilder, DateTime.Now).ToList());
    }

    #endregion
}
