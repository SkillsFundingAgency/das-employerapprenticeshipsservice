using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests;

public class WhenPinging : ApiClientTestBase
{
    protected override void HttpClientSetup()
    {
        HttpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult(""));
    }

    [Test]
    public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
    {
        // Act
        await ApiClient!.Ping();

        // Assert
        const string expectedUrl = $"http://some-url/api/ping";
        HttpClient!.Verify(c => c.GetAsync(expectedUrl), Times.Once);
    }
}