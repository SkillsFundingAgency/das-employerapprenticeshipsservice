using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using AccountWithBalanceViewModel = SFA.DAS.EAS.Account.Api.Types.AccountWithBalanceViewModel;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.EmployerAccountsControllerTests;

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
                new() { AccountHashId = "ABC123", AccountId = 123, AccountName = "Test 1", IsAllowedPaymentOnService = true },
                new() { AccountHashId = "ABC999", AccountId = 987, AccountName = "Test 2", IsAllowedPaymentOnService = true }
            }
        };

        EmployerAccountsApiService!.Setup(s => s.GetAccounts(null, 1000, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountsResponse);

        var response = new List<AccountBalance>
        {
            new() { AccountId = 123, Balance =10000 },
            new() { AccountId = 987, Balance =10000 },
        };
           
        EmployerFinanceApiService!.Setup(x => x.GetAccountBalances(It.IsAny<List<string>>(), CancellationToken.None)).ReturnsAsync(response);

        //Act
        var result = await Controller!.GetAccounts();
            
        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<PagedApiResponseViewModel<AccountWithBalanceViewModel>>>();

        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var oKResult = result.Result as OkObjectResult;
          
        Assert.Multiple(() =>
        {
            Assert.That(oKResult!.Value, Is.Not.Null);
            Assert.That(oKResult.Value, Is.InstanceOf<PagedApiResponseViewModel<AccountWithBalanceViewModel>>());
        });
            
        var value = oKResult!.Value as PagedApiResponseViewModel<AccountWithBalanceViewModel>;

        value.Should().NotBeNull();
        value.Should().BeEquivalentTo(accountsResponse);
    }
}