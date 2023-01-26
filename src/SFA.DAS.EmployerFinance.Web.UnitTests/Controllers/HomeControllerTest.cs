using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Controllers;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private EmployerFinanceConfiguration _configuration;
        private Mock<IDependencyResolver> _dependancyResolver;
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerFinanceConfiguration
            {
                EmployerPortalBaseUrl = "https://localhost"
            };

            _dependancyResolver = new Mock<IDependencyResolver>();
            _dependancyResolver.Setup(r => r.GetService(typeof(EmployerFinanceConfiguration))).Returns(_configuration);

            DependencyResolver.SetResolver(_dependancyResolver.Object);

            _homeController =
                new HomeController(Mock.Of<IAuthenticationService>(), Mock.Of<EmployerFinanceConfiguration>())
                {
                    Url = new UrlHelper()
                };
        }

        [Test]
        public void IndexRedirectsToPortalSite()
        {
            // Act
            var result = _homeController.Index() as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_configuration.EmployerPortalBaseUrl, result.Url);
        }
    }
}
