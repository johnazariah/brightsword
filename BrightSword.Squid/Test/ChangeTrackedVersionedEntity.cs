using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class ChangeTrackedVersionedEntity : PropertyChangingNotificationSink, ISampleInterface
    {
        private string _hiddenProperty;

        public ChangeTrackedVersionedEntity()
        {
            BaseAmounts = [];
            BlobsByName = new Dictionary<ISampleInterface, byte[]>();
            Ids = [];
        }

        public int IntegerValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(IntegerValue), typeof(int), field, value))
                {
                    field = value;
                }
            }
        }

        public long LongValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(LongValue), typeof(long), field, value))
                {
                    field = value;
                }
            }
        }

        public short ShortValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(ShortValue), typeof(short), field, value))
                {
                    field = value;
                }
            }
        }

        public byte ByteValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(ByteValue), typeof(byte), field, value))
                {
                    field = value;
                }
            }
        }

        public bool BoolValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(BoolValue), typeof(bool), field, value))
                {
                    field = value;
                }
            }
        }

        public DateTime DateTimeValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(DateTimeValue), typeof(DateTime), field, value))
                {
                    field = value;
                }
            }
        }

        public string StringValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(StringValue), typeof(string), field, value))
                {
                    field = value;
                }
            }
        }

        public IList<decimal> BaseAmounts { get; }
        public IDictionary<ISampleInterface, byte[]> BlobsByName { get; }
        public IList<int> Ids { get; }

        public string ReadonlyName => throw new NotSupportedException();

        public decimal MutableValue
        {
            get;
            set
            {
                if (OnPropertyChanging(nameof(MutableValue), typeof(decimal), field, value))
                {
                    field = value;
                }
            }
        }

        string IBaseInterface.HiddenProperty
        {
            get => _hiddenProperty;
            set
            {
                if (OnPropertyChanging(nameof(IBaseInterface.HiddenProperty), typeof(string), _hiddenProperty, value))
                {
                    _hiddenProperty = value;
                }
            }
        }

        decimal ISampleInterface.HiddenProperty => throw new NotImplementedException();

        public void VoidMethodNoArgs() => throw new NotImplementedException();
        public int IntMethodWithArgs(int a, int b) => throw new NotImplementedException();
        public int IntMethodWithArgs(int a, int b, int c) => throw new NotImplementedException();
        public char ParamsMethod(int a, int b, params string[] foo) => throw new NotImplementedException();
        public Guid MethodWithRefParameters(ref string a) => throw new NotImplementedException();
        public char[] ArrayMethodWithOutParameters(out string a) => throw new NotImplementedException();

        public event EventHandler Event;
    }
}
