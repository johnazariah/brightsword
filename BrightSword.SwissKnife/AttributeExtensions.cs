using System;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    public static class AttributeExtensions
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this Type @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
        {
            _ = flags;
            return @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
        {
            _ = flags;
            return @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
        }

        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this Type @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            _ = flags;
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
            return attr == null ? defaultValue : selector(attr);
        }

        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            _ = flags;
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
            return attr == null ? defaultValue : selector(attr);
        }
    }
}
