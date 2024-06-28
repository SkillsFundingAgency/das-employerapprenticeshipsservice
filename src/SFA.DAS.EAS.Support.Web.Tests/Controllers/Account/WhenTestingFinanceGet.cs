using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account;

[TestFixture]
public class WhenTestingFinanceGet : WhenTestingAccountController
{
    [Test]
    public async Task ItShouldReturnAViewAndModelOnSuccess()
    {
        var accountFinanceResponse = new AccountFinanceResponse
        {
            Account = new Core.Models.Account
            {
                AccountId = 123,
                DasAccountName = "Test Account",
                DateRegistered = DateTime.Today,
                OwnerEmail = "owner@tempuri.org"
            },
            StatusCode = SearchResponseCodes.Success
        };
        const string id = "123";
        AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
        var actual = await Unit!.Finance("123");

        // Asset
        actual.Should().NotBeNull();
        var result = actual.Should().BeAssignableTo<ViewResult>();
        result.Subject.ViewName.Should().BeNull();
        var model = result.Subject.Model.Should().BeAssignableTo<FinanceViewModel>();
        model.Subject.Account.Should().BeEquivalentTo(accountFinanceResponse.Account);
        model.Subject.Balance.Should().Be(accountFinanceResponse.Balance);
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
    {
        var accountFinanceResponse = new AccountFinanceResponse
        {
            Account = new Core.Models.Account
            {
                AccountId = 123,
                DasAccountName = "Test Account",
                DateRegistered = DateTime.Today,
                OwnerEmail = "owner@tempuri.org"
            },
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };
        const string id = "123";
        AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
        var actual = await Unit!.Finance("123");
        
        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<NotFoundResult>();
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
    {
        var accountFinanceResponse = new AccountFinanceResponse
        {
            Account = new Core.Models.Account
            {
                AccountId = 123,
                DasAccountName = "Test Account",
                DateRegistered = DateTime.Today,
                OwnerEmail = "owner@tempuri.org"
            },
            StatusCode = SearchResponseCodes.SearchFailed
        };
            
        const string id = "123";
        AccountHandler!.Setup(x => x.FindFinance(id)).ReturnsAsync(accountFinanceResponse);
        var actual = await Unit!.Finance("123");

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<NotFoundResult>();
    }
}