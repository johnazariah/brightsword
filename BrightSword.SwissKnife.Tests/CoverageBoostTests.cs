namespace BrightSword.SwissKnife.Tests
{
    /// <summary>
    /// Targeted tests to boost branch coverage for uncovered paths in
    /// Functional, Validator, CoerceExtensions, TypeExtensions,
    /// AttributeExtensions, and StringExtensions.
    /// </summary>
    public class CoverageBoostTests
    {
        #region Functional — MemoizeFix 2-arg and 3-arg overloads

        [Fact]
        public void MemoizeFix_TwoArgs_ComputesAndMemoizes()
        {
            var callCount = 0;
            var add = Functional.MemoizeFix<int, int, int>(
                self => (a, b) => { callCount++; return a + b; });

            Assert.Equal(5, add(2, 3));
            Assert.Equal(5, add(2, 3)); // memoized — should not increment callCount again
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MemoizeFix_ThreeArgs_ComputesAndMemoizes()
        {
            var callCount = 0;
            var sum = Functional.MemoizeFix<int, int, int, int>(
                self => (a, b, c) => { callCount++; return a + b + c; });

            Assert.Equal(6, sum(1, 2, 3));
            Assert.Equal(6, sum(1, 2, 3)); // memoized
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void MemoizeFix_TwoArgs_DifferentInputsComputeSeparately()
        {
            var add = Functional.MemoizeFix<int, int, int>(
                self => (a, b) => a + b);

            Assert.Equal(3, add(1, 2));
            Assert.Equal(7, add(3, 4));
        }

        [Fact]
        public void MemoizeFix_ThreeArgs_DifferentInputsComputeSeparately()
        {
            var sum = Functional.MemoizeFix<int, int, int, int>(
                self => (a, b, c) => a + b + c);

            Assert.Equal(6, sum(1, 2, 3));
            Assert.Equal(15, sum(4, 5, 6));
        }

        #endregion

        #region Validator — passing paths and CreateExceptionInstance branches

        [Fact]
        public void Check_Bool_TrueCondition_DoesNotThrow()
        {
#pragma warning disable CS0618
            true.Check("should not throw");
#pragma warning restore CS0618
        }

        [Fact]
        public void Check_Bool_Generic_TrueCondition_DoesNotThrow()
        {
#pragma warning disable CS0618
            true.Check<InvalidOperationException>("should not throw");
#pragma warning restore CS0618
        }

        [Fact]
        public void Check_Func_TrueCondition_DoesNotThrow()
        {
#pragma warning disable CS0618
            new Func<bool>(() => true).Check("should not throw");
#pragma warning restore CS0618
        }

        [Fact]
        public void Check_Func_Generic_TrueCondition_DoesNotThrow()
        {
#pragma warning disable CS0618
            new Func<bool>(() => true).Check<InvalidOperationException>("should not throw");
#pragma warning restore CS0618
        }

        // Exception with no string constructor — triggers CreateExceptionInstance catch path
        private class NoStringCtorException : Exception
        {
            public NoStringCtorException() : base("default-no-string-ctor") { }
        }

        [Fact]
        public void Check_Generic_NoStringCtor_FallsBackToDefaultCtor()
        {
#pragma warning disable CS0618
            var ex = Assert.Throws<NoStringCtorException>(
                () => false.Check<NoStringCtorException>("custom message"));
#pragma warning restore CS0618
            Assert.Equal("default-no-string-ctor", ex.Message);
        }

        [Fact]
        public void Check_Generic_NullMessage_UsesDefaultCtor()
        {
#pragma warning disable CS0618
            var ex = Assert.Throws<InvalidOperationException>(
                () => false.Check<InvalidOperationException>(null));
#pragma warning restore CS0618
            // null message → skips Activator.CreateInstance branch, uses new TException()
            Assert.NotNull(ex);
        }

        [Fact]
        public void Check_Func_Generic_NoStringCtor_FallsBackToDefaultCtor()
        {
#pragma warning disable CS0618
            var ex = Assert.Throws<NoStringCtorException>(
                () => new Func<bool>(() => false).Check<NoStringCtorException>("msg"));
#pragma warning restore CS0618
            Assert.Equal("default-no-string-ctor", ex.Message);
        }

        [Fact]
        public void Check_Func_Generic_NullMessage_UsesDefaultCtor()
        {
#pragma warning disable CS0618
            var ex = Assert.Throws<InvalidOperationException>(
                () => new Func<bool>(() => false).Check<InvalidOperationException>(null));
#pragma warning restore CS0618
            Assert.NotNull(ex);
        }

        #endregion

        #region CoerceExtensions — catch/error paths and fallback branches

        [Fact]
        public void CoerceType_ConvertChangeTypeFails_ReturnsOriginalValue()
        {
            // Guid is not handled by any parser and Convert.ChangeType will throw
#pragma warning disable CS0618
            var result = "not-a-guid".CoerceType(typeof(Guid), null);
#pragma warning restore CS0618
            Assert.Equal("not-a-guid", result);
        }

        [Fact]
        public void CoerceType_ParseThrows_NullDefault_ValueType_ReturnsActivatorDefault()
        {
            // int parser matches (IsAssignableFrom) but int.Parse throws,
            // defaultValue is null so fallback → Activator.CreateInstance(typeof(int)) → 0
#pragma warning disable CS0618
            var result = "not_a_number".CoerceType(typeof(int), null);
#pragma warning restore CS0618
            Assert.Equal(0, result);
        }

        [Fact]
        public void CoerceType_ParseThrows_ReferenceType_ReturnsNull()
        {
            // checkFunc returns true, parseFunc throws, targetType is reference → null fallback
#pragma warning disable CS0618
            var success = "xyz".CoerceType(
                typeof(Uri),
                out var returnValue,
                (Type _) => true,
                (Type _, object v) => throw new FormatException("bad"),
                null);
#pragma warning restore CS0618
            Assert.True(success);
            Assert.Null(returnValue);
        }

        [Fact]
        public void CoerceType_ComplexObject_FallbackCatch_ReturnsValue()
        {
            // An object that no parser handles and Convert.ChangeType also fails
            var obj = new object();
#pragma warning disable CS0618
            var result = obj.CoerceType(typeof(Guid), null);
#pragma warning restore CS0618
            Assert.Same(obj, result);
        }

        #endregion

        #region TypeExtensions — Name() method

        [Fact]
        public void Name_NonGenericType_ReturnsPrintableName()
        {
            Assert.Equal("Int32", typeof(int).Name());
        }

        [Fact]
        public void Name_GenericType_ReturnsPrintableName()
        {
            Assert.Equal("List<String>", typeof(List<string>).Name());
        }

        #endregion

        #region AttributeExtensions — GetCustomAttributeValue on MemberInfo

        [AttributeUsage(AttributeTargets.Method)]
        private sealed class TestMethodAttr(string value) : Attribute
        {
            public string Value { get; } = value;
        }

        private sealed class Decorated
        {
            [TestMethodAttr("found")]
#pragma warning disable CA1822
            public void HasAttr() { }
            public void NoAttr() { }
#pragma warning restore CA1822
        }

        [Fact]
        public void GetCustomAttributeValue_MemberInfo_ReturnsValue_WhenPresent()
        {
            var mi = typeof(Decorated).GetMethod("HasAttr")!;
            var result = mi.GetCustomAttributeValue<TestMethodAttr, string>(a => a.Value, "default");
            Assert.Equal("found", result);
        }

        [Fact]
        public void GetCustomAttributeValue_MemberInfo_ReturnsDefault_WhenAbsent()
        {
            var mi = typeof(Decorated).GetMethod("NoAttr")!;
            var result = mi.GetCustomAttributeValue<TestMethodAttr, string>(a => a.Value, "default");
            Assert.Equal("default", result);
        }

        #endregion

        #region StringExtensions — whitespace-only and edge-case paths

        [Fact]
        public void SplitIntoSegments_WhitespaceOnly_ReturnsEmpty()
        {
            var result = "   ".SplitIntoSegments().ToArray();
            Assert.Empty(result);
        }

        [Fact]
        public void SplitCamelCase_NullString_ReturnsEmpty()
        {
            string input = null;
            var result = input.SplitCamelCase().ToArray();
            Assert.Empty(result);
        }

        [Fact]
        public void SplitCamelCaseAndUnderscore_EmptyString_ReturnsEmpty()
        {
            var result = string.Empty.SplitCamelCaseAndUnderscore().ToArray();
            Assert.Empty(result);
        }

        [Fact]
        public void SplitDotted_NullString_ReturnsEmpty()
        {
            string input = null;
            var result = input.SplitDotted().ToArray();
            Assert.Empty(result);
        }

        #endregion
    }
}
