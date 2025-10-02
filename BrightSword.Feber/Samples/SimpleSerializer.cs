// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.SimpleSerializer
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

#nullable disable
namespace BrightSword.Feber.Samples;

public static class SimpleSerializer
{
  public static string Serialize<T>(this T _this) => SimpleSerializer<T>.Serialize(_this);
}
