#nullable disable
namespace BrightSword.Feber.Samples;

public static class PrettyPrinter
{
  public static void Print<T>(this T _this) => PrettyPrinter<T>.Print(_this);
}
