using System;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Filters
{
    [TestFixture]
    [Parallelizable]
    public class DasEmployerAccountsAuthorizationFilterTests
    {
        private Mock<IAuthenticationService> _mockAuthenticationService;
        public Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext ActionExecutingContext { get; set; }
        public Mock<ActionDescriptor> MockActionDescriptor { get; set; }
        public AuthorizationFilter AuthorizationFilter { get; set; }
        public Mock<IAuthorizationService> MockAuthorizationService { get; set; }
        public string[] ActionOptions { get; set; }
        public string[] ControllerOptions { get; set; }
        private readonly string _supportConsoleUsers = "Tier1User,Tier2User";
        private const string HashedAccountId = "HashedAccountId";
        private readonly Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
        private readonly Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
        private EmployerAccountsConfiguration _config;
        public RouteData RouteData { get; set; }


        [SetUp]
        public void Arrange()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            MockActionDescriptor = new Mock<ActionDescriptor>();
            ActionExecutingContext = new Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext { ActionDescriptor = MockActionDescriptor.Object };
            MockAuthorizationService = new Mock<IAuthorizationService>();
            ActionOptions = new string[0];
            ControllerOptions = new string[0];
            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = _supportConsoleUsers
            };
            MockActionDescriptor.Setup(d => d.ControllerDescriptor.ControllerName).Returns(Guid.NewGuid().ToString());
            MockActionDescriptor.Setup(d => d.ControllerDescriptor.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { });
            MockActionDescriptor.Setup(d => d.ActionName).Returns(Guid.NewGuid().ToString());
            MockActionDescriptor.Setup(d => d.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { });

            AuthorizationFilter = new AuthorizationFilter(() => MockAuthorizationService.Object);
            ActionOptions = new[] { "Action.Option" };
            MockActionDescriptor.Setup(d => d.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { new DasAuthorizeAttribute(ActionOptions) });
            mockContext.Setup(htx => htx.Request).Returns(mockRequest.Object);
            mockContext.Setup(htx => htx.Response).Returns(mockResponse.Object);

        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void OnActionExecuting_WhenActionIsExecutingAndControllerIsDecoratedWithDasAuthorizeAttributeAndControllerOptionsAreNotAuthorized_ThenReturnStatusCodeForbidden(string role)
        {
            //Arrange
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);


            //Act
            AuthorizationFilter.OnActionExecuting(ActionExecutingContext);

            //Assert
            var httpStatusCodeResult = ActionExecutingContext.Result as Microsoft.AspNetCore.Mvc.StatusCodeResult;
            Assert.That(httpStatusCodeResult, Is.Not.Null);
            Assert.AreEqual(httpStatusCodeResult.StatusCode, (int)HttpStatusCode.Forbidden);
        }


        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public void OnActionExecuting_WhenActionIsExecutingAndControllerIsDecoratedWithDasAuthorizeAttributeAndControllerOptionsAreNotAuthorized_ThenReturnAccessDenied(string role)
        {
            //Arrange           
            AuthorizationFilter = new DasEmployerAccountsAuthorizationFilter(() =>
                MockAuthorizationService.Object, _config);
            _mockAuthenticationService.Setup(m => m.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role)).Returns(true);
            mockContext.Setup(x => x.User.IsInRole(role)).Returns(true);
            RouteData = new RouteData();
            RouteData.Values.Add(RouteValueKeys.AccountHashedId, HashedAccountId);
            mockContext.Setup(x => x.Request.RequestContext.RouteData).Returns(RouteData);
            ActionExecutingContext.HttpContext = mockContext.Object;

            //Act
            AuthorizationFilter.OnActionExecuting(ActionExecutingContext);

            //Assert
            var redirectToRouteResult = ActionExecutingContext.Result as RedirectToRouteResult;
            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["controller"], Is.EqualTo("Error"));
            Assert.That(redirectToRouteResult.RouteValues["action"], Is.EqualTo($"accessdenied/{HashedAccountId}"));
        }

    }
}
