using System.Diagnostics.CodeAnalysis;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class SimpleImplementation : ISampleInterface
    {
        public SimpleImplementation()
        {
            BaseAmounts = [];
            BlobsByName = new Dictionary<ISampleInterface, byte[]>();
            Ids = [];
        }

        public IDictionary<ISampleInterface, byte[]> BlobsByName { get; }

        public string ReadonlyName => throw new NotSupportedException();

        public IList<int> Ids { get; }

        public decimal MutableValue { get; set; }

        public void VoidMethodNoArgs() => throw new NotImplementedException();

        public int IntMethodWithArgs(int a, int b) => throw new NotImplementedException();

        public int IntMethodWithArgs(int a, int b, int c) => throw new NotImplementedException();

        public char ParamsMethod(int a, int b, params string[] foo) => throw new NotImplementedException();

        public Guid MethodWithRefParameters(ref string a) => throw new NotImplementedException();

        public char[] ArrayMethodWithOutParameters(out string a) => throw new NotImplementedException();

        public event EventHandler Event;

        public IList<decimal> BaseAmounts { get; }

        string IBaseInterface.HiddenProperty { get; set; }

        decimal ISampleInterface.HiddenProperty => throw new NotImplementedException();
    }
}
