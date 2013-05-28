using System.Collections.Generic;

namespace Ester.Model.Extensions
{
    public static class ListExtensions
    {
        public static bool ContainsAll<T>(this List<T> source, List<T> toCheck)
        {
            if (toCheck.Count == 0 || source.Count == 0) return false;
            foreach (var s in toCheck)
            {
                if (!source.Contains(s))
                    return false;
            }
            return true;
        }
    }
}
