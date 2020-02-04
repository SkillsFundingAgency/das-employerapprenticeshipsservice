using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.Mvc.Filters;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Filters
{
    [TestFixture]
    [Parallelizable]
    public class DasEmployerAccountsAuthorizationFilterTests
    {
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public Mock<ActionDescriptor> mockActionDescriptor { get; set; }
        public AuthorizationFilter AuthorizationFilter { get; set; }
        public Mock<IAuthorizationService> mockAuthorizationService { get; set; }
        public string[] ActionOptions { get; set; }
        public string[] ControllerOptions { get; set; }
        private const string Tier2User = "Tier2User";
        private const string HashedAccountId = "HashedAccountId";
        private readonly Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
        private readonly Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
        public RouteData RouteData { get; set; }


        [SetUp]
        public void Arrange()
        {
            mockActionDescriptor = new Mock<ActionDescriptor>();
            ActionExecutingContext = new ActionExecutingContext { ActionDescriptor = mockActionDescriptor.Object };
            mockAuthorizationService = new Mock<IAuthorizationService>();
            ActionOptions = new string[0];
            ControllerOptions = new string[0];

            mockActionDescriptor.Setup(d => d.ControllerDescriptor.ControllerName).Returns(Guid.NewGuid().ToString());
            mockActionDescriptor.Setup(d => d.ControllerDescriptor.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { });
            mockActionDescriptor.Setup(d => d.ActionName).Returns(Guid.NewGuid().ToString());
            mockActionDescriptor.Setup(d => d.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { });

            AuthorizationFilter = new AuthorizationFilter(() => mockAuthorizationService.Object);
            ActionOptions = new[] { "Action.Option" };
            mockActionDescriptor.Setup(d => d.GetCustomAttributes(typeof(DasAuthorizeAttribute), true)).Returns(new object[] { new DasAuthorizeAttribute(ActionOptions) });
            mockContext.Setup(htx => htx.Request).Returns(mockRequest.Object);
            mockContext.Setup(htx => htx.Response).Returns(mockResponse.Object);

        }

        [Test]
        public void OnActionExecuting_WhenActionIsExecutingAndControllerIsDecoratedWithDasAuthorizeAttributeAndControllerOptionsAreNotAuthorized_ThenReturnStatusCodeForbidden()
        {
            //Arrange
           


            //Act
            AuthorizationFilter.OnActionExecuting(ActionExecutingContext);

            //Assert
            var httpStatusCodeResult = ActionExecutingContext.Result as HttpStatusCodeResult;
            Assert.That(httpStatusCodeResult, Is.Not.Null);
            Assert.AreEqual(httpStatusCodeResult.StatusCode, (int)HttpStatusCode.Forbidden);
        }


        [Test]
        public void OnActionExecuting_WhenActionIsExecutingAndControllerIsDecoratedWithDasAuthorizeAttributeAndControllerOptionsAreNotAuthorized_ThenReturnAccessDenied()
        {
            //Arrange           
            AuthorizationFilter = new DasEmployerAccountsAuthorizationFilter(() => mockAuthorizationService.Object);          
            mockContext.Setup(x => x.User.IsInRole(Tier2User)).Returns(true);
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
