using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Helpers;

[TestFixture]
public class AccessDeniedViewRenderButtonTestsTier1
{
    private Mock<ClaimsPrincipal> _mockIPrincipal;
    private Mock<ClaimsIdentity> _mockClaimsIdentity;
    private const string Tier1User = "Tier1User";
    private const string HashedAccountId = "HashedAccountId";
    private List<Claim> _claims;
    private readonly string _supportConsoleUsers = "Tier1User,Tier2User";
    private EmployerAccountsConfiguration _config;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;


    [SetUp]
    public void Arrange()
    {
        _claims = new List<Claim>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        _config = new EmployerAccountsConfiguration()
        {
            SupportConsoleUsers = _supportConsoleUsers
        };

        _claims.Add(new Claim(RouteValueKeys.HashedAccountId, HashedAccountId));
        _mockIPrincipal = new Mock<ClaimsPrincipal>();
        _mockClaimsIdentity = new Mock<ClaimsIdentity>();
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockIPrincipal.Setup(m => m.Identity).Returns(_mockClaimsIdentity.Object);
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(true);

        _mockHttpContext.Setup(x => x.User).Returns(_mockIPrincipal.Object);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);
    }

    [TestCase(false, null, "Back")]
    [TestCase(true, null, "Back")]
    [TestCase(false, "12345", "Back to the homepage")]
    public void RenderReturnToHomePageLinkText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnLinkText(bool isTier1User,
        string accountId, string expectedText)
    {
        //Arrange
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(isTier1User);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageLinkText(accountId);

        //Assert                       
        Assert.AreEqual(expectedText, result);
    }

    [Test]
    public void TestTier1UserWithNoAccountIdSetAndNoClaimSet()
    {
        //Arrange
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(true);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageLinkText("12345");

        //Assert
        Assert.IsNotNull("Back", result);
    }

    [TestCase(false, null, "/")]
    [TestCase(true, "12345", "/accounts/12345/teams/view")]
    [TestCase(false, "12345", "/")]
    public void RenderReturnToHomePageLinkText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnLink(bool isTier1User,
        string accountId, string expectedLink)
    {
        //Arrange            
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(isTier1User);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageLinkHref(accountId);

        //Assert                       
        Assert.AreEqual(expectedLink, result);
    }

    [Test]
    public void WhenTheUserIsTier1UserAndAccountIdandClaimsNotSet_ThenReturnLink()
    {
        //Arrange
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(true);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageLinkHref(null);

        //Assert
        Assert.IsNotNull("/", result);
    }

    [TestCase(false, null, "Go back to the service home page")]
    [TestCase(true, "12345", "Return to your team")]
    [TestCase(false, "12345", "Go back to the account home page")]
    public void ReturnToHomePageButtonText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnButtonText(bool isTier1User,
        string accountId, string expectedText)
    {
        //Arrange            
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(isTier1User);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageButtonText(accountId);

        //Assert                       
        Assert.AreEqual(expectedText, result);
    }


    [Test]
    public void WhenTheUserIsTier1UserAndAccountIdandClaimsNotSet_ThenReturnButtonText()
    {
        //Arrange
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(true);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageButtonText(null);

        //Assert
        Assert.IsNotNull("Go back to the service home page", result);
    }


    [TestCase(false, null, "/")]
    [TestCase(true, "12345", "/accounts/12345/teams/view")]
    [TestCase(false, "12345", "/accounts/12345/teams")]
    public void ReturnToHomePageButtonHreft_WhenTheUserRoleAndAccountIdHasValues_ThenReturnButtonHref(bool isTier1User,
        string accountId, string expectedLink)
    {
        //Arrange          
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(isTier1User);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageButtonHref(accountId);

        //Assert                       
        Assert.AreEqual(expectedLink, result);
    }


    [Test]
    public void WhenTheUserIsTier1UserAndAccountIdandClaimsNotSet_ThenReturnButtonHref()
    {
        //Arrange
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(true);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnToHomePageButtonHref(null);

        //Assert
        Assert.IsNotNull("/", result);
    }

    [TestCase(true, "You do not have permission to access this part of the service.")]
    [TestCase(false, "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.")]
    public void ReturnParagraphContent_WhenTheUserIsTier1_ThenContentOfTheParagraph(bool isTier1User, string expectedContent)
    {
        //Arrange
        _mockIPrincipal.Setup(x => x.IsInRole(Tier1User)).Returns(isTier1User);
        var htmlHelper = new HtmlHelpers(_config,
            Mock.Of<IMediator>(),
            _mockHttpContextAccessor.Object,
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<ICompositeViewEngine>()
        );

        //Act
        var result = htmlHelper.ReturnParagraphContent();

        //Assert
        Assert.AreEqual(expectedContent, result);
    }

    [Test]
    public void TestSecurityExtension()
    {
        //Act
        //MockIPrincipal
        var result = _mockClaimsIdentity.Object.HashedAccountId();

        //Assert
        Assert.IsNotNull(result);

    }
}