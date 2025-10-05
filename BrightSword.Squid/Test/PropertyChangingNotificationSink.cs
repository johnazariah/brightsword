using System.Diagnostics.CodeAnalysis;

using BrightSword.Squid.API;

namespace BrightSword.Squid.Test
{
    [ExcludeFromCodeCoverage]
    public class PropertyChangingNotificationSink : INotifyPropertyChanging
    {
        private readonly Dictionary<string, object> _changedProperties;

        public PropertyChangingNotificationSink()
        {
            IsNew = true;

            _changedProperties = [];
        }

        public bool IsPropertyChanged(string propertyName) => _changedProperties.ContainsKey(propertyName);

        public object GetOriginalValue(string propertyName) => _changedProperties.TryGetValue(propertyName, out var result) ? result : null;

        public bool IsNew { get; internal set; }

        public bool IsChanged => _changedProperties.Count != 0;

        public bool OnPropertyChanging(string propertyName, Type propertyType, object currentValue, object newValue)
        {
            if (ReferenceEquals(currentValue, newValue) || Equals(currentValue, newValue))
            {
                return false;
            }

            if (_changedProperties.TryGetValue(propertyName, out var existing))
            {
                if (EqualityComparer<object>.Default.Equals(existing, newValue))
                {
                    _ = _changedProperties.Remove(propertyName);
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
