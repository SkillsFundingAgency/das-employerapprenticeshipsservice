using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.UnitTests.Infrastructure;

public class WhenHandlingAGetRequest
{
    [Test]
    public async Task Then_The_Endpoint_Is_Called_With_Authentication_Header_And_Data_Returned()
    {
        //Arrange
        const string key = "123-abc-567";
        var getTestRequest = new GetTestRequest();
        var testObject = new List<string>();
        var config = new EmployerAccountsOuterApiConfiguration { BaseUrl = "http://valid-url/", Key = key };

        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(testObject)),
            StatusCode = HttpStatusCode.Accepted
        };

        var httpMessageHandler = SetupMessageHandlerMock(response, $"{config.BaseUrl}{getTestRequest.GetUrl}", config.Key);
        var httClient = new HttpClient(httpMessageHandler.Object) { BaseAddress = new Uri(config.BaseUrl) };
        httClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.Key);
        httClient.DefaultRequestHeaders.Add("X-Version", "1");

        var apiClient = new OuterApiClient(httClient);

        //Act
        var actual = await apiClient.Get<List<string>>(getTestRequest);

        //Assert
        actual.Should().BeEquivalentTo(testObject);
    }

    [Test]
    public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown()
    {
        //Arrange
        const string key = "123-abc-567";
        var getTestRequest = new GetTestRequest();
        var config = new EmployerAccountsOuterApiConfiguration { BaseUrl = "http://valid-url/", Key = key };
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.BadRequest
        };

        var httpMessageHandler = SetupMessageHandlerMock(response, $"{config.BaseUrl}{getTestRequest.GetUrl}", config.Key);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new OuterApiClient(client);

        //Act Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => apiClient.Get<List<string>>(getTestRequest));
    }

    private class GetTestRequest : IGetApiRequest
    {
        public string GetUrl => "test-url/get";
    }

    private static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, string url, string key)
    {
        var httpMessageHandler = new Mock<HttpMessageHandler>();

        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.Headers.Contains("Ocp-Apim-Subscription-Key")
                    && c.Headers.GetValues("Ocp-Apim-Subscription-Key").Single().Equals(key)
                    && c.Headers.Contains("X-Version")
                    && c.Headers.GetValues("X-Version").Single().Equals("1")
                    && c.RequestUri.AbsoluteUri.Equals(url)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return httpMessageHandler;
    }
}