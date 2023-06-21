using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler;

[TestFixture]
public class WhenCallingFindFinance : WhenTestingAccountHandler
{
    [Test]
    public async Task ItShouldReturnNoAccountInTheResponseIfNotFoundInFindFinance()
    {
        MockAccountRepository!.Setup(x => x.Get(Id, AccountFieldsSelection.Finance))
            .ReturnsAsync(null as Core.Models.Account);
        var actual = await Unit!.FindFinance(Id);
        Assert.That(actual.StatusCode, Is.EqualTo(SearchResponseCodes.NoSearchResultsFound));
    }

    [Test]
    public async Task ItShouldReturnTheAccountWithBalanceLookupIfNoTransactionsAreFound()
    {
        MockAccountRepository!.Setup(x => x.Get(Id, AccountFieldsSelection.Finance))
            .Returns(Task.FromResult(new Core.Models.Account
            {
                Transactions = new List<TransactionViewModel>()
            }));

        MockAccountRepository.Setup(x => x.GetAccountBalance(Id))
            .ReturnsAsync(0m);

        var actual = await Unit!.FindFinance(Id);

        MockAccountRepository.Verify(x => x.GetAccountBalance(Id), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(actual.StatusCode, Is.EqualTo(SearchResponseCodes.Success));
            Assert.That(actual.Account, Is.Not.Null);
        });
    }
}