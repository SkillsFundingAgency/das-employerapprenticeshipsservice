﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenPinging : ApiClientTestBase
    {
        public override void HttpClientSetup()
        {
            HttpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(""));
        }

        [Test]
        public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
        {
            // Act
            await ApiClient.Ping();

            // Assert
            var expectedUrl = $"http://some-url/api/ping";
            HttpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }
    }
}
