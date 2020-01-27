using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Filters
{
    [TestFixture]
    [Parallelizable]
    public class DasEmployerAccountsUnauthorizedAccessExceptionFilterTests
    {
        public ExceptionContext ExceptionContext { get; set; }
        public RouteData RouteData { get; set; }
        public DasEmployerAccountsUnauthorizedAccessExceptionFilter UnauthorizedAccessExceptionFilter { get; set; }
        public Exception Exception { get; set; }
        private const string HashedAccountId = "HashedAccountId";
        private readonly Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
        private readonly Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
        private EmployerAccountsConfiguration _config;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly string SupportConsoleUsers = "Tier1User,Tier2User";

        [SetUp]
        public void Arrange()
        {
            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = SupportConsoleUsers
            };
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            Exception = new UnauthorizedAccessException();
            ExceptionContext = new ExceptionContext();
            RouteData = new RouteData();
            mockContext.Setup(htx => htx.Request).Returns(mockRequest.Object);
            mockContext.Setup(htx => htx.Response).Returns(mockResponse.Object);
            //mockContext.Setup(x => x.User.IsInRole(Tier2User)).Returns(true);
            UnauthorizedAccessExceptionFilter = new DasEmployerAccountsUnauthorizedAccessExceptionFilter(_config, _mockAuthenticationService.Object);
        }


        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void OnException_WhenAnUnauthorizedAccessExceptionIsThrownForTier2User_ThenReturnToAccessDenied(string role)
        {
            //Arrange            
            ExceptionContext.Exception = Exception;
            RouteData.Values.Add(RouteValueKeys.AccountHashedId, HashedAccountId);
            mockContext.Setup(x => x.Request.RequestContext.RouteData).Returns(RouteData);
            ExceptionContext.HttpContext = mockContext.Object;
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);

            //Act            
            UnauthorizedAccessExceptionFilter.OnException(ExceptionContext);

            //Assert
            var redirectToRouteResult = ExceptionContext.Result as RedirectToRouteResult;
            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["controller"], Is.EqualTo("Error"));
            Assert.That(redirectToRouteResult.RouteValues["action"], Is.EqualTo($"accessdenied/{HashedAccountId}"));
        }


        [Test]
        public void OnException_WhenAnUnauthorizedAccessExceptionIsThrownForTier2UserAndNoHashedAccountIdIsSet_ButClaimValueIsSet_ThenReturnToAccessDenied()
        {
            //Arrange            
            ExceptionContext.Exception = Exception;
            mockContext.Setup(x => x.Request.RequestContext.RouteData).Returns(RouteData);
            ExceptionContext.HttpContext = mockContext.Object;
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(x => x.FindFirst("HashedAccountId")).Returns(new Claim("HashedAccountId", HashedAccountId));

            var cp = new Mock<ClaimsPrincipal>();
            cp.Setup(m => m.IsInRole(Tier2User)).Returns(true);
            cp.Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            cp.Setup(m => m.Identity).Returns(identityMock.Object);
            mockContext.Setup(ctx => ctx.User).Returns(cp.Object);

            //Act            
            UnauthorizedAccessExceptionFilter.OnException(ExceptionContext);

            //Assert
            var redirectToRouteResult = ExceptionContext.Result as RedirectToRouteResult;
            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["controller"], Is.EqualTo("Error"));
            Assert.That(redirectToRouteResult.RouteValues["action"], Is.EqualTo($"accessdenied/{HashedAccountId}"));
        }

        [Test]
        public void OnException_WhenAnUnauthorizedAccessExceptionIsThrownForTier2UserAndNoHashedAccountIdIsSet_AndNoClaimValueIsSet_ThenReturnToAccessDenied()
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void OnException_WhenAnUnauthorizedAccessExceptionIsThrownForTier2UserAndNoHashedAccountIdIsSet_ThenReturnToAccessDenied(string role)
        {
            //Arrange            
            ExceptionContext.Exception = Exception;
            mockContext.Setup(x => x.Request.RequestContext.RouteData).Returns(RouteData);
            ExceptionContext.HttpContext = mockContext.Object;
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);

            //Act            
            UnauthorizedAccessExceptionFilter.OnException(ExceptionContext);

            //Assert
            var redirectToRouteResult = ExceptionContext.Result as RedirectToRouteResult;
            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["controller"], Is.EqualTo("Error"));
            Assert.That(redirectToRouteResult.RouteValues["action"], Is.EqualTo($"accessdenied"));
        }

        [Test]
        public void OnException_WhenAnUnauthorizedAccessExceptionIsThrownForTier2User_ThenExceptionShouldBeHandled()
        {
            //Arrange            
            ExceptionContext.Exception = Exception;
            mockContext.Setup(x => x.Request.RequestContext.RouteData).Returns(RouteData);
            ExceptionContext.HttpContext = mockContext.Object;

            //Act            
            UnauthorizedAccessExceptionFilter.OnException(ExceptionContext);

            //Assert
            Assert.IsTrue(ExceptionContext.ExceptionHandled);
        }
        
    }
}
