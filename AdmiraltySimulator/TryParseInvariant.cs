using System.Globalization;

namespace AdmiraltySimulator
{
    public static class TryParseInvariant
    {
        public static bool Int(string s, out int result)
        {
            return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool Double(string s, out double result)
        {
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}