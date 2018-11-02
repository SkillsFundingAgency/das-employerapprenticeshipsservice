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
        private class IntPair
        {
            public int LowerBound { get; set; }
            public int UpperBound { get; set; }
            public bool IsValid { get; set; }
        }

        public static IEnumerable<int> ToInts(string definition)
        {
            return ToInts(definition, NumberRangeOptions.None);
        }

        public static IEnumerable<int> ToInts(string definition, NumberRangeOptions options)
        {
            var result = definition
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ParseIntPair()
                .YieldIntRange();

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

        private static IEnumerable<int> YieldIntRange(this IEnumerable<IntPair> intPairs)
        {
            foreach (var intPair in intPairs)
            {
                for (int i = intPair.LowerBound; i <= intPair.UpperBound; i++)
                {
                    yield return i;
                }
            }
        }

        private static IEnumerable<IntPair> ParseIntPair(this IEnumerable<string> source)
        {
            foreach (var s in source)
            {

                var splitAt = s.IndexOf("-", StringComparison.Ordinal);
                var result = new IntPair();
                int l, u = 0;

                if (splitAt < 0)
                {
                    result.IsValid = int.TryParse(s.Trim(), out l);
                    u = l;
                }
                else
                {
                    result.IsValid = int.TryParse(s.Substring(0, splitAt).Trim(), out l) &&
                                     int.TryParse(s.Substring(splitAt + 1).Trim(), out u);
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
                    throw new Exception($"The int pair definition '{s}' is invalid. Should be n or n-m");
                }
            }
        }
    }
}
