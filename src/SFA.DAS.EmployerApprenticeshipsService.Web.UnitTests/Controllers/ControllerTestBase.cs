using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NLog;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers
{
    public abstract class ControllerTestBase
    {
        protected Mock<HttpRequestBase> _httpRequest;
        protected Mock<HttpContextBase> _httpContext;
        protected Mock<ControllerContext> _controllerContext;
        private Mock<HttpResponseBase> _httpResponse;
        protected RouteCollection _routes;
        protected Mock<ILogger> Logger;
        protected Mock<IMediator> Mediator;

        public virtual void Arrange(string redirectUrl = "http://localhost/testpost")
        {

            Logger = new Mock<ILogger>();
            Mediator = new Mock<IMediator>();

            _routes = new RouteCollection();
            RouteConfig.RegisterRoutes(_routes);

            _httpRequest = new Mock<HttpRequestBase>();
            _httpRequest.Setup(r => r.UserHostAddress).Returns("123.123.123.123");
            _httpRequest.Setup(r => r.Url).Returns(new Uri("http://test.local",UriKind.Absolute));
            _httpRequest.Setup(r => r.ApplicationPath).Returns("/");
            _httpRequest.Setup(r => r.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            _httpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            _httpResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns("http://localhost/testpost");

            _httpContext = new Mock<HttpContextBase>();
            _httpContext.Setup(c => c.Request).Returns(_httpRequest.Object);
            _httpContext.Setup(c => c.Response).Returns(_httpResponse.Object);

            _controllerContext = new Mock<ControllerContext>();
            _controllerContext.Setup(c => c.HttpContext).Returns(_httpContext.Object);
        }

        protected void AddUserToContext(string id = "USER_ID", string email = "my@local.com", string name="test name")
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("sub", id),
            });
            var principal = new ClaimsPrincipal(identity);
            _httpContext.Setup(c => c.User).Returns(principal);
        }
    }
}
