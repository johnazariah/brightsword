namespace BrightSword.Squid.Test
{
#pragma warning disable CA1716 // test interface uses Event member name intentionally
    public interface ISampleInterface : IBaseInterface
    {
        new decimal HiddenProperty { get; }

        string ReadonlyName { get; }

        IList<int> Ids { get; }

        IDictionary<ISampleInterface, byte[]> BlobsByName { get; }

        decimal MutableValue { get; set; }

        void VoidMethodNoArgs();

        int IntMethodWithArgs(int a, int b);

        int IntMethodWithArgs(int a, int b, int c);

        char ParamsMethod(int a, int b, params string[] foo);

        Guid MethodWithRefParameters(ref string a);

        char[] ArrayMethodWithOutParameters(out string a);

        event EventHandler Event;
    }
#pragma warning restore CA1716
}
