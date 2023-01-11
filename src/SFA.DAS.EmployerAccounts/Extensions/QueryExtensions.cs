using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerAccounts.Extensions;

public static class QueryExtensions
{
    public static IEnumerable<T> SetItemValues<T>(this IEnumerable<T> items, params Action<T>[] setters)
    {
        foreach (var item in items)
        {
            foreach (var action in setters)
            {
                action(item);
            }

            yield return item;
        }
    }

    public static QueryFutureEnumerable<int> FutureCount<T>(this IQueryable<T> source)
    {
        return source.GroupBy(a => 1).Select(g => g.Count()).Future();
    }
}