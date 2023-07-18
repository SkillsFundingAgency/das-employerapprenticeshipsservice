using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nest;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenSupportUserBannerIsRendered
{
    private const string UserId = "Useramo187";
    private SupportUserBannerViewComponent _viewComponentContext;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<ClaimsPrincipal> _mockPrincipal;
    private Mock<ClaimsIdentity> _mockClaimsIdentity;
    private bool _isAuthenticated = true;
    private List<Claim> _claims;
    private Mock<EmployerTeamOrchestratorWithCallToAction> _mockEmployerTeamOrchestrator;

    private OrchestratorResponse<AccountSummaryViewModel> _orchestratorAccountSummaryResponse;
    private AccountSummaryViewModel _accountSummaryViewModel;
    private Account _account;
    private string _hashedAccountId;

    [SetUp]
    public void Arrange()
    {
        var userRefClaim = new Claim(ControllerConstants.UserRefClaimKeyName, UserId);
        _mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestratorWithCallToAction>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockPrincipal = new Mock<ClaimsPrincipal>();
        _mockClaimsIdentity = new Mock<ClaimsIdentity>();
        _claims = new List<Claim> { userRefClaim };

        _mockPrincipal.Setup(m => m.Identity).Returns(_mockClaimsIdentity.Object);
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
        _mockPrincipal.Setup(x => x.Claims).Returns(_claims);
        _mockHttpContext.Setup(m => m.User).Returns(_mockPrincipal.Object);

        _mockPrincipal.Setup(m => m.FindFirst(ControllerConstants.UserRefClaimKeyName)).Returns(userRefClaim);
        _httpContextAccessorMock.SetupGet(m => m.HttpContext).Returns(_mockHttpContext.Object);

        _hashedAccountId = Guid.NewGuid().ToString();
        _account = new Account
        {
            PublicHashedId = _hashedAccountId
        };

        _accountSummaryViewModel = new AccountSummaryViewModel
        {
            Account = _account
        };

        _orchestratorAccountSummaryResponse = new OrchestratorResponse<AccountSummaryViewModel>()
        {
            Status = HttpStatusCode.OK,
            Data = _accountSummaryViewModel
        };

        _viewComponentContext = new SupportUserBannerViewComponent(_mockEmployerTeamOrchestrator.Object, _httpContextAccessorMock.Object);
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUser_WithTier2Claim_ShouldReturnTier2()
    {
        // Arrange
        var supportClaim = new Claim(ClaimTypes.Role, SupportUserClaimConstants.Tier2UserClaim);
        _isAuthenticated = true;
        _claims.Add(supportClaim);
        _mockPrincipal.Setup(m => m.FindFirst(ClaimTypes.Role)).Returns(supportClaim);

        //Act
        var result = await _viewComponentContext.InvokeAsync(null) as ViewViewComponentResult;
        var model = result.ViewData.Model as SupportUserBannerViewModel;

        //Assert
        model.ConsoleUserType.Should().Be("Service user (T2 Support)");
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUser_WithoutTier2Claim_ShouldReturnStandardUser()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimTypes.Role, SupportUserClaimConstants.Tier1UserClaim));

        //Act
        var result = await _viewComponentContext.InvokeAsync(null) as ViewViewComponentResult;
        var model = result.ViewData.Model as SupportUserBannerViewModel;

        //Assert
        model.ConsoleUserType.Should().Be("Standard user");
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUserAndNullHashedAccountId_SupportUserBannerViewIsReturnedWithANullAccount()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimTypes.Role, ControllerConstants.Tier2UserClaim));

        //Act
        var result = await _viewComponentContext.InvokeAsync(null) as ViewViewComponentResult;
        var model = result.ViewData.Model as SupportUserBannerViewModel;
        //Assert
        model.Account.Should().BeNull();
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUser_TheAccountIsRetrievedFromTheOrchestrator()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimTypes.Role, ControllerConstants.Tier2UserClaim));
        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(_orchestratorAccountSummaryResponse);
        //Act
        var result = await _viewComponentContext.InvokeAsync(_hashedAccountId) as ViewViewComponentResult;

        //Assert
        _mockEmployerTeamOrchestrator.Verify(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUser_TheExpectedAccountIsPassedTotheView()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimTypes.Role, ControllerConstants.Tier2UserClaim));
        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(_orchestratorAccountSummaryResponse);

        //Act
        var result = await _viewComponentContext.InvokeAsync(_hashedAccountId) as ViewViewComponentResult;
        var model = result.ViewData.Model as SupportUserBannerViewModel;

        //Assert
        _account.Should().BeEquivalentTo(model.Account);
    }

    [Test]
    public async Task ThenForAuthenticatedSupportUserAndTheAccountOrchestratorErrors_SupportUserBannerViewIsReturnedWithANullAccount()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimTypes.Role, ControllerConstants.Tier2UserClaim));

        _mockEmployerTeamOrchestrator
            .Setup(m => m.GetAccountSummary(_hashedAccountId, It.IsAny<string>()))
            .ReturnsAsync(new OrchestratorResponse<AccountSummaryViewModel> { Status = HttpStatusCode.BadRequest });

        //Act
        var result = await _viewComponentContext.InvokeAsync(_hashedAccountId) as ViewViewComponentResult;
        var model = result.ViewData.Model as SupportUserBannerViewModel;

        //Assert
        model.Account.Should().BeNull();
    }
}