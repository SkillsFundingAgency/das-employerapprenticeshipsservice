using Moq;
using NUnit.Framework;
using SFA.DAS.Authenication;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(Mock.Of<IAuthenticationService>(), Mock.Of<EmployerAccountsConfiguration>());

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
