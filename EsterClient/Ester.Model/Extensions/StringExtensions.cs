using System;

namespace Ester.Model.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool ContainsAny(this string source, string[] toCheck, StringComparison comp)
        {
            foreach (var s in toCheck)
            {
                if (source.IndexOf(s, comp) >= 0)
                    return true;
            }
            return false;
        }

        public static bool ContainsAll(this string source, string[] toCheck, StringComparison comp)
        {
            foreach (var s in toCheck)
            {
                if (source.IndexOf(s, comp) < 0)
                    return false;
            }
            return true;
        }
    }
}
