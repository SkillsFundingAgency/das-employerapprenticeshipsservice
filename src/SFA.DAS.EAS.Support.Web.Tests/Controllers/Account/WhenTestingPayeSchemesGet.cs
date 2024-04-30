using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account;

[TestFixture]
public class WhenTestingPayeSchemesGet : WhenTestingAccountController
{
    [Test]
    public async Task ItShouldReturnAViewAndModelOnSuccess()
    {
        // Arrange
        var response = new AccountPayeSchemesResponse
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
        AccountHandler!.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(response);
        
        // Act
        var actual = await Unit!.PayeSchemes("123");

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeAssignableTo<ViewResult>();
        result.Subject.ViewName.Should().BeEmpty();
        var model = result.Subject.Model.Should().BeAssignableTo<AccountDetailViewModel>();
        model.Subject.Account.Should().BeEquivalentTo(response.Account);
        model.Subject.SearchUrl.Should().BeNull();
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
    {
        var response = new AccountPayeSchemesResponse
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
        AccountHandler!.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(response);
        
        // Act
        var actual = await Unit!.PayeSchemes("123");
        
        // Assert
        actual.Should().BeAssignableTo<NotFoundResult>();
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
    {
        // Arrange
        var response = new AccountPayeSchemesResponse
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
            
        AccountHandler!.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(response);
        
        // Act
        var actual = await Unit!.PayeSchemes("123");

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<NotFoundResult>();
    }
}