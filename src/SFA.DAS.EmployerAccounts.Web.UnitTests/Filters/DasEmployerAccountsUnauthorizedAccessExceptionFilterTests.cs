using System;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
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
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly string _supportConsoleUsers = "Tier1User,Tier2User";
        private IUserContext _userContext;
        private EmployerAccountsConfiguration _config;
        private const string Tier2User = "Tier2User";


        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            Exception = new UnauthorizedAccessException();
            ExceptionContext = new ExceptionContext();
            RouteData = new RouteData();
            mockContext.Setup(htx => htx.Request).Returns(mockRequest.Object);
            mockContext.Setup(htx => htx.Response).Returns(mockResponse.Object);
            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = _supportConsoleUsers
            };
            _userContext = new UserContext(_mockAuthenticationService.Object, _config);
            UnauthorizedAccessExceptionFilter = new DasEmployerAccountsUnauthorizedAccessExceptionFilter(_userContext);
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

            var cp = new Mock<ClaimsPrincipal>();
            cp.Setup(m => m.IsInRole(Tier2User)).Returns(true);
            cp.Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            cp.Setup(m => m.Identity).Returns(identityMock.Object);
            mockContext.Setup(ctx => ctx.User).Returns(cp.Object);
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, Tier2User)).Returns(true);

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
