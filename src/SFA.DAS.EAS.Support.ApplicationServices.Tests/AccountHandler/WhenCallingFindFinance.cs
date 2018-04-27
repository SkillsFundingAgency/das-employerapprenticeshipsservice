using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenCallingFindFinance : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnNoAccountInTheResponseIfNotFoundInFindFinance()
        {
            MockAccountRepository.Setup(x => x.Get(Id, AccountFieldsSelection.Finance))
                .ReturnsAsync(null as Core.Models.Account);
            var actual = await Unit.FindFinance(Id);
            Assert.AreEqual(SearchResponseCodes.NoSearchResultsFound, actual.StatusCode);
        }

        [Test]
        public async Task ItShouldReturnTheAccountWithBalanceFromTransactionsIfFound()
        {
            MockAccountRepository.Setup(x => x.Get(Id, AccountFieldsSelection.Finance))
                .ReturnsAsync(new Core.Models.Account
                {
                    Transactions = new List<TransactionViewModel> {new TransactionViewModel {Balance = 100m}}
                });

            var actual = await Unit.FindFinance(Id);

            MockAccountRepository.Verify(x => x.GetAccountBalance(Id), Times.Never);

            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
            Assert.IsNotNull(actual.Account);
        }


        [Test]
        public async Task ItShouldReturnTheAccountWithBallanceLookupIfNoTransactionsAreFound()
        {
            MockAccountRepository.Setup(x => x.Get(Id, AccountFieldsSelection.Finance))
                .Returns(Task.FromResult(new Core.Models.Account
                {
                    Transactions = new List<TransactionViewModel>()
                }));

            MockAccountRepository.Setup(x => x.GetAccountBalance(Id))
                .ReturnsAsync(0m);

            var actual = await Unit.FindFinance(Id);

            MockAccountRepository.Verify(x => x.GetAccountBalance(Id), Times.Once);

            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
            Assert.IsNotNull(actual.Account);
        }
    }
}