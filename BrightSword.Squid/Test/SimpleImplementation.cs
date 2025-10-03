using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class SimpleImplementation : ISampleInterface
    {
        private readonly IList<decimal> _baseAmounts;
        private readonly IDictionary<ISampleInterface, byte[]> _blobsByName;
        private readonly IList<int> _ids;

        public SimpleImplementation()
        {
            _baseAmounts = new List<decimal>();
            _blobsByName = new Dictionary<ISampleInterface, byte[]>();
            _ids = new List<int>();
        }

        public IDictionary<ISampleInterface, byte[]> BlobsByName
        {
            get { return _blobsByName; }
        }

        public string ReadonlyName
        {
            get { throw new NotSupportedException(); }
        }

        public IList<int> Ids
        {
            get { return _ids; }
        }

        public decimal MutableValue { get; set; }

        public void VoidMethodNoArgs()
        {
            throw new NotImplementedException();
        }

        public int IntMethodWithArgs(int a,
                                     int b)
        {
            throw new NotImplementedException();
        }

        public int IntMethodWithArgs(int a,
                                     int b,
                                     int c)
        {
            throw new NotImplementedException();
        }

        public char ParamsMethod(int a,
                                 int b,
                                 params string[] foo)
        {
            throw new NotImplementedException();
        }

        public Guid MethodWithRefParameters(ref string a)
        {
            throw new NotImplementedException();
        }

        public char[] ArrayMethodWithOutParameters(out string a)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Event;

        public IList<decimal> BaseAmounts
        {
            get { return _baseAmounts; }
        }

        string IBaseInterface.HiddenProperty { get; set; }

        decimal ISampleInterface.HiddenProperty
        {
            get { throw new NotImplementedException(); }
        }
    }
}
