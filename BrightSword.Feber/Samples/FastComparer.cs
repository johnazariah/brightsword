#nullable disable
namespace BrightSword.Feber.Samples;

public static class FastComparer
{
  public static bool AllPropertiesAreEqualWith<T>(this T _this, T other)
  {
    return FastComparer<T>.AllPropertiesAreEqual(_this, other);
  }
}
