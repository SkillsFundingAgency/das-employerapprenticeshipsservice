using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Controllers;
using Microsoft.AspNetCore.Routing;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private HomeController _homeController;
        private Mock<ControllerContext> _mockControllerContext;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<HttpRequest> _mockHttpRequest;
        private Mock<RequestContext> _mockRequestContext;
        private QueryCollection _queryString;
        private EmployerApprenticeshipsServiceConfiguration _config;
        private string gaValue;

        [SetUp]
        public void Arrange()
        {
            gaValue = Guid.NewGuid().ToString();
            _mockControllerContext = new Mock<ControllerContext>();
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockRequestContext = new Mock<RequestContext>();
            _queryString = new QueryCollection(new Dictionary<string, StringValues>() { { "_ga", gaValue } });
            _config = new EmployerApprenticeshipsServiceConfiguration();

            _mockHttpContext
                .Setup(m => m.Request)
                .Returns(_mockHttpRequest.Object);

            _mockHttpRequest
                .Setup(m => m.Query)
                .Returns(_queryString);

            _mockControllerContext.Object.HttpContext = _mockHttpContext.Object;

            _homeController = new HomeController(_config)
            {
                ControllerContext = _mockControllerContext.Object,
                Url = new UrlHelper(new ActionContext(_mockHttpContext.Object, new RouteData(), new ActionDescriptor()))
            };

            _config.EmployerAccountsBaseUrl = @"http://localhost";
        }

        [Test]
        public void ThenTheGoogleTagQueryStringIsPreservedWhenRedirecting()
        {
            //Act
            var result = new Uri((_homeController.Index() as RedirectResult).Url);

            //Assert
            Assert.IsTrue(result.Query.Contains($"_ga={gaValue}"));
        }
    }
}