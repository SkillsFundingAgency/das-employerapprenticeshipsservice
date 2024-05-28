using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account;

[TestFixture]
public class WhenTestingHeaderGet : WhenTestingAccountController
{
    [Test]
    public async Task ItShouldReturnHttpNotFoundOnNoSearchResultsFound()
    {
        var accountResponse = new AccountReponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        };
            
        const string id = "123";
        AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
        var actual = await Unit!.Header(id);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
    {
        var accountResponse = new AccountReponse
        {
            StatusCode = SearchResponseCodes.SearchFailed
        };
        const string id = "123";
        AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
        var actual = await Unit!.Header(id);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ItShouldReturnTheSubHeaderViewAndModelOnSuccess()
    {
        var accountResponse = new AccountReponse
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
        AccountHandler!.Setup(x => x.Find(id)).ReturnsAsync(accountResponse);
        var actual = await Unit!.Header(id);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<ViewResult>());
        Assert.That("SubHeader", Is.EqualTo(((ViewResult)actual).ViewName));
        Assert.That(((ViewResult)actual).Model, Is.InstanceOf<Core.Models.Account>());
        Assert.That(accountResponse.Account, Is.EqualTo(((ViewResult)actual).Model));
    }
}