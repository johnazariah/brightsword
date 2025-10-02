// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Samples.CloneFactory
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

#nullable disable
namespace BrightSword.Feber.Samples;

public static class CloneFactory
{
  public static TSource Clone<TSource>(this TSource source) where TSource : new()
  {
    return CloneFactory<TSource, TSource, TSource>.Clone(source);
  }

  public static TDestination Clone<TSource, TDestination>(this TSource source) where TDestination : new()
  {
    return CloneFactory<TSource, TSource, TDestination>.Clone(source);
  }

  public static TDestination Clone<TSource, TDestination, TBase>(this TSource source) where TDestination : new()
  {
    return CloneFactory<TBase, TSource, TDestination>.Clone(source);
  }
}
