using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers
{
    public abstract class ControllerTestBase
    {
        protected Mock<HttpRequestBase> _httpRequest;
        protected Mock<HttpContextBase> _httpContext;
        protected Mock<ControllerContext> _controllerContext;
        private Mock<HttpResponseBase> _httpResponse;
        protected RouteCollection _routes;
        protected Mock<ILog> Logger;
        protected Mock<IMediator> Mediator;

        public virtual void Arrange(string redirectUrl = "http://localhost/testpost")
        {

            Logger = new Mock<ILog>();
            Mediator = new Mock<IMediator>();

            _routes = new RouteCollection();
            
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

        public T Invoke<T>(Expression<Func<T>> exp) where T : ActionResult
        {
            var methodCall = (MethodCallExpression)exp.Body;
            var method = methodCall.Method;
            var memberExpression = (MemberExpression)methodCall.Object;

            var getCallerExpression = Expression.Lambda<Func<object>>(memberExpression);
            var getCaller = getCallerExpression.Compile();
            var ctrlr = (Controller)getCaller();

            var controllerDescriptor = new ReflectedControllerDescriptor(ctrlr.GetType());
            var actionDescriptor = new ReflectedActionDescriptor(method, method.Name, controllerDescriptor);

            // OnActionExecuting

            ctrlr.ControllerContext = _controllerContext.Object;
            var actionExecutingContext = new ActionExecutingContext(ctrlr.ControllerContext, actionDescriptor, new Dictionary<string, object>());
            var onActionExecuting = ctrlr.GetType().GetMethod("OnActionExecuting", BindingFlags.Instance | BindingFlags.NonPublic);
            onActionExecuting.Invoke(ctrlr, new object[] { actionExecutingContext });
            var actionResult = actionExecutingContext.Result;

            if (actionResult != null)
                return (T)actionResult;

            // call controller method

            var result = exp.Compile()();

            // OnActionExecuted
            var ctx2 = new ActionExecutedContext(ctrlr.ControllerContext,actionDescriptor, false, null)
            { Result = result };
            MethodInfo onActionExecuted = ctrlr.GetType().GetMethod("OnActionExecuted", BindingFlags.Instance | BindingFlags.NonPublic);
            onActionExecuted.Invoke(ctrlr, new object[] { ctx2 });

            return (T)ctx2.Result;
        }
    }
}
