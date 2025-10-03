using System;

namespace BrightSword.Squid.API
{
    /// <summary>
    /// Defines a notification contract for types that wish to be informed before a property value changes.
    /// Implementations return a boolean indicating whether the change should proceed.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// Called before a property value is changed. Implementations may veto the change by returning false.
        /// </summary>
        /// <param name="propertyName">The name of the property to be changed.</param>
        /// <param name="propertyType">The runtime type of the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="newValue">The new value that will be assigned if the change is allowed.</param>
        /// <returns><c>true</c> if the change may proceed; otherwise <c>false</c>.</returns>
        bool OnPropertyChanging(string propertyName, Type propertyType, object currentValue, object newValue);
    }
}
