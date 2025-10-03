using System;

namespace BrightSword.Squid.API
{
    public interface INotifyPropertyChanging
    {
        bool OnPropertyChanging(string propertyName,
                                Type propertyType,
                                object currentValue,
                                object newValue);
    }
}