using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.Web.Controllers;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
    public abstract class WhenTestingChallengeController
    {
        protected Mock<IChallengeHandler> MockChallengeHandler;
        protected Mock<HttpContext> MockContextBase;
        protected Mock<HttpRequest> MockRequestBase;
        protected Mock<HttpResponse> MockResponseBase;
        protected Mock<ClaimsPrincipal> MockUser;
        protected Mock<ILogger<ChallengeController>> _logger;
        protected RouteData RouteData;
        protected ChallengeController Unit;
        protected ControllerContext UnitControllerContext;

        [SetUp]
        public void Setup()
        {
            MockChallengeHandler = new Mock<IChallengeHandler>();
            _logger = new Mock<ILogger<ChallengeController>>();
            Unit = new ChallengeController(MockChallengeHandler.Object, _logger.Object);

            RouteData = new RouteData();
            MockContextBase = new Mock<HttpContext>();

            MockRequestBase = new Mock<HttpRequest>();
            MockResponseBase = new Mock<HttpResponse>();
            MockUser = new Mock<ClaimsPrincipal>();

            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
            MockContextBase.Setup(x => x.User).Returns(MockUser.Object);

            UnitControllerContext = new ControllerContext();

            Unit.ControllerContext = UnitControllerContext;
        }
    }
}