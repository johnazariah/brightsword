using System.Reflection;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public class AttributeExtensionsTests
    {
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    private sealed class SampleAttr : System.Attribute
    {
        public string Name { get; }
        public SampleAttr(string name) { Name = name; }
    }

        [SampleAttr("Cls")]
        private sealed class C
        {
            [SampleAttr("Mth")]
            #pragma warning disable CA1822 // Method does not access instance data - kept instance for reflection-based attribute lookup
            public void M() { }
            #pragma warning restore CA1822
        }

        [Fact]
        public void GetCustomAttributeOnType_Works()
        {
            var attr = typeof(C).GetCustomAttribute<SampleAttr>();
            Assert.NotNull(attr);
            Assert.Equal("Cls", attr.Name);
        }

        [Fact]
        public void GetCustomAttributeOnMember_Works()
        {
            var mi = typeof(C).GetMethod("M", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var attr = mi.GetCustomAttribute<SampleAttr>();
            Assert.NotNull(attr);
            Assert.Equal("Mth", attr.Name);
        }

        [Fact]
        public void GetCustomAttributeValue_Works()
        {
            var name = typeof(C).GetCustomAttributeValue<SampleAttr, string>(a => a.Name, "def");
            Assert.Equal("Cls", name);
        }
    }
}
