using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class AccessDeniedViewRenderButtonTests
    {
        private Mock<IViewDataContainer> MockViewDataContainer;
        private Mock<ViewContext> MockViewContext;        
        private  Mock<HttpContextBase> MockContextBase;
        private const string Tier2User = "Tier2User";

        [SetUp]
        public void Arrange()
        {
            MockViewDataContainer = new Mock<IViewDataContainer>();
            MockContextBase = new Mock<HttpContextBase>();
            var claimsIdentity = new ClaimsIdentity(new[]            {

                new Claim("sub", "UserRef"),
            });
            claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, Tier2User));
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);          
        }


        [Test]
        public void RenderReturnToHomePageButton_WhenUserInRoleIsTier2UserAndAccountIdSet_ThenRenderLinkToReturnToTeamViewPage()
        {
            //Arrange  
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageButton(htmlHelper, "12345");

            //Assert
            Assert.AreEqual("<a class=\"button\" href=\"accounts/12345/teams/view\">Return to your team </a>", result.ToHtmlString());            
        }


        [Test]
        public void RenderReturnToHomePageButton_WhenUserInRoleIsNotTier2UserAndAccountIdSet_ThenRenderLinkToReturnToTeamPage()
        {
            //Arrange            
            var claimsIdentity = new ClaimsIdentity(new[]            {

                new Claim("sub", "UserRef"),
            });            
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageButton(htmlHelper, "12345");

            //Assert
            Assert.AreEqual("<a class=\"button\" href=\"accounts/12345/teams\">Go back to the account home page</a>", result.ToHtmlString());
        }

        [Test]
        public void RenderReturnToHomePageButton_WhenUserInRoleIsNotTier2UserAndAccountIdNotSet_ThenRenderLinkToReturnToServicePage()
        {
            //Arrange  
            MockViewDataContainer = new Mock<IViewDataContainer>();
            MockContextBase = new Mock<HttpContextBase>();
            var claimsIdentity = new ClaimsIdentity(new[]            {

                new Claim("sub", "UserRef"),
            });
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageButton(htmlHelper, null);

            //Assert
            Assert.AreEqual("<a class=\"button\" href=\"/\">Go back to the service home page</a>", result.ToHtmlString());
        }
        

        [Test]
        public void RenderReturnToHomePageLinkForBreadcrumbSection_WhenUserInRoleIsTier2UserAndAccountIdSet_ThenRenderLinkToReturnToTeamViewPage()
        { 
            //Arrange             
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageLinkForBreadcrumbSection(htmlHelper, "12345");

            //Assert                       
            Assert.AreEqual("<a href=\"accounts/12345/teams/view\" class=\"back - link\">Return to your team</a>", result.ToHtmlString());
        }


        [Test]
        public void RenderReturnToHomePageLinkForBreadcrumbSection_WhenUserInRoleIsNotTier2UserAndAccountIdSet_ThenRenderLinkToReturnToHomePage()
        {
            //Arrange           
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim("sub", "UserRef") });            
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageLinkForBreadcrumbSection(htmlHelper, "12345");

            //Assert                       
            Assert.AreEqual("<a href=\"/\" class=\"back - link\">Back to the homepage</a>", result.ToHtmlString());
        }
        

        [Test]
        public void RenderReturnToHomePageLinkForBreadcrumbSection_WhenUserInRoleIsTier2UserAndAccountIdNotSet_ThenRenderLinkToReturnToHomePage()
        {
            //Arrange            
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageLinkForBreadcrumbSection(htmlHelper, null);

            //Assert                       
            Assert.AreEqual("<a href=\"/\" class=\"back - link\">Back</a>", result.ToHtmlString());
        }


        [Test]
        public void RenderReturnToHomePageLinkForBreadcrumbSection_WhenUserInRoleIsNotTier2UserAndAccountIdIsNotSet_ThenRenderLinkToReturnToHomePage()
        {
            //Arrange            
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim("sub", "UserRef") });            
            var principal = new ClaimsPrincipal(claimsIdentity);
            MockContextBase.Setup(c => c.User).Returns(principal);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.RenderReturnToHomePageLinkForBreadcrumbSection(htmlHelper, null);

            //Assert                       
            Assert.AreEqual("<a href=\"/\" class=\"back - link\">Back</a>", result.ToHtmlString());
        }
    }
}
