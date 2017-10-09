using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingTransactions : ApiClientTestBase
    {
        public override void HttpClientSetup()
        {
            HttpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    JsonConvert.SerializeObject(new List<TransactionSummaryViewModel>())));
        }

        public async Task ThenItShouldCallTheApiWithTheCorrectUrl()
        {
            // Act
            await ApiClient.GetTransactionSummary(TextualAccountId);
            var accountId = TextualAccountId;

            // Assert
            var expectedUrl = $"http://some-url/api/accounts/{accountId}/transactions";
            HttpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnTransactions()
        {
            // Act
            var actual = await ApiClient.GetTransactionSummary(TextualAccountId);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await ApiClient.GetTransactionSummary(TextualAccountId);

            // Assert
            Assert.IsAssignableFrom<List<TransactionSummaryViewModel>>(actual);
        }
    }
}
