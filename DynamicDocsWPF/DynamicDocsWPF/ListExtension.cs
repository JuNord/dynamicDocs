using System.Collections.Generic;
using System.Linq;

namespace DynamicDocsWPF
{
    public static class ListExtension
    {
        public static int GetHash<T>(this IEnumerable<T> list)
        {
            return list.Aggregate(17, (current, item) => current * 23 + ((item != null) ? item.GetHashCode() : 0));
        }
    }
}