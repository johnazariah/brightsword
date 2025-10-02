using System.Linq.Expressions;

namespace BrightSword.Feber.Tests
{
    public class ActionBuilderTests
    {
        private sealed class Proto { public int X { get; set; } public int Y { get; set; } }

        private sealed class SumActionBuilder : ActionBuilder<Proto, Proto>
        {
            protected override Expression PropertyExpression(System.Reflection.PropertyInfo property, ParameterExpression instanceParameterExpression) =>
                Expression.Call(typeof(Console).GetMethod("WriteLine", [typeof(int)])!, Expression.Convert(Expression.Property(instanceParameterExpression, property), typeof(int)));
        }

        [Fact]
        public void ActionBuilderProducesAction()
        {
            var b = new SumActionBuilder();
            var a = b.Action;
            Assert.NotNull(a);
        }

        [Property]
        public static void ActionBuilderActionIsNotNull(int x, int y)
        {
            var b = new SumActionBuilder();
            var a = b.Action;
            Assert.NotNull(a);
        }
    }
}
