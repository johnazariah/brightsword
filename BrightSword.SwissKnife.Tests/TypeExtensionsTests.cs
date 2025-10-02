using System.Collections.Generic;
using System.Linq;
using BrightSword.SwissKnife;
using Xunit;

namespace BrightSword.SwissKnife.Tests
{
    public interface I1 { int A { get; } }
    public interface I2 : I1 { int B { get; } }

    public class TypeExtensionsTests
    {
        [Fact]
        public void NameForGenericTypeWorks()
        {
            var name = typeof(List<string>).Name();
            Assert.Equal("List<String>", name);
        }

        [Fact]
        public void GetAllPropertiesInterfaceInheritanceWorks()
        {
            var props = typeof(I2).GetAllProperties().Select(p => p.Name).ToArray();
            Assert.Contains("A", props);
            Assert.Contains("B", props);
        }
    }
}
