using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Worker.Extensions
{
    public static class EnumerableExtensions
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

            for (var i = 1; i <= size && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }
    }
}