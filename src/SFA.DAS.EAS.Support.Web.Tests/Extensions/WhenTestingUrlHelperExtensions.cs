using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Extensions;

namespace SFA.DAS.EAS.Support.Web.Tests.Extensions
{
    [TestFixture]
    public class WhenTestingUrlHelperExtensions
    {
        private WebConfiguration _configuration;
        private Mock<IDependencyResolver> _dependancyResolver;
        protected AccountController Controller;
        private UrlHelper _urlHelper;

        [SetUp]
        public void Setup()
        {
            _configuration = new WebConfiguration
            {
                EmployerAccountsConfiguration = new EmployerAccountsConfiguration
                {
                    EmployerAccountsBaseUrl = "https://localhost:44344"
                }
            };
            _dependancyResolver=new Mock<IDependencyResolver>();
            _dependancyResolver.Setup(r => r.GetService(typeof(IWebConfiguration))).Returns(_configuration);
            DependencyResolver.SetResolver(_dependancyResolver.Object);

            var routes = new RouteCollection();
            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            var routeData = new RouteData();

            _urlHelper = new UrlHelper(new RequestContext(context.Object, routeData), routes);
        }

        [Test]
        public void StaffLoginAction_MustReturn_ExpectedRouteUrl()
        {
            
            var expected = "https://localhost:44344/login/staff?HashedAccountId=A1B2C3";
            var actual = UrlHelperExtensions.StaffLoginAction(_urlHelper, "login/staff?HashedAccountId=A1B2C3");
            Assert.AreEqual(expected, actual);
        }
    }
}
