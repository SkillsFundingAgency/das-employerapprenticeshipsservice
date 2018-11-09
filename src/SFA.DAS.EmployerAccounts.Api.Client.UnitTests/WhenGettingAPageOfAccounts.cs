using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    public class WhenGettingAPageOfAccounts : ApiClientTestBase
    {
        public override void HttpClientSetup()
        {
            HttpClient.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    JsonConvert.SerializeObject(new PagedApiResponseViewModel<AccountWithBalanceViewModel>
                    {
                        Page = 1,
                        TotalPages = 1,
                        Data = new List<AccountWithBalanceViewModel>
                        {
                            new AccountWithBalanceViewModel
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
            await ApiClient.GetPageOfAccounts(pageNumber, pageSize, new DateTime(toYear, toMonth, toDay));

            // Assert
            var expectedUrl = $"http://some-url/api/accounts?pageNumber={pageNumber}&pageSize={pageSize}&toDate={toDateString}";
            HttpClient.Verify(c => c.GetAsync(expectedUrl), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnAPageOfAccounts()
        {
            // Act
            var actual = await ApiClient.GetPageOfAccounts();

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldDeserializeTheResponseCorrectly()
        {
            // Act
            var actual = await ApiClient.GetPageOfAccounts();

            // Assert
            Assert.IsAssignableFrom<PagedApiResponseViewModel<AccountWithBalanceViewModel>>(actual);
        }
    }
}
