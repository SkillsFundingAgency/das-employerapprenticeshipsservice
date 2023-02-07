using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.HomeControllerTests;

public class WhenIComeInFromSaveAndSearch
{
    private Mock<IAuthenticationService> _owinWrapper;
    private HomeController _homeController;
    private EmployerAccountsConfiguration _configuration;
    private Mock<HomeOrchestrator> _homeOrchestrator;
    private Mock<ICookieStorageService<ReturnUrlModel>> _returnUrlCookieStorageService;

    private string _expectedEmail;
    private string _expectedId;
    private string _expectedFirstName;
    private string _expectedLastName;
    private string _expectedReturnUrl;
    private string _expectedCorrelationId;
    private IActionResult _actualResult;

    [SetUp]
    public async Task Arrange()
    {
        _owinWrapper = new Mock<IAuthenticationService>();
        _configuration = new EmployerAccountsConfiguration();
        _homeOrchestrator = new Mock<HomeOrchestrator>();
        _returnUrlCookieStorageService = new Mock<ICookieStorageService<ReturnUrlModel>>();

        _homeController = new HomeController(
            _homeOrchestrator.Object,
            _configuration,
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            _returnUrlCookieStorageService.Object,
                Mock.Of<ILogger<HomeController>>());


        _expectedEmail = "test@test.com";
        _expectedId = "123456";
        _expectedFirstName = "Test";
        _expectedLastName = "tester";
        _expectedReturnUrl = "campaign page";
        _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns(_expectedId);
        _owinWrapper.Setup(x => x.GetClaimValue("email")).Returns(_expectedEmail);
        _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.GivenName)).Returns(_expectedFirstName);
        _owinWrapper.Setup(x => x.GetClaimValue(DasClaimTypes.FamilyName)).Returns(_expectedLastName);

        _actualResult = await _homeController.SaveAndSearch(_expectedReturnUrl);
    }

    [Test]
    public void ThenTheClaimsAreRefreshed()
    {
        _owinWrapper.Verify(x => x.UpdateClaims());
    }

    [Test]
    public void ThenTheUpdatedIdentityAttributesAreSaved()
    {
        _homeOrchestrator.Verify(x => x.SaveUpdatedIdentityAttributes(_expectedId, _expectedEmail, _expectedFirstName, _expectedLastName, _expectedCorrelationId));
    }

    [Test]
    public void ThenTheReturnUrlIsStoredInTheCookieService()
    {
        _returnUrlCookieStorageService.Verify(x => x.Create(It.Is<ReturnUrlModel>(model => model.Value == _expectedReturnUrl), "SFA.DAS.EmployerAccounts.Web.Controllers.ReturnUrlCookie", It.IsAny<int>()));
    }

    [Test]
    public void ThenTheRedirectIsToGetApprenticeshipFunding()
    {
        Assert.That(_actualResult, Is.Not.Null);
        Assert.That(_actualResult, Is.AssignableFrom<RedirectToRouteResult>());
        var actualRedirect = _actualResult as RedirectToRouteResult;
        Assert.That(actualRedirect, Is.Not.Null);
        Assert.That(actualRedirect.RouteValues["controller"], Is.EqualTo(ControllerConstants.EmployerAccountControllerName));
        Assert.That(actualRedirect.RouteValues["action"], Is.EqualTo(ControllerConstants.GetApprenticeshipFundingActionName));
    }
}