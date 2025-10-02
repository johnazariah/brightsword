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

        [Property]
        public static void FunctionEqualsSeedPlusSum(int x, int y)
        {
            var b = new SumBuilder();
            var f = b.Function;
            var p = new Proto { X = x, Y = y };
            var expected = 0 + x + y;
            Assert.Equal(expected, f(p));
        }

        private sealed class ProtoPair { public int X { get; set; } public int Y { get; set; } }

    private sealed class BinarySumBuilder : FunctionBuilder<ProtoPair, ProtoPair, ProtoPair, int>
        {
            protected override int Seed => 0;
            protected override System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> Conjunction => (l, r) => Expression.Add(l, r);
            protected override System.Linq.Expressions.Expression PropertyExpression(System.Reflection.PropertyInfo property, System.Linq.Expressions.ParameterExpression leftParam, System.Linq.Expressions.ParameterExpression rightParam)
            {
                var leftExpr = Expression.Convert(Expression.Property(leftParam, property), typeof(int));
                var rightExpr = Expression.Convert(Expression.Property(rightParam, property), typeof(int));
                return Expression.Add(leftExpr, rightExpr);
            }
        }

        [Property]
        public static void BinaryFunctionSumsBothSides(int lx, int ly, int rx, int ry)
        {
            var b = new BinarySumBuilder();
            var f = b.Function;
            var left = new ProtoPair { X = lx, Y = ly };
            var right = new ProtoPair { X = rx, Y = ry };
            // expected: (left.X + right.X) + (left.Y + right.Y)
            var expected = (left.X + right.X) + (left.Y + right.Y);
            Assert.Equal(expected, f(left, right));
        }

        [Property]
        public static void FunctionSumNonNegative(NonNegativeInt nx, NonNegativeInt ny)
        {
            var x = nx.Get;
            var y = ny.Get;
            var b = new SumBuilder();
            var f = b.Function;
            var p = new Proto { X = x, Y = y };
            Assert.True(f(p) >= 0);
        }

        [Property]
        public static void BinaryFunctionCommutative(NonNegativeInt lx, NonNegativeInt ly, NonNegativeInt rx, NonNegativeInt ry)
        {
            var left = new ProtoPair { X = lx.Get, Y = ly.Get };
            var right = new ProtoPair { X = rx.Get, Y = ry.Get };
            var b = new BinarySumBuilder();
            var f = b.Function;
            Assert.Equal(f(left, right), f(right, left));
        }
    }
}
