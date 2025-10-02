using System;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class ObjectDescriberTests
    {
    private sealed class C { public int Prop { get; set; } public static void Method() { } }

        [Fact]
        public void GetName_MemberExpression_Works()
        {
            var name = ObjectDescriber.GetName<C, int>(c => c.Prop);
            Assert.Equal("Prop", name);
        }

        [Fact]
        public void GetName_MethodCall_Works()
        {
            var name = ObjectDescriber.GetName(() => C.Method());
            Assert.Equal("Method", name);
        }
    }
}
