using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.LevyAnalyser
{
    [Flags]
    public enum NumberRangeOptions
    {
        None = 0,
        OrderAscending = 1,
        OrderDescending = 2,
        RemoveDuplicates = 4,
    }

    /// <summary>
    ///     Converts a number range (think "print pages in range" such as "1-4,7,9,12-16") into 
    ///     an enumerable of int.
    /// </summary>
    public static class NumberRange
    {
        private class LongPair
        {
            public long LowerBound { get; set; }
            public long UpperBound { get; set; }
            public bool IsValid { get; set; }
        }

        public static IEnumerable<long> ToLongs(string definition)
        {
            return ToLongs(definition, NumberRangeOptions.None);
        }

        public static IEnumerable<long> ToLongs(string definition, NumberRangeOptions options)
        {
            var result = definition
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ParseLongPair()
                .YieldLongRange();

            if (options.HasFlag(NumberRangeOptions.RemoveDuplicates))
            {
                result = result.Distinct();
            }

            if (options.HasFlag(NumberRangeOptions.OrderAscending))
            {
                result = result.OrderBy(i => i);
            }
            else if (options.HasFlag(NumberRangeOptions.OrderDescending))
            {
                result = result.OrderByDescending(i => i);
            }

            return result;
        }

        private static IEnumerable<long> YieldLongRange(this IEnumerable<LongPair> longPairs)
        {
            foreach (var intPair in longPairs)
            {
                for (long i = intPair.LowerBound; i <= intPair.UpperBound; i++)
                {
                    yield return i;
                }
            }
        }

        private static IEnumerable<LongPair> ParseLongPair(this IEnumerable<string> source)
        {
            foreach (var s in source)
            {

                var splitAt = s.IndexOf("-", StringComparison.Ordinal);
                var result = new LongPair();
                long l, u = 0;

                if (splitAt < 0)
                {
                    result.IsValid = long.TryParse(s.Trim(), out l);
                    u = l;
                }
                else
                {
                    result.IsValid = long.TryParse(s.Substring(0, splitAt).Trim(), out l) &&
                                     long.TryParse(s.Substring(splitAt + 1).Trim(), out u);
                }

                result.IsValid = result.IsValid && l <= u;

                if (result.IsValid)
                {
                    result.LowerBound = l;
                    result.UpperBound = u;

                    yield return result;
                }
                else
                {
                    throw new Exception($"The long pair definition '{s}' is invalid. Should be n or n-m");
                }
            }
        }
    }
}
