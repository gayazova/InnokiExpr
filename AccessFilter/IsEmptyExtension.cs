using System.Collections.Generic;
using System.Linq;

namespace AccessFilter
{
    public static class IsEmptyExtension
    {
        public static bool IsNotEmpty<T>(this IEnumerable<T> @enum)
        {
            if (@enum is string str)
                return !string.IsNullOrWhiteSpace(str);

            return @enum != null && @enum.Any();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> @enum) => !IsNotEmpty(@enum);
    }
}
