// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.FastMapper
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

#nullable disable
namespace BrightSword.Feber.Samples;

public static class FastMapper
{
  public static T MapStaticToStatic<T>(this T _this, T source)
  {
    FastMapper<T>.MapStaticToStatic(source, _this);
    return _this;
  }

  public static T MapDynamicToStatic<T>(this T _this, object source)
  {
    FastMapper<T>.MapDynamicToStatic(source, _this);
    return _this;
  }

  public static object MapStaticToDynamic<T>(this object _this, T source)
  {
    FastMapper<T>.MapStaticToDynamic(source, _this);
    return _this;
  }

  public static object MapDynamicToDynamic<T>(this object _this, object source)
  {
    FastMapper<T>.MapDynamicToDynamic(source, _this);
    return _this;
  }

  public static object MapToBackingFields<T>(this T _this, object source)
  {
    FastMapper<T>.MapToBackingFields(source, _this);
    return (object) _this;
  }
}
