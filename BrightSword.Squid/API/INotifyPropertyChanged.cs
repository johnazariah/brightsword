using System;

namespace BrightSword.Squid.API
{
    /// <summary>
    /// Defines a notification contract for types that want to be informed after a property has changed.
    /// This interface is used by the Squid emitted types to notify interested base types about property changes.
    /// </summary>
    public interface INotifyPropertyChanged
    {
        /// <summary>
        /// Called when a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="propertyType">The runtime type of the property.</param>
        /// <param name="currentValue">The previous value of the property.</param>
        /// <param name="newValue">The new value being assigned to the property.</param>
        void OnPropertyChanged(string propertyName, Type propertyType, object currentValue, object newValue);
    }
}
