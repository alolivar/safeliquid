
// Type: SafeLiquid.StringExtensions




namespace SafeLiquid
{
  internal static class StringExtensions
  {
    public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrEmpty(s) || s.Trim().Length == 0;
  }
}
