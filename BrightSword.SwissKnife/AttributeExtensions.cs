using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BrightSword.SwissKnife;

public static class AttributeExtensions
{
  public static TAttribute GetCustomAttribute<TAttribute>(this Type _this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
  {
    return (TAttribute) ((IEnumerable<object>) _this.GetCustomAttributes(typeof (TAttribute), true)).FirstOrDefault<object>();
  }

  public static TAttribute GetCustomAttribute<TAttribute>(this MemberInfo _this, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TAttribute : Attribute
  {
    return (TAttribute) ((IEnumerable<object>) _this.GetCustomAttributes(typeof (TAttribute), true)).FirstOrDefault<object>();
  }

  public static TResult GetCustomAttributeValue<TAttribute, TResult>(
    this Type _this,
    Func<TAttribute, TResult> selector,
    TResult defaultValue = default(TResult),
    BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    where TAttribute : Attribute
  {
    return ((IEnumerable<object>) _this.GetCustomAttributes(typeof (TAttribute), true)).FirstOrDefault<object>().Maybe<object, TResult>((Func<object, TResult>) (_ => selector((TAttribute) _)), defaultValue);
  }

  public static TResult GetCustomAttributeValue<TAttribute, TResult>(
    this MemberInfo _this,
    Func<TAttribute, TResult> selector,
    TResult defaultValue = default(TResult),
    BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
    where TAttribute : Attribute
  {
    return ((IEnumerable<object>) _this.GetCustomAttributes(typeof (TAttribute), true)).FirstOrDefault<object>().Maybe<object, TResult>((Func<object, TResult>) (_ => selector((TAttribute) _)), defaultValue);
  }
}
