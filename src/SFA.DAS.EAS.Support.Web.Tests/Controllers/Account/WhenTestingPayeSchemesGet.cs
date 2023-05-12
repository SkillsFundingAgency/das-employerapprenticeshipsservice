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

        Assert.IsNotNull(actual);
        Assert.IsNotNull(actual);
        Assert.IsInstanceOf<ViewResult>(actual);
        Assert.AreEqual(true, string.IsNullOrEmpty(((ViewResult)actual).ViewName));
        Assert.IsInstanceOf<AccountDetailViewModel>(((ViewResult)actual).Model);
        Assert.AreEqual(response.Account, ((AccountDetailViewModel)((ViewResult)actual).Model!).Account);
        Assert.IsNull(((AccountDetailViewModel)((ViewResult)actual).Model!).SearchUrl);
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
        Assert.IsNotNull(actual);
        Assert.IsInstanceOf<NotFoundResult>(actual);
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

        Assert.IsNotNull(actual);
        Assert.IsInstanceOf<NotFoundResult>(actual);
    }
}