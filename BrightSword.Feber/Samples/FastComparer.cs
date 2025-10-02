// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.FastComparer
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

#nullable disable
namespace BrightSword.Feber.Samples;

public static class FastComparer
{
  public static bool AllPropertiesAreEqualWith<T>(this T _this, T other)
  {
    return FastComparer<T>.AllPropertiesAreEqual(_this, other);
  }
}
