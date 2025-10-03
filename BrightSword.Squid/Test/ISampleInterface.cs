using System;
using System.Collections.Generic;

namespace BrightSword.Squid.Test
{
    public interface ISampleInterface : IBaseInterface
    {
        new decimal HiddenProperty { get; }

        string ReadonlyName { get; }

        IList<int> Ids { get; }

        IDictionary<ISampleInterface, byte[]> BlobsByName { get; }

        decimal MutableValue { get; set; }

        void VoidMethodNoArgs();

        int IntMethodWithArgs(int a,
                              int b);

        int IntMethodWithArgs(int a,
                              int b,
                              int c);

        char ParamsMethod(int a,
                          int b,
                          params string[] foo);

        Guid MethodWithRefParameters(ref string a);

        char[] ArrayMethodWithOutParameters(out string a);

        event EventHandler Event;
    }
}