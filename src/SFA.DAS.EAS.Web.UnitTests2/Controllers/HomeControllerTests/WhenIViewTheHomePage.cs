/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Controllers;
using System;
using System.Collections.Specialized;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private HomeController _homeController;
        private Mock<ControllerContext> _mockControllerContext;
        private Mock<HttpContext> _mockHttpContextBase;
        private Mock<HttpRequest> _mockHttpRequestBase;
        private Mock<RequestContext> _mockRequestContext;
        private Mock<IDependencyResolver> _mockDependencyResolver;
        private NameValueCollection _queryString;
        private EmployerApprenticeshipsServiceConfiguration _config;

        [SetUp]
        public void Arrange()
        {
            _mockControllerContext = new Mock<ControllerContext>();
            _mockHttpContextBase = new Mock<HttpContext>();
            _mockHttpRequestBase = new Mock<HttpRequest>();
            _mockRequestContext = new Mock<RequestContext>();
            _mockDependencyResolver = new Mock<IDependencyResolver>();
            _queryString = new NameValueCollection();
            _config = new EmployerApprenticeshipsServiceConfiguration();

            _mockHttpContextBase
                .Setup(m => m.Request)
                .Returns(_mockHttpRequestBase.Object);

            _mockHttpRequestBase
                .Setup(m => m.QueryString)
                .Returns(_queryString);

            _mockControllerContext
                .Setup(m => m.HttpContext)
                .Returns(_mockHttpContextBase.Object);

            _homeController = new HomeController
            {
                ControllerContext = _mockControllerContext.Object,
                Url = new UrlHelper(new RequestContext(_mockHttpContextBase.Object, new RouteData()))
            };
            _homeController.Url.RequestContext.HttpContext = _mockHttpContextBase.Object;

            DependencyResolver.SetResolver(_mockDependencyResolver.Object);

            _mockDependencyResolver
                .Setup(m => m.GetService(It.IsAny<Type>()))
                .Returns<Type>(t =>
                {
                    if (t.IsAssignableFrom(typeof(EmployerApprenticeshipsServiceConfiguration)))
                    {
                        return _config;
                    }

                    return null;
                });

            _config.EmployerAccountsBaseUrl = @"http://localhost";
        }

        [Test]
        public void ThenTheGoogleTagQueryStringIsPreservedWhenRedirecting()
        {
            // Arrange
            var gaValue = Guid.NewGuid().ToString();
            _queryString.Add("_ga", gaValue);

            //Act
            var result = new Uri((_homeController.Index() as RedirectResult).Url);

            //Assert
            Assert.IsTrue(result.Query.Contains($"_ga={gaValue}"));
        }
    }
}*/