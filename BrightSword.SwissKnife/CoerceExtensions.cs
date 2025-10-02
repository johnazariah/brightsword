// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.CoerceExtensions
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System;

#nullable disable
namespace BrightSword.SwissKnife;

public static class CoerceExtensions
{
  public static object CoerceType(this object value, Type targetType, object defaultValue)
  {
    if (value is string && targetType == typeof (string))
      return value;
    if (targetType == typeof (bool))
    {
      string str = value.ToString();
      if (string.IsNullOrEmpty(str))
        return (object) false;
      if (str.Equals("y", StringComparison.OrdinalIgnoreCase))
        return (object) true;
      if (str.Equals("n", StringComparison.OrdinalIgnoreCase))
        return (object) false;
      throw new ArgumentException("Boolean should be either 'y' or 'n'");
    }
    if (targetType.IsEnum)
    {
      object returnValue;
      if (value.CoerceType(targetType, out returnValue, (Func<Type, bool>) (_ => true), (Func<Type, object, object>) ((_type, _value) => Enum.Parse(_type, _value.ToString(), true)), defaultValue))
        return returnValue;
      throw new InvalidCastException($"Cannot cast {value} to {targetType}");
    }
    object returnValue1;
    if (value.CoerceType<bool>(targetType, out returnValue1, (Func<Type, object, bool>) ((_, _value) => bool.Parse(_value.ToString())), defaultValue) || value.CoerceType<Decimal>(targetType, out returnValue1, (Func<Type, object, Decimal>) ((_, _value) => Decimal.Parse(_value.ToString())), defaultValue) || value.CoerceType<long>(targetType, out returnValue1, (Func<Type, object, long>) ((_, _value) => long.Parse(_value.ToString())), defaultValue) || value.CoerceType<int>(targetType, out returnValue1, (Func<Type, object, int>) ((_, _value) => int.Parse(_value.ToString())), defaultValue) || value.CoerceType<short>(targetType, out returnValue1, (Func<Type, object, short>) ((_, _value) => short.Parse(_value.ToString())), defaultValue) || value.CoerceType<byte>(targetType, out returnValue1, (Func<Type, object, byte>) ((_, _value) => byte.Parse(_value.ToString())), defaultValue) || value.CoerceType<char>(targetType, out returnValue1, (Func<Type, object, char>) ((_, _value) => char.Parse(_value.ToString())), defaultValue) || value.CoerceType<double>(targetType, out returnValue1, (Func<Type, object, double>) ((_, _value) => double.Parse(_value.ToString())), defaultValue) || value.CoerceType<float>(targetType, out returnValue1, (Func<Type, object, float>) ((_, _value) => float.Parse(_value.ToString())), defaultValue))
      return returnValue1;
    if (value.CoerceType<DateTime>(targetType, out returnValue1, (Func<Type, object, DateTime>) ((_, _value) => DateTime.Parse(_value.ToString())), defaultValue))
      return returnValue1;
    try
    {
      return Convert.ChangeType(value, targetType);
    }
    catch (Exception ex)
    {
      return value;
    }
  }

  public static bool CoerceType<T>(
    this object value,
    Type targetType,
    out object returnValue,
    Func<Type, object, T> parseFunc,
    object defaultValue)
  {
    return value.CoerceType(targetType, out returnValue, (Func<Type, bool>) (_ => _.IsAssignableFrom(typeof (T))), (Func<Type, object, object>) ((_type, _value) => (object) parseFunc(_type, _value)), defaultValue);
  }

  public static bool CoerceType(
    this object value,
    Type targetType,
    out object returnValue,
    Func<Type, bool> checkFunc,
    Func<Type, object, object> parseFunc,
    object defaultValue)
  {
    try
    {
      if (checkFunc(targetType))
      {
        returnValue = parseFunc(targetType, value);
        return true;
      }
    }
    catch (Exception ex)
    {
      if (defaultValue != null && defaultValue.GetType().IsAssignableFrom(targetType))
      {
        returnValue = defaultValue;
        return true;
      }
      returnValue = targetType.IsValueType ? Activator.CreateInstance(targetType) : (object) null;
      return true;
    }
    returnValue = (object) null;
    return false;
  }
}
