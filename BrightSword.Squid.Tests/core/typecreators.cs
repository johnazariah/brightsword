using System;

using BrightSword.Squid.Test;
using BrightSword.Squid.TypeCreators;

namespace Tests.BrightSword.Squid.core
{
    public class PropertyChangingNotificationSinkTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
        where T : class
    {
        public PropertyChangingNotificationSinkTypeCreator() { }

        public PropertyChangingNotificationSinkTypeCreator(params Func<Type, Type>[] typeMaps)
            : base(typeMaps) { }

        public override string ClassName
        {
            get => string.Format(System.Globalization.CultureInfo.InvariantCulture, "Types.{0}.ChangeTrackedAttributed",
                                   typeof(T).Name);
        }

        public override Type BaseType => typeof(PropertyChangingNotificationSink);
    }
}
