using System.Linq.Expressions;

namespace BrightSword.Feber.Tests
{
    public class FunctionBuilderTests
    {
        private sealed class Proto { public int X { get; set; } public int Y { get; set; } }

        private sealed class SumBuilder : FunctionBuilder<Proto, Proto, int>
        {
            protected override int Seed => 0;
            protected override Func<Expression, Expression, Expression> Conjunction => (l, r) => Expression.Add(l, r);
            protected override Expression PropertyExpression(System.Reflection.PropertyInfo property, ParameterExpression instanceParameterExpression) =>
                Expression.Convert(Expression.Property(instanceParameterExpression, property), typeof(int));
        }

        [Fact]
        public void FunctionBuildsSum()
        {
            var b = new SumBuilder();
            var f = b.Function;
            var p = new Proto { X = 3, Y = 4 };
            var result = f(p);
            Assert.Equal(7, result);
        }

        [Property]
        public static void FunctionSumIsCommutative(int x, int y)
        {
            var b = new SumBuilder();
            var f = b.Function;
            var p1 = new Proto { X = x, Y = y };
            var p2 = new Proto { X = y, Y = x };
            Assert.Equal(f(p1), f(p2));
        }
    }
}
