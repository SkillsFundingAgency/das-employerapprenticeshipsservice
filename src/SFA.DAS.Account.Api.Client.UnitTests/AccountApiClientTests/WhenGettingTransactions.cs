using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingTransactions
    {
        private AccountApiConfiguration _configuration;
        private Mock<SecureHttpClient> _httpClient;
        private AccountApiClient _apiClient;

        [SetUp]
        public void Arrange()
        {
            _configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };

            _httpClient = new Mock<SecureHttpClient>();
            _httpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    JsonConvert.SerializeObject(new TransactionsViewModel())));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
        {
            // Arrange
            var accountId = "ABC123";

            // Act
            await _apiClient.GetTransactions(accountId);

            // Assert
            var expectedUrl = $"http://some-url/api/accounts/{accountId}/transactions";
            _httpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnTransactions()
        {
            // Arrange
            var accountId = "ABC123";

            // Act
            var actual = await _apiClient.GetTransactions(accountId);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Arrange
            var accountId = "ABC123";

            // Act
            var actual = await _apiClient.GetTransactions(accountId);

            // Assert
            Assert.IsAssignableFrom<TransactionsViewModel>(actual);
        }
    }
}
