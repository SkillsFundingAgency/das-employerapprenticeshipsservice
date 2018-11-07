using System.Linq;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerAccounts.Extensions
{
    public static class QueryableExtensions
    {
        public static QueryFutureEnumerable<int> FutureCount<T>(this IQueryable<T> source)
        {
            return source.GroupBy(a => 1).Select(g => g.Count()).Future();
        }
    }
}