using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Account;

public abstract class WhenTestingAccountController
{
    protected Mock<IAccountHandler>? AccountHandler;
    private Mock<IPayeLevySubmissionsHandler>? _payeLevySubmissionsHandler;
    private Mock<IPayeLevyMapper>? _payeLevyDeclarationMapper;
    protected AccountController? Unit;

    [SetUp]
    public void Setup()
    {
        AccountHandler = new Mock<IAccountHandler>();
        _payeLevySubmissionsHandler = new Mock<IPayeLevySubmissionsHandler>();
        _payeLevyDeclarationMapper = new Mock<IPayeLevyMapper>();

        Unit = new AccountController(
            Mock.Of<IEasSupportConfiguration>(),
            AccountHandler.Object,
            _payeLevySubmissionsHandler.Object,
            _payeLevyDeclarationMapper.Object,
            Mock.Of<ILogger<AccountController>>()
            );
    }
}

[TestFixture]
public class WhenTestingIndexGet : WhenTestingAccountController
{
    [Test]
    public async Task ItShouldReturnAViewAndModelOnSuccess()
    {
        var response = new AccountDetailOrganisationsResponse
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
        AccountHandler!.Setup(x => x.FindOrganisations(id)).ReturnsAsync(response);
        var actual = await Unit!.Index("123");

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
        var response = new AccountDetailOrganisationsResponse
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
        AccountHandler!.Setup(x => x.FindOrganisations(id)).ReturnsAsync(response);
        var actual = await Unit!.Index("123");
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task ItShouldReturnHttpNotFoundOnSearchFailed()
    {
        var response = new AccountDetailOrganisationsResponse
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
        AccountHandler!.Setup(x => x.FindOrganisations(id)).ReturnsAsync(response);
        var actual = await Unit!.Index("123");

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }
}