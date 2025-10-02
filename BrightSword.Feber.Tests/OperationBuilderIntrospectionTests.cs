using System.Reflection;
using Xunit;

namespace BrightSword.Feber.Tests
{
    public class OperationBuilderIntrospectionTests
    {
        private sealed class Proto { public int X { get; set; } public int Y { get; set; } }

        private sealed class IntrospectionBuilder : BrightSword.Feber.Core.OperationBuilderBase<Proto>
        {
            protected override System.Linq.Expressions.Expression BuildPropertyExpression(PropertyInfo propertyInfo) => System.Linq.Expressions.Expression.Constant(propertyInfo.Name);
        }

        [Fact]
        public void FilteredPropertiesReturnsPublicInstanceProperties()
        {
            var b = new IntrospectionBuilder();
            var props = b.FilteredProperties;
            Assert.Contains(props, p => p.Name == "X");
            Assert.Contains(props, p => p.Name == "Y");
        }
    }
}
