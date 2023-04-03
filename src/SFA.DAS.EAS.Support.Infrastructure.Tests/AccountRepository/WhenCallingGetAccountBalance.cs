using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetAccountBalance : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnAZeroBalanceWhenTheClientThrowsAnException()
        {
            var id = "123";

            var exception = new Exception("Some exception");

            AccountApiClient
                .Setup(x => x.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}"))
                .ThrowsAsync(exception);

            Logger.Setup(x => x.LogDebug(It.IsAny<string>()));
            Logger.Setup(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()));

            var actual = await _sut.GetAccountBalance(id);

            Logger.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Never);
            Logger.Verify(x => x.LogError(exception, $"Account Balance with id {id} not found"), Times.Once);

            Assert.AreEqual(0, actual);
        }

        [Test]
        public async Task ItShouldReturnTheMatchingAccountBalanceValue()
        {
            var id = "123";
            var response = new AccountWithBalanceViewModel
            {
                AccountId = 123,
                AccountHashId = "ERWERW",
                AccountName = "Test Account",
                Balance = 0m,
                Href = "https://tempuri.org/account/{id}",
                IsLevyPayer = true
            };

            AccountApiClient.Setup(x => x.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(response);

            var actual = await _sut.GetAccountBalance(id);

            Logger.Verify(x =>
                x.LogDebug(
                    $"{nameof(IAccountApiClient)}.{nameof(IAccountApiClient.GetResource)}<{nameof(AccountWithBalanceViewModel)}>(\"/api/accounts/{id}\"); {response.Balance}"));
            Assert.AreEqual(response.Balance, actual);
        }
    }
}