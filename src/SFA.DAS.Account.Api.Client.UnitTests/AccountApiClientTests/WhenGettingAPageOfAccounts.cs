using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPageOfAccounts : ApiClientTestBase
    {
        protected override void HttpClientSetup()
        {
            HttpClient!.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    JsonConvert.SerializeObject(new PagedApiResponseViewModel<AccountWithBalanceViewModel>
                    {
                        Page = 1,
                        TotalPages = 1,
                        Data = new List<AccountWithBalanceViewModel>
                        {
                            new()
                            {
                                AccountId = 1,
                                AccountHashId = "1",
                                PublicAccountHashId = "2",
                                AccountName = "Account 1",
                                Balance = 1234567.89m,
                                Href = "/api/accounts/1"
                            }
                        }
                    })));
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
            await ApiClient!.GetPageOfAccounts(pageNumber, pageSize, new DateTime(toYear, toMonth, toDay));

            // Assert
            var expectedUrl = $"http://some-url/api/accounts?pageNumber={pageNumber}&pageSize={pageSize}&toDate={toDateString}";
            HttpClient!.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAPageOfAccounts()
        {
            // Act
            var actual = await ApiClient!.GetPageOfAccounts();

            // Assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await ApiClient!.GetPageOfAccounts();

            // Assert
            Assert.That(actual, Is.AssignableFrom<PagedApiResponseViewModel<AccountWithBalanceViewModel>>());
        }
    }
}