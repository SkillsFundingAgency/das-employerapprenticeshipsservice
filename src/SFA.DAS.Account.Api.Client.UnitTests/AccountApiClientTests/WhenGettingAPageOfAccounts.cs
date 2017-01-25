using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPageOfAccounts
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
                    JsonConvert.SerializeObject(new Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>
                    {
                        Page = 1,
                        TotalPages = 1,
                        Data = new List<Dtos.AccountWithBalanceViewModel>
                        {
                            new Dtos.AccountWithBalanceViewModel
                            {
                                AccountId = 1,
                                AccountHashId = "1",
                                AccountName = "Account 1",
                                Balance = 1234567.89m,
                                Href = "http://localhost/api/accounts/1"
                            }
                        }
                    })));

            _apiClient = new AccountApiClient(_configuration, _httpClient.Object);
        }

        [TestCase(1, 1000, "20161001")]
        [TestCase(2, 100, "20161231")]
        public async Task ThenItShouldCallTheApiWithTheCorrectUrl(int pageNumber, int pageSize, string toDateString)
        {
            // Arrange
            var toYear = int.Parse(toDateString.Substring(0, 4));
            var toMonth = int.Parse(toDateString.Substring(4, 2));
            var toDay = int.Parse(toDateString.Substring(6, 2));

            // Act
            await _apiClient.GetPageOfAccounts(pageNumber, pageSize, new DateTime(toYear, toMonth, toDay));

            // Assert
            var expectedUrl = $"http://some-url/api/accounts?page={pageNumber}&pageSize={pageSize}&toDate={toDateString}";
            _httpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAPageOfAccounts()
        {
            // Act
            var actual = await _apiClient.GetPageOfAccounts();

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await _apiClient.GetPageOfAccounts();

            // Assert
            Assert.IsAssignableFrom<Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>>(actual);
        }
    }
}
