namespace AccessFilter
{
    public static class IsNotEmptyExtensions
    {
        public static bool IsNotEmpty(this string str) => !IsEmpty(str);

        public static bool IsEmpty(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool NotContains(this string str, string val) => !str.Contains(val);

        public static bool EqualsIgnoreCase(this string str, string val) => string.Equals(str, val, StringComparison.InvariantCultureIgnoreCase);
    }
}
