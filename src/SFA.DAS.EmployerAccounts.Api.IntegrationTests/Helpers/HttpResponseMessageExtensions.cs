using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;

public static class HttpResponseMessageExtensions
{
    public static TContent GetContent<TContent>(this HttpResponseMessage response)
    {
        var content = response.Content.ReadAsStringAsync().Result;

        return JsonConvert.DeserializeObject<TContent>(content);
    }

    public static void ExpectStatusCodes(this HttpResponseMessage response, params HttpStatusCode[] statusCodes)
    {
        Assert.IsTrue(statusCodes.Contains(response.StatusCode), $"Received response {response.StatusCode} " +
                                                                 $"when expected any of [{string.Join(",", statusCodes.Select(sc => sc))}]. " +
                                                                 $"Additional information sent to the client: {response.ReasonPhrase}. ");
    }
}