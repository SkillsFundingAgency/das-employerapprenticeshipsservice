using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.Extensions
{
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
    }
}