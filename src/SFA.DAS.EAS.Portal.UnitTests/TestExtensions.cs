using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

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
        public static bool IsEqual(this object source, object expected)
        {
            return new CompareLogic().Compare(expected, source).AreEqual;
        }
    }
}
