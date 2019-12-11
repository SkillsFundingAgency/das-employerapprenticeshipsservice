﻿using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Collections.Generic;
using System.Security.Claims;
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
        private Mock<HttpContextBase> MockContextBase;            
        private Mock<IPrincipal> MockIPrincipal;
        private Mock<ClaimsIdentity> MockClaimsIdentity;
        private const string Tier2User = "Tier2User";
        private const string HashedAccountId = "HashedAccountId";
        private readonly List<Claim> claims = new List<Claim>();

        [SetUp]
        public void Arrange()
        {
            claims.Add(new Claim(RouteValueKeys.AccountHashedId, HashedAccountId));
            MockIPrincipal = new Mock<IPrincipal>();           
            MockViewDataContainer = new Mock<IViewDataContainer>();
            MockContextBase = new Mock<HttpContextBase>();
            MockClaimsIdentity = new Mock<ClaimsIdentity>();
            MockClaimsIdentity.Setup(m => m.Claims).Returns(claims);
            MockIPrincipal.Setup(m => m.Identity).Returns(MockClaimsIdentity.Object);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            MockContextBase.Setup(c => c.User).Returns(MockIPrincipal.Object);
            MockViewContext = new Mock<ViewContext>();
            MockViewContext.Setup(x => x.HttpContext).Returns(MockContextBase.Object);
        }


        [TestCase(false, null, "Back")]
        [TestCase(true, null, "Back")]
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

        [Test]
        public void TestTier2UserWithNoAccountIdSetAndNoClaimSet()
        {
            //Arrange
            MockClaimsIdentity.Setup(m => m.Claims).Returns(claims);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageLinkText(htmlHelper, "12345");

            //Assert
            Assert.IsNotNull("Back", result);
        }

        [TestCase(false, null, "/")]
        [TestCase(true, "12345", "/accounts/12345/teams/view")]
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

        [Test]
        public void WhenTheUserIsTier2UserAndAccountIdandClaimsNotSet_ThenReturnLink()
        {
            //Arrange
            MockClaimsIdentity.Setup(m => m.Claims).Returns(claims);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageLinkHref(htmlHelper, null);

            //Assert
            Assert.IsNotNull("/", result);
        }

        [TestCase(false, null, "Go back to the service home page")]
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


        [Test]
        public void WhenTheUserIsTier2UserAndAccountIdandClaimsNotSet_ThenReturnButtonText()
        {
            //Arrange
            MockClaimsIdentity.Setup(m => m.Claims).Returns(claims);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageButtonText(htmlHelper, null);

            //Assert
            Assert.IsNotNull("Go back to the service home page", result);
        }


        [TestCase(false, null, "/")]
        [TestCase(true, "12345", "/accounts/12345/teams/view")]
        [TestCase(false, "12345", "/accounts/12345/teams")]
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


        [Test]
        public void WhenTheUserIsTier2UserAndAccountIdandClaimsNotSet_ThenReturnButtonHref()
        {
            //Arrange
            MockClaimsIdentity.Setup(m => m.Claims).Returns(claims);
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(true);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnToHomePageButtonHref(htmlHelper, null);

            //Assert
            Assert.IsNotNull("/", result);
        }

        [TestCase(true, "You do not have permission to access this part of the service.")]
        [TestCase(false, "If you are experiencing difficulty accessing the area of the site you need, first contact an/the account owner to ensure you have the correct role assigned to your account.")]
        public void ReturnParagraphContent_WhenTheUserIsTier2OrTier1_ThenContentOfTheParagraph(bool isTier2User, string expectedContent)
        {
            //Arrange
            MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
            var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);

            //Act
            var result = Helpers.HtmlHelperExtensions.ReturnParagraphContent(htmlHelper);

            //Assert
            Assert.AreEqual(expectedContent, result);
        }

        [Test]
        public void TestSecurityExtension()
        {
            //Act
            //MockIPrincipal
            var result = SecurityExtensions.HashedAccountId(MockClaimsIdentity.Object);

            //Assert
            Assert.IsNotNull(result);

        }


        //[TestCase("G6M7RV")]
        //[TestCase("")]
        //public void GetContextAccountId_WhenAccountIdIsNull_ThenGetAccountIdFromHttpCntext(string accountId)
        //{
        //    //Arrange
        //    string url = $"http://tempuri.org/accounts/{accountId}";
        //    HttpContext.Current = new HttpContext(
        //                         new HttpRequest("", url, ""),
        //                         new HttpResponse(new StringWriter()));
        //    //Act
        //    var result = Helpers.HtmlHelperExtensions.GetContextAccountId();

        //    //Assert
        //    Assert.AreEqual(result, accountId);
        //}


        //[TestCase("G6M7RV")]
        //[TestCase("")]
        //public void GetClaimsHashedAccountId_WhenAccountIdIsNull_ThenGetHashedAccountIdFromClaims(string actualHashedAccountId)
        //{
        //    //Arrange           
        //   var claimsIdentity = new ClaimsIdentity(new[]
        //   {
        //        new Claim(HashedAccountId, actualHashedAccountId)                
        //   });
        //   //HttpContext.Current.User = new ClaimsPrincipal(claimsIdentity);
        //   // MockIPrincipal.Setup(x => x.IsInRole(Tier2User)).Returns(isTier2User);
        //   var htmlHelper = new HtmlHelper(MockViewContext.Object, MockViewDataContainer.Object);
        //    htmlHelper.ViewContext.RequestContext.HttpContext.User = new ClaimsPrincipal(claimsIdentity);

        //    //Act
        //    var result = Helpers.HtmlHelperExtensions.GetClaimsHashedAccountId(htmlHelper);

        //   //Assert
        //   Assert.AreEqual(result, actualHashedAccountId);
        //}
    }
}
