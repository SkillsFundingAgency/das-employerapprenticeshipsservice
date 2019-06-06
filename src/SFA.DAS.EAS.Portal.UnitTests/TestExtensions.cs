using System;
using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests
{
    public static class TestExtensions
    {
        public static T Clone<T>(this T source)
        {
            if (ReferenceEquals(source, null)) return default;

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        /// <summary>
        /// Are two test objects equal, without having to implement Equals, GetHashCode, operators
        /// </summary>
        public static (bool, string) IsEqual(this object source, object expected)
        {
            var comparisonResult = new CompareLogic(new ComparisonConfig
                {
                    MaxDifferences = 100,
                    IgnoreCollectionOrder = true,
                    //DecimalPrecision = 0.001m
                    //todo: hack. *appears* to be bug in CompareNetObjects!?
                    ClassTypesToIgnore = new List<Type>() {typeof(Apprenticeship)}
                })
                .Compare(expected, source);
            return (comparisonResult.AreEqual,comparisonResult.DifferencesString);
        }
        
        /// <summary>
        /// Return a random element from the IEnumerable.
        /// A simplified version of https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/RandomSubset.cs
        /// </summary>
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            var sourceCollection = source as ICollection<T>;
            return source.Skip(new Random().Next(sourceCollection?.Count ?? source.Count())).First();
        }
    }
}
