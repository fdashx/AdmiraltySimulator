using System.Globalization;

namespace AdmiraltySimulator
{
    public static class ParseUtil
    {
        public static bool TryInt(string s, out int result)
        {
            return int.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool TryDouble(string s, out double result)
        {
            return double.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static int Int(string s, int valueIfNullOrEmpty = 0)
        {
            var sTrim = s?.Trim();
            return string.IsNullOrEmpty(sTrim)
                ? valueIfNullOrEmpty
                : int.Parse(sTrim, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}