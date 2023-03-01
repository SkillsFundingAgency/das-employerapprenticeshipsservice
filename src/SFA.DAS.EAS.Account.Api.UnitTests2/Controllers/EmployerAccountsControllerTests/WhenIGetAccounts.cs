using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                    new AccountWithBalanceViewModel { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", IsAllowedPaymentOnService = true },
                    new AccountWithBalanceViewModel { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", IsAllowedPaymentOnService = true }
                }
            };

            _employerAccountsApiService.Setup(s => s.GetAccounts(null, 1000, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);

            var response = new List<AccountBalance>
                {
                    new AccountBalance { AccountId = 123, Balance =10000 },
                    new AccountBalance { AccountId = 987, Balance =10000 },
                };
           

            _employerFinanceApiService.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>())).ReturnsAsync(response);

            //Act
            var result = await _controller.GetAccounts();
            
            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>>();

            var okResult = (ActionResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>) result;
            okResult.Value.Should().NotBeNull();
            okResult.Value.ShouldBeEquivalentTo(accountsResponse);
        }
    }
}
