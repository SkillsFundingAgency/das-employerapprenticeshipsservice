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
        var actual = await Unit!.PayeSchemes("123");

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<ViewResult>());
        Assert.That(true, Is.EqualTo(string.IsNullOrEmpty(((ViewResult)actual).ViewName)));
        Assert.That(((ViewResult)actual).Model, Is.InstanceOf<AccountDetailViewModel>());
        Assert.That(response.Account, Is.EqualTo(((AccountDetailViewModel)((ViewResult)actual).Model!).Account));
        Assert.That(((AccountDetailViewModel)((ViewResult)actual).Model!).SearchUrl, Is.Null);
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
        
        var actual = await Unit!.PayeSchemes("123");
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
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
            StatusCode = SearchResponseCodes.SearchFailed
        };
            
        const string id = "123";
            
        AccountHandler!.Setup(x => x.FindPayeSchemes(id)).ReturnsAsync(response);
        var actual = await Unit!.PayeSchemes("123");

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }
}