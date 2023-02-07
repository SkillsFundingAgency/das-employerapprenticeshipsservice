using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenSupportUserBannerIsRendered
{
    private EmployerTeamController _controller;

    private Mock<ICookieStorageService<FlashMessageViewModel>> _mockCookieStorageService;
    private Mock<EmployerTeamOrchestrator> _mockEmployerTeamOrchestrator;
    private Mock<ControllerContext> _mockControllerContext;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<ClaimsPrincipal> _mockPrincipal;
    private Mock<ClaimsIdentity> _mockClaimsIdentity;
    private bool _isAuthenticated = true;
    private List<Claim> _claims;
    private OrchestratorResponse<AccountDashboardViewModel> _orchestratorResponse;
    private OrchestratorResponse<AccountSummaryViewModel> _orchestratorAccountSummaryResponse;
    private AccountDashboardViewModel _accountViewModel;
    private AccountSummaryViewModel _accountSummaryViewModel;
    private Account _account;
    private string _hashedAccountId;
    private string _userId;

    [SetUp]
    public void Arrange()
    {
        _mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
        _mockControllerContext = new Mock<ControllerContext>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockPrincipal = new Mock<ClaimsPrincipal>();
        _mockClaimsIdentity = new Mock<ClaimsIdentity>();

        _userId = "TestUser";

        _claims = new List<Claim>
        {
            new Claim(ControllerConstants.UserRefClaimKeyName, _userId)
        };

        _mockPrincipal.Setup(m => m.Identity).Returns(_mockClaimsIdentity.Object);
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockHttpContext.Setup(m => m.User).Returns(_mockPrincipal.Object);
        _mockControllerContext.Setup(m => m.HttpContext).Returns(_mockHttpContext.Object);

        _hashedAccountId = Guid.NewGuid().ToString();
        _account = new Account
        {
            PublicHashedId = _hashedAccountId
        };

        _accountViewModel = new AccountDashboardViewModel
        {
            Account = _account
        };

        _accountSummaryViewModel = new AccountSummaryViewModel
        {
            Account = _account
        };

        _orchestratorResponse = new OrchestratorResponse<AccountDashboardViewModel>()
        {
            Status = System.Net.HttpStatusCode.OK,
            Data = _accountViewModel
        };

        _orchestratorAccountSummaryResponse = new OrchestratorResponse<AccountSummaryViewModel>()
        {
            Status = System.Net.HttpStatusCode.OK,
            Data = _accountSummaryViewModel
        };

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccount(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(_orchestratorResponse);

        _controller = new EmployerTeamController(
            _mockCookieStorageService.Object,
            _mockEmployerTeamOrchestrator.Object)
        {
            ControllerContext = _mockControllerContext.Object
        };
    }

    [Test]
    public void ThenForAuthenticatedSupportUserAndNullModelThe_SupportUserBannerViewIsReturnedWithANullAccount()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));

        //Act
        var result = _controller.SupportUserBanner(null) as PartialViewResult;

        //Assert
        Assert.AreEqual("_SupportUserBanner", result.ViewName);
        Assert.IsNull(((SupportUserBannerViewModel)result.Model).Account);
    }

    [Test]
    public void ThenForAuthenticatedSupportUserAndNullHashedAccountId_SupportUserBannerViewIsReturnedWithANullAccount()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        var model = new TestModel(null);

        //Act
        var result = _controller.SupportUserBanner(model) as PartialViewResult;

        //Assert            
        Assert.AreEqual("_SupportUserBanner", result.ViewName);
        Assert.IsNull(((SupportUserBannerViewModel)result.Model).Account);
    }

    [Test]
    public void ThenForAuthenticatedSupportUser_TheAccountIsRetrievedFromTheOrchestrator()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        var model = new TestModel(_hashedAccountId);

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(_orchestratorAccountSummaryResponse);
        //Act
        var result = _controller.SupportUserBanner(model) as PartialViewResult;

        //Assert
        _mockEmployerTeamOrchestrator.Verify(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void ThenForAuthenticatedSupportUser_TheExpectedAccountIsPassedTotheView()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        var model = new TestModel(_hashedAccountId);

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(_orchestratorAccountSummaryResponse);

        //Act
        var result = _controller.SupportUserBanner(model) as PartialViewResult;

        //Assert
        Assert.AreSame(_account, ((SupportUserBannerViewModel)result.Model).Account);
    }

    [Test]
    public void ThenForAuthenticatedSupportUserAndTheAccountOrchestratorErrors_SupportUserBannerViewIsReturnedWithANullAccount()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        var model = new TestModel(_hashedAccountId);

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccount(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<AccountDashboardViewModel> { Status = System.Net.HttpStatusCode.BadRequest });

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId,It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<AccountSummaryViewModel> { Status = System.Net.HttpStatusCode.BadRequest });

        //Act
        var result = _controller.SupportUserBanner(model) as PartialViewResult;

        //Assert
        Assert.IsNull(((SupportUserBannerViewModel)result.Model).Account);
    }

    internal class TestModel : IAccountIdentifier
    {
        public string HashedAccountId { get; set; }

        public TestModel(string hashedAccountId)
        {
            HashedAccountId = hashedAccountId;
        }
    }
}