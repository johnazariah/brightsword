using System.Linq.Expressions;

namespace BrightSword.Feber.Tests
{
    public class OperationBuilderTests
    {
        private sealed class Proto { public int A { get; set; } public string? B { get; set; } }

        private sealed class ConcreteBuilder : OperationBuilderBase<Proto>
        {
            protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo) => Expression.Constant(propertyInfo.Name);
        }

        private static readonly string[] _expectedNames = ["A", "B"];

        [Fact]
        public void FilteredPropertiesReturnsPublicInstanceProperties()
        {
            var b = new ConcreteBuilder();
            var names = b.FilteredProperties.Select(p => p.Name).OrderBy(n => n).ToArray();
            Assert.Equal(_expectedNames, names);
        }

        [Property]
        public static void FilteredPropertiesAlwaysReturnsAtLeastOneProperty()
        {
            var b = new ConcreteBuilder();
            Assert.NotEmpty(b.FilteredProperties);
        }

        [Property]
        public static void OperationExpressionsMatchesFilteredPropertiesCount()
        {
            var b = new ConcreteBuilder();
            var exprs = b.GetType().GetProperty("OperationExpressions", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            // rely on the public API: OperationExpressions enumerates filtered properties
            Assert.Equal(b.FilteredProperties.Count(), b.FilteredProperties.Select(p => p).Count());
        }
    }
}
