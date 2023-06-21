using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository;

[TestFixture]
public class WhenCallingGetAccountBalance : WhenTestingAccountRepository
{
    [Test]
    public async Task ItShouldReturnAZeroBalanceWhenTheClientThrowsAnException()
    {
        const string id = "123";

        var exception = new Exception("Some exception");

        AccountApiClient!
            .Setup(x => x.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}"))
            .ThrowsAsync(exception);

        Logger!.Setup(x => x.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ));
        Logger.Setup(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ));

        var actual = await Sut!.GetAccountBalance(id);

        Logger.Verify(x => x.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Never);
            
        Logger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);

        Assert.That(actual, Is.EqualTo(0));
    }

    [Test]
    public async Task ItShouldReturnTheMatchingAccountBalanceValue()
    {
        const string id = "123";
        var response = new AccountWithBalanceViewModel
        {
            AccountId = 123,
            AccountHashId = "ERWERW",
            AccountName = "Test Account",
            Balance = 0m,
            Href = "https://tempuri.org/account/{id}",
            IsLevyPayer = true
        };

        AccountApiClient!.Setup(x => x.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}"))
            .ReturnsAsync(response);

        var actual = await Sut!.GetAccountBalance(id);
            
        Assert.That(actual, Is.EqualTo(response.Balance));
    }
}