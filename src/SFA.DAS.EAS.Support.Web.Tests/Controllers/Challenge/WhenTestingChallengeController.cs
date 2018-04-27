using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.Web.Controllers;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge
{
    public abstract class WhenTestingChallengeController
    {
        protected Mock<IChallengeHandler> MockChallengeHandler;
        protected Mock<HttpContextBase> MockContextBase;
        protected Mock<HttpRequestBase> MockRequestBase;
        protected Mock<HttpResponseBase> MockResponseBase;
        protected Mock<IPrincipal> MockUser;
        protected RouteData RouteData;
        protected ChallengeController Unit;
        protected ControllerContext UnitControllerContext;

        [SetUp]
        public void Setup()
        {
            MockChallengeHandler = new Mock<IChallengeHandler>();
            Unit = new ChallengeController(MockChallengeHandler.Object);

            RouteData = new RouteData();
            MockContextBase = new Mock<HttpContextBase>();

            MockRequestBase = new Mock<HttpRequestBase>();
            MockResponseBase = new Mock<HttpResponseBase>();
            MockUser = new Mock<IPrincipal>();

            MockContextBase.Setup(x => x.Request).Returns(MockRequestBase.Object);
            MockContextBase.Setup(x => x.Response).Returns(MockResponseBase.Object);
            MockContextBase.Setup(x => x.User).Returns(MockUser.Object);
            UnitControllerContext = new ControllerContext(MockContextBase.Object, RouteData, Unit);

            Unit.ControllerContext = UnitControllerContext;
        }
    }
}