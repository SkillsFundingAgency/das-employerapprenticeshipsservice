using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return GetBatch(enumerator, size);
                }
            }
        }

        private static IEnumerable<T> GetBatch<T>(IEnumerator<T> source, int size)
        {
            yield return source.Current;

            for (var i = 1; i < size && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }

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
