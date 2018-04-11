using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.Domain.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        public static bool HasAtLeast<T>(this IEnumerable<T> items, uint count, Predicate<T> predicate)
        {
            var foundItems = 0;

            foreach (var item in items)
            {
                if (predicate(item))
                    foundItems++;

                if (foundItems >= count)
                    return true;
            }

            return false;
        }
    }
}
