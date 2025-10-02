#nullable disable
namespace BrightSword.Feber.Samples;

public static class SimpleSerializer
{
  public static string Serialize<T>(this T _this) => SimpleSerializer<T>.Serialize(_this);
}
