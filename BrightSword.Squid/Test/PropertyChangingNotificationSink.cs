using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using BrightSword.Squid.API;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class PropertyChangingNotificationSink : INotifyPropertyChanging
    {
        private readonly IDictionary<string, object> _changedProperties;

        public PropertyChangingNotificationSink()
        {
            IsNew = true;

            _changedProperties = new Dictionary<string, object>();
        }

        public bool IsPropertyChanged(string propertyName)
        {
            return _changedProperties.ContainsKey(propertyName);
        }

        public object GetOriginalValue(string propertyName)
        {
            object result;
            return _changedProperties.TryGetValue(propertyName,
                                                  out result)
                       ? result
                       : null;
        }

        public bool IsNew { get; internal set; }

        public bool IsChanged
        {
            get { return _changedProperties.Any(); }
        }

        public bool OnPropertyChanging(string propertyName,
                                               Type propertyType,
                                               object currentValue,
                                               object newValue)
        {
            if (currentValue == newValue)
            {
                return false;
            }

            if (_changedProperties.ContainsKey(propertyName))
            {
                if (_changedProperties[propertyName] == newValue)
                {
                    _changedProperties.Remove(propertyName);
                }
            }
            else
            {
                _changedProperties[propertyName] = newValue;
            }

            return true;
        }
    }
}