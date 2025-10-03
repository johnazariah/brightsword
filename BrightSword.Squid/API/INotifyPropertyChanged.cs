using System;

namespace BrightSword.Squid.API
{
    public interface INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName,
                               Type propertyType,
                               object currentValue,
                               object newValue);
    }
}
