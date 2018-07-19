using Moq;
using NUnit.Framework;
using SFA.DAS.Authenication;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Web.Controllers;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(Mock.Of<IAuthenticationService>(), Mock.Of<EmployerFinanceConfiguration>());

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
