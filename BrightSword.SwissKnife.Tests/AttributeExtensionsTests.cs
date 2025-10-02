namespace BrightSword.SwissKnife.Tests
{
    public class AttributeExtensionsTests
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        private sealed class SampleAttr(string name) : Attribute
        {
            public string Name { get; } = name;
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
        public void GetCustomAttributeOnTypeWorks()
        {
            var attr = typeof(C).GetCustomAttribute<SampleAttr>();
            Assert.NotNull(attr);
            Assert.Equal("Cls", attr.Name);
        }

        [Fact]
        public void GetCustomAttributeOnMemberWorks()
        {
            var mi = typeof(C).GetMethod("M", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var attr = mi.GetCustomAttribute<SampleAttr>();
            Assert.NotNull(attr);
            Assert.Equal("Mth", attr.Name);
        }

        [Fact]
        public void GetCustomAttributeValueWorks()
        {
            var name = typeof(C).GetCustomAttributeValue<SampleAttr, string>(a => a.Name, "def");
            Assert.Equal("Cls", name);
        }
    }
}
