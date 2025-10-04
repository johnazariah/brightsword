namespace BrightSword.SwissKnife.Tests
{
    public interface I1 { int A { get; } }
    public interface I2 : I1 { int B { get; } }

    public class TypeExtensionsTests
    {
        [Fact]
        public void NameForGenericTypeWorks()
        {
            var name = typeof(List<string>).PrintableName();
            Assert.Equal("List<String>", name);
        }

        [Fact]
        public void GetAllPropertiesInterfaceInheritanceWorks()
        {
            var props = typeof(I2).GetAllProperties().Select(p => p.Name).ToArray();
            Assert.Contains("A", props);
            Assert.Contains("B", props);
        }

        private static readonly Type[] SampleTypes = [typeof(int), typeof(string), typeof(List<string>), typeof(object)];

        [Property]
        public static void NameIdempotent(int idx)
        {
            var t = SampleTypes[Math.Abs(idx) % SampleTypes.Length];
            var name = t.PrintableName();
            Assert.NotNull(name);
        }
    }
}
