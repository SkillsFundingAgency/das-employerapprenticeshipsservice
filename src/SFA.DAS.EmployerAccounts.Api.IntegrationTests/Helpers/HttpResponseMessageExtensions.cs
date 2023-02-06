using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

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

    public static void ExpectStatusCodes(this HttpResponseMessage response, params HttpStatusCode[] statuscodes)
    {
        Assert.IsTrue(statuscodes.Contains(response.StatusCode), $"Received response {response.StatusCode} " +
                                                                 $"when expected any of [{string.Join(",", statuscodes.Select(sc => sc))}]. " +
                                                                 $"Additional information sent to the client: {response.ReasonPhrase}. ");
    }

}