// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.AttributeExtensions
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
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
