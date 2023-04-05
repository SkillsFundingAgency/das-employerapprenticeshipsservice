using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
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

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var oKResult = result.Result as OkObjectResult;

            Assert.IsNotNull(oKResult.Value);
            Assert.IsInstanceOf<PagedApiResponseViewModel<AccountWithBalanceViewModel>>(oKResult.Value);

            var value = oKResult.Value as PagedApiResponseViewModel<AccountWithBalanceViewModel>;

            value.Should().NotBeNull();
            value.Should().BeEquivalentTo(accountsResponse);
        }
    }
}
