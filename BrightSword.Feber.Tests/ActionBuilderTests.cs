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

        private static readonly System.Collections.Generic.List<int> _record = new();

        private sealed class RecordingActionBuilder : ActionBuilder<Proto, Proto>
        {
            protected override System.Linq.Expressions.Expression PropertyExpression(System.Reflection.PropertyInfo property, System.Linq.Expressions.ParameterExpression instanceParameterExpression)
            {
                // build an expression that adds the property value to a static list via a helper method
                var addMethod = typeof(ActionBuilderTests).GetMethod(nameof(Record), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
                var propExpr = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Property(instanceParameterExpression, property), typeof(int));
                return System.Linq.Expressions.Expression.Call(addMethod, propExpr);
            }
        }

        private static void Record(int v) => _record.Add(v);

        [Property]
        public static void ActionRecordsPropertyValues(int x, int y)
        {
            _record.Clear();
            var b = new RecordingActionBuilder();
            var a = b.Action;
            var p = new Proto { X = x, Y = y };
            a(p);
            // two properties expected
            Assert.Equal(2, _record.Count);
            Assert.Contains(x, _record);
            Assert.Contains(y, _record);
        }
    }
}
