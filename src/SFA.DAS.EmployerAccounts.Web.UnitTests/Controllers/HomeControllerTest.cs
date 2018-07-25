using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Controllers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
