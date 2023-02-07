using MediatR;
using Moq;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.EmployerUsers.WebClientComponents;
using ActionExecutedContext = Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext;
using ActionExecutingContext = Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;
using SFA.DAS.EmployerAccounts.Web.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers
{
    public abstract class ControllerTestBase
    {
        protected Mock<HttpRequest> HttpRequest = new();
        protected Mock<HttpContext> HttpContext = new();
        protected ControllerContext ControllerContext;
        protected Mock<HttpResponse> HttpResponse = new();
        protected RouteData Routes;
        protected Mock<ILog> Logger;
        protected Mock<IMediator> Mediator;
        
        public virtual void Arrange(string redirectUrl = "http://localhost/testpost")
        {
            Logger = new Mock<ILog>();
            Mediator = new Mock<IMediator>();

            Routes = new RouteData();
            
            HttpContext.Setup(x => x.Request.Host).Returns(new HostString("test.local"));
            HttpContext.Setup(x => x.Request.Scheme).Returns("http");
            HttpContext.Setup(x => x.Request.PathBase).Returns("/");
            HttpContext.Setup(x => x.Connection.RemoteIpAddress).Returns(IPAddress.Parse("123.123.123.123"));
            HttpContext.Setup(c => c.Request).Returns(HttpRequest.Object);
            HttpContext.Setup(c => c.Response).Returns(HttpResponse.Object);

            //_httpRequest = new Mock<HttpRequestBase>();
            //_httpRequest.Setup(r => r.UserHostAddress).Returns("123.123.123.123");
            // _httpRequest.Setup(r => r.Url).Returns(new Uri("http://test.local", UriKind.Absolute));
            //_httpRequest.Setup(r => r.ApplicationPath).Returns("/");

            //_httpRequest.Setup(r => r.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            //_httpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);
            //HttpResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns("http://localhost/testpost");

            //_httpContext = new Mock<HttpContextBase>();


            ControllerContext = new ControllerContext()
            {
                HttpContext = HttpContext.Object,
                RouteData = Routes,
            };
            //ControllerContext.Setup(c => c.HttpContext).Returns(HttpContextAccessor.Object.HttpContext);
            //ControllerContext.Setup(c => c.HttpContext).Returns(new ControllerContext(HttpContextAccessor.Object.HttpContext));
        }

        protected void AddUserToContext(string id = "USER_ID", string email = "my@local.com", string name = "test name")
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("sub", id),
            });
            var principal = new ClaimsPrincipal(identity);
            HttpContext.Setup(c => c.User).Returns(principal);
        }

        protected void AddUserToContext(string id, string email, string givenName, string familyName)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, $"{givenName} {familyName}"),
                new Claim(ClaimTypes.Email, email),
                new Claim(ControllerConstants.EmailClaimKeyName, email),
                new Claim(DasClaimTypes.GivenName, givenName),
                new Claim(DasClaimTypes.FamilyName, familyName),
                new Claim(ControllerConstants.UserRefClaimKeyName, id),
                new Claim("sub", id),
            });
            var principal = new ClaimsPrincipal(identity);
            HttpContext.Setup(c => c.User).Returns(principal);
        }

        //public T Invoke<T>(Expression<Func<T>> exp) where T : ActionResult
        //{
        //    var methodCall = (MethodCallExpression)exp.Body;
        //    var method = methodCall.Method;
        //    var memberExpression = (MemberExpression)methodCall.Object;

        //    var getCallerExpression = Expression.Lambda<Func<object>>(memberExpression);
        //    var getCaller = getCallerExpression.Compile();
        //    var ctrlr = (Controller)getCaller();

        //    var controllerDescriptor = new ReflectedControllerDescriptor(ctrlr.GetType());
        //    var actionDescriptor = new ReflectedActionDescriptor(method, method.Name, controllerDescriptor);

        //    // OnActionExecuting

        //    ctrlr.ControllerContext = ControllerContext.Object;
        //    var actionExecutingContext = new ActionExecutingContext(ctrlr.ControllerContext, actionDescriptor, new Dictionary<string, object>());
        //    var onActionExecuting = ctrlr.GetType().GetMethod("OnActionExecuting", BindingFlags.Instance | BindingFlags.NonPublic);
        //    onActionExecuting.Invoke(ctrlr, new object[] { actionExecutingContext });
        //    var actionResult = actionExecutingContext.Result;

        //    if (actionResult != null)
        //        return (T)actionResult;

        //    // call controller method

        //    var result = exp.Compile()();

        //    // OnActionExecuted
        //    var ctx2 = new ActionExecutedContext(ctrlr.ControllerContext, actionDescriptor, false, null)
        //    { Result = result };
        //    MethodInfo onActionExecuted = ctrlr.GetType().GetMethod("OnActionExecuted", BindingFlags.Instance | BindingFlags.NonPublic);
        //    onActionExecuted.Invoke(ctrlr, new object[] { ctx2 });

        //    return (T)ctx2.Result;
        //}
    }
}
