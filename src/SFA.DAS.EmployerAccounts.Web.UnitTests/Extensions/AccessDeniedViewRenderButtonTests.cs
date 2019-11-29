using Moq;
using NUnit.Framework;
using System.Security.Principal;
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
        private Mock<IPrincipal> MockIPrincipal;        

        [SetUp]
        public void Arrange()
        {
            MockIPrincipal = new Mock<IPrincipal>();           
            MockViewDataContainer = new Mock<IViewDataContainer>();
            MockContextBase = new Mock<HttpContextBase>();          
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            MockContextBase.Setup(c => c.User).Returns(MockIPrincipal.Object);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);          
        }


        [TestCase(false, null, "Back")]
        [TestCase(true, null, "Back")]
        [TestCase(true, "12345", "Return to your team")]
        [TestCase(false, "12345", "Back to the homepage")]
        public void RenderReturnToHomePageLinkText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnLinkText(bool isTier2User,
            string accountId, string expectedText)
        {
            //Arrange            
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageLinkText(htmlHelper, accountId);

            //Assert                       
            Assert.AreEqual(expectedText, result);
        }

        [TestCase(false, null, "/")]
        [TestCase(true, null, "/")]
        [TestCase(true, "12345", "accounts/12345/teams/view")]
        [TestCase(false, "12345", "/")]
        public void RenderReturnToHomePageLinkText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnLink(bool isTier2User,
           string accountId, string expectedLink)
        {
            //Arrange            
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageLinkHref(htmlHelper, accountId);

            //Assert                       
            Assert.AreEqual(expectedLink, result);
        }


        [TestCase(false, null, "Go back to the service home page")]
        [TestCase(true, null, "Go back to the service home page")]
        [TestCase(true, "12345", "Return to your team")]
        [TestCase(false, "12345", "Go back to the account home page")]
        public void ReturnToHomePageButtonText_WhenTheUserRoleAndAccountIdHasValues_ThenReturnButtonText(bool isTier2User,
            string accountId, string expectedText)
        {
            //Arrange            
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageButtonText(htmlHelper, accountId);

            //Assert                       
            Assert.AreEqual(expectedText, result);
        }


        [TestCase(false, null, "/")]
        [TestCase(true, null, "/")]
        [TestCase(true, "12345", "accounts/12345/teams/view")]
        [TestCase(false, "12345", "accounts/12345/teams")]
        public void ReturnToHomePageButtonHreft_WhenTheUserRoleAndAccountIdHasValues_ThenReturnButtonHref(bool isTier2User,
         string accountId, string expectedLink)
        {
            //Arrange            
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageButtonHref(htmlHelper, accountId);

            //Assert                       
            Assert.AreEqual(expectedLink, result);
        }

    }
}
