using System.Linq.Expressions;

// See documentation: BrightSword.Feber/docs/FunctionBuilder.md
// The docs include a "NullChecker" example that mirrors the NullCheckBuilder used in this test.

namespace BrightSword.Feber.Tests
{
    public class NullCheckBuilderTests
    {
        public sealed class Proto { public string? A { get; set; } public object? B { get; set; } }

        private sealed class NullCheckBuilder<T> : FunctionBuilder<T, T, bool>
        {
            protected override Expression PropertyExpression(
                PropertyInfo propertyInfo,
                ParameterExpression instanceParameter) =>
                Expression.Equal(Expression.Property(instanceParameter, propertyInfo), Expression.Constant(null, propertyInfo.PropertyType));
            protected override bool Seed => false;
            protected override Func<Expression, Expression, Expression> Conjunction => Expression.OrElse;
        }

        private readonly Lazy<Func<Proto, bool>> nullChecker = new(() => new NullCheckBuilder<Proto>().Function);

        [Property]
        public void NullChecksDetectNulls(Proto instance)
        {
            var actual = nullChecker.Value(instance);
            var expected = instance.A is null || instance.B is null;

            Assert.Equal(expected, actual);
        }
    }

    // No helpers required; test inspects the raw expression array produced by the builder.
}
