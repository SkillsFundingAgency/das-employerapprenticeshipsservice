using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Domain.Models.Account;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AccountWithBalanceViewModel = SFA.DAS.EAS.Account.Api.Types.AccountWithBalanceViewModel;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    [TestFixture]
    public class WhenIGetAccounts : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenAccountsAreReturnedWithTheirBalanceAndAUriToGetAccountDetails()
        {
            //Arrange
            var accountsResponse = new PagedApiResponseViewModel<AccountWithBalanceViewModel>()
            {
                Page = 123,
                TotalPages = 123,
                Data = new List<AccountWithBalanceViewModel>
                {
                    new AccountWithBalanceViewModel { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", IsLevyPayer = true },
                    new AccountWithBalanceViewModel { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", IsLevyPayer = true }
                }
            };

            ApiService.Setup(s => s.GetAccounts(null, 1000, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);         

            var response = new GetAccountBalancesResponse
            {
                Accounts = new List<AccountBalance>
                {
                    new AccountBalance { AccountId = 123, Balance =10000 },
                    new AccountBalance { AccountId = 987, Balance =10000 },
                }
            };

            FinanceApiService.Setup(x => x.GetAccountBalances(It.IsAny<AccountBalanceRequest>())).ReturnsAsync(response);

            //Act
            var result = await Controller.GetAccounts();
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>>();

            var okResult = (OkNegotiatedContentResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>) result;
            okResult.Content.Should().NotBeNull();
            okResult.Content.ShouldBeEquivalentTo(accountsResponse);
        }
    }
}
