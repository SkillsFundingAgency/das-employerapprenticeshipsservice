using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.FinanceRepository;

[TestFixture]
public class WhenCallingGetFinanceBalance : WhenTestingFinanceRepository
{
    [Test]
    public async Task ItShouldReturnTheMatchingAccountBalanceValue()
    {
        const string id = "123";

        var balances = new List<AccountBalance>
        {
            new()
            {
                AccountId = 342,
                Balance = 1291.22m,
                IsLevyPayer = 1,
                LevyOverride = true,
                RemainingTransferAllowance = 223423.33m,
                StartingTransferAllowance = 3242.32m
            }
        };

        FinanceService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);

        var actual = await Sut!.GetAccountBalance(id);

        actual.Should().Be(balances.First().Balance);
    }

    [Test]
    public async Task ItShouldReturnZeroIfThereAreEmptyResultsFromApi()
    {
        const string id = "123";

        FinanceService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<AccountBalance>());

        var actual = await Sut!.GetAccountBalance(id);

        actual.Should().Be(0);
    }
    
    [Test]
    public async Task ItShouldReturnZeroIfThereResponseFromApiIsNull()
    {
        const string id = "123";

        FinanceService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        var actual = await Sut!.GetAccountBalance(id);

        actual.Should().Be(0);
    }
}