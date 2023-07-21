using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Claim = System.Security.Claims.Claim;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Helpers;

class HtmlHelpersTests
{
    private Mock<HttpContext> _mockHttpContext;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<ClaimsPrincipal> _mockPrincipal;
    private Mock<ClaimsIdentity> _mockClaimsIdentity;
    private bool _isAuthenticated = true;
    private List<Claim> _claims;
    private string _userId;
    private IHtmlHelpers _htmlHelper;
    private Mock<IMediator> _mockMediator;
    private EmployerAccountsConfiguration _employerConfirguration;
    private readonly string _supportConsoleUsers = "Tier1User,Tier2User";

    [SetUp]
    public void SetUp()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockPrincipal = new Mock<ClaimsPrincipal>();
        _mockClaimsIdentity = new Mock<ClaimsIdentity>();
        _employerConfirguration = new EmployerAccountsConfiguration()
        {
            SupportConsoleUsers = _supportConsoleUsers

        };
        _userId = "TestUser";

        _claims = new List<Claim>
        {
            new Claim(ControllerConstants.UserRefClaimKeyName, _userId)
        };

        _mockPrincipal.Setup(m => m.Identity).Returns(_mockClaimsIdentity.Object);
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockHttpContext.Setup(m => m.User).Returns(_mockPrincipal.Object);
        _mockHttpContextAccessor.Setup(x=> x.HttpContext).Returns(_mockHttpContext.Object);

        _mockMediator = new Mock<IMediator>();

        _htmlHelper = new HtmlHelpers(_employerConfirguration,
            _mockMediator.Object,
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>());

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(provider => provider.GetService(typeof(EmployerAccountsConfiguration)))
            .Returns(_employerConfirguration);

    }

    [Test]
    public void WhenAuthenticatedSupportUser_ShouldReturnTrue()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void WhenUnauthenticatedSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(false); // re-apply the mock return
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void WhenUnauthenticatedNonSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _isAuthenticated = false;

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void WhenAuthenticatedNonSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _isAuthenticated = true;

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [TestCaseSource(nameof(LabelCases))]
    public void WhenICallSetZenDeskLabelsWithLabels_ThenTheKeywordsAreCorrect(string[] labels, string keywords)
    {
        // Arrange
        var expected = $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [{keywords}] }});</script>";

        // Act
        var actual = HtmlHelpers.SetZenDeskLabels(labels).ToString();

        // Assert
        Assert.AreEqual(expected, actual);
    }


    private static readonly object[] LabelCases =
    {
        new object[] { new string[] { "a string with multiple words", "the title of another page" }, "'a string with multiple words','the title of another page'"},
        new object[] { new string[] { "eas-estimate-apprenticeships-you-could-fund" }, "'eas-estimate-apprenticeships-you-could-fund'"},
        new object[] { new string[] { "eas-apostrophe's" }, @"'eas-apostrophe\'s'"},
        new object[] { new string[] { null }, "''" }
    };
}