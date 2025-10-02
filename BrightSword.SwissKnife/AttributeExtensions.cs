using System;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife
{
    public static class AttributeExtensions
    {
#pragma warning disable RCS1163 // Unused parameter
        public static TAttribute GetCustomAttribute<TAttribute>(this Type @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
            => (TAttribute)@this.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
#pragma warning restore RCS1163

#pragma warning disable RCS1163 // Unused parameter
        public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo @this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
            => (TAttribute)@this.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
#pragma warning restore RCS1163

        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this Type @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
            return attr == null ? defaultValue : selector((TAttribute)attr);
        }

        public static TResult GetCustomAttributeValue<TAttribute, TResult>(this MemberInfo @this, Func<TAttribute, TResult> selector, TResult defaultValue = default, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where TAttribute : Attribute
        {
            var attr = @this.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
            return attr == null ? defaultValue : selector((TAttribute)attr);
        }
    }
}
