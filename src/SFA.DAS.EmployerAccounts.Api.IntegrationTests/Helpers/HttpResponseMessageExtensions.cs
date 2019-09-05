using System.Net.Http;
using FluentAssertions;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    public static class HttpResponseMessageExtensions
    {
        public static void ShouldHaveContentOfType<TContent>(this HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            TContent resources = JsonConvert.DeserializeObject<TContent>(content);

            resources
                .Should()
                .BeOfType<TContent>();
        }

        public static TContent GetContent<TContent>(this HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            return
                JsonConvert.DeserializeObject<TContent>(content);
        }
    }
}