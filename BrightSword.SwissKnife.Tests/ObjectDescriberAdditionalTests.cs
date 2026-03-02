using System.Linq.Expressions;

namespace BrightSword.SwissKnife.Tests
{
    public class ObjectDescriberAdditionalTests
    {
        [Fact]
        public void GetName_UnsupportedExpressionType_ThrowsNotSupported()
        {
            var param = Expression.Constant(42);
            var lambda = Expression.Lambda<Func<int>>(param);
            Assert.Throws<NotSupportedException>(() => ObjectDescriber.GetName(lambda));
        }

        [Fact]
        public void GetName_ActionWithArg_MethodCall_ReturnsName()
        {
            var name = ObjectDescriber.GetName<string>(x => x.ToLower());
            Assert.Equal("ToLower", name);
        }

        [Fact]
        public void GetName_FuncWithArg_MemberExpression_ReturnsName()
        {
            var name = ObjectDescriber.GetName<List<int>, int>(x => x.Count);
            Assert.Equal("Count", name);
        }

        [Fact]
        public void GetName_MemberExpression_ReturnsPropertyName()
        {
            var name = ObjectDescriber.GetName<string, int>(x => x.Length);
            Assert.Equal("Length", name);
        }

        [Fact]
        public void GetName_MethodCallExpression_ReturnsMethodName()
        {
            var name = ObjectDescriber.GetName<string, string>(x => x.ToUpper());
            Assert.Equal("ToUpper", name);
        }

        [Fact]
        public void GetName_ActionExpression_ReturnsName()
        {
            var name = ObjectDescriber.GetName(() => Console.WriteLine());
            Assert.Equal("WriteLine", name);
        }

        [Fact]
        public void GetName_FuncExpression_ReturnsName()
        {
            var name = ObjectDescriber.GetName(() => DateTime.Now);
            Assert.Equal("Now", name);
        }

        [Fact]
        public void GetName_ActionArg_ReturnsName()
        {
            var name = ObjectDescriber.GetName<List<int>>(x => x.Clear());
            Assert.Equal("Clear", name);
        }
    }
}
