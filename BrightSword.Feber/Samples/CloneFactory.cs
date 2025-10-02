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
