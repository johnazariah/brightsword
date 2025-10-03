using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class ChangeTrackedVersionedEntity : PropertyChangingNotificationSink,
                                                ISampleInterface
    {
        private readonly IList<decimal> _baseAmounts;
        private readonly IDictionary<ISampleInterface, byte[]> _blobsByName;
        private readonly IList<int> _ids;
        private bool _boolValue;
        private byte _byteValue;
        private DateTime _dateTimeValue;
        private string _hiddenProperty;
        private int _integerValue;
        private long _longValue;
        private decimal _mutableValue;
        private short _shortValue;
        private string _stringValue;

        public ChangeTrackedVersionedEntity()
        {
            _baseAmounts = new List<decimal>();
            _blobsByName = new Dictionary<ISampleInterface, byte[]>();
            _ids = new List<int>();
        }

        public int IntegerValue
        {
            get { return _integerValue; }
            set
            {
                if (OnPropertyChanging(nameof(IntegerValue),
                                               typeof (int),
                                               _integerValue,
                                               value))
                {
                    _integerValue = value;
                }
            }
        }

        public long LongValue
        {
            get { return _longValue; }
            set
            {
                if (OnPropertyChanging(nameof(LongValue),
                                               typeof (int),
                                               _longValue,
                                               value))
                {
                    _longValue = value;
                }
            }
        }

        public short ShortValue
        {
            get { return _shortValue; }
            set
            {
                if (OnPropertyChanging(nameof(ShortValue),
                                               typeof (int),
                                               _shortValue,
                                               value))
                {
                    _shortValue = value;
                }
            }
        }

        public byte ByteValue
        {
            get { return _byteValue; }
            set
            {
                if (OnPropertyChanging(nameof(ByteValue),
                                               typeof (int),
                                               _byteValue,
                                               value))
                {
                    _byteValue = value;
                }
            }
        }

        public bool BoolValue
        {
            get { return _boolValue; }
            set
            {
                if (OnPropertyChanging(nameof(BoolValue),
                                               typeof (int),
                                               _boolValue,
                                               value))
                {
                    _boolValue = value;
                }
            }
        }

        public DateTime DateTimeValue
        {
            get { return _dateTimeValue; }
            set
            {
                if (OnPropertyChanging(nameof(DateTimeValue),
                                               typeof (int),
                                               _dateTimeValue,
                                               value))
                {
                    _dateTimeValue = value;
                }
            }
        }

        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                if (OnPropertyChanging(nameof(StringValue),
                                               typeof (int),
                                               _stringValue,
                                               value))
                {
                    _stringValue = value;
                }
            }
        }

        public IList<decimal> BaseAmounts
        {
            get { return _baseAmounts; }
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

        public decimal MutableValue
        {
            get { return _mutableValue; }
            set
            {
                if (OnPropertyChanging(nameof(MutableValue),
                                               typeof (decimal),
                                               _mutableValue,
                                               value))
                {
                    _mutableValue = value;
                }
                _mutableValue = value;
            }
        }

        string IBaseInterface.HiddenProperty
        {
            get { return _hiddenProperty; }
            set
            {
                _hiddenProperty = value;
                if (OnPropertyChanging(nameof(IBaseInterface.HiddenProperty),
                                               typeof (string),
                                               _mutableValue,
                                               value))
                {
                    _hiddenProperty = value;
                }
                _hiddenProperty = value;
            }
        }

        decimal ISampleInterface.HiddenProperty
        {
            get { throw new NotImplementedException(); }
        }

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
    }
}