using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFA.DAS.EmployerAccounts.Web.Controllers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
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
