using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILoginAUser
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<HomeOrchestrator> _homeOrchestrator;
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();

            _homeController = new HomeController(_owinWrapper.Object, _homeOrchestrator.Object);
        }

        [Test]
        public void ThenTheUserIsRedirectedToTheIndex()
        {
            //Act
            var actual = _homeController.SignIn();
            
            //Assert
            Assert.IsNotNull(actual);
            var actualRedirectResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirectResult);
            Assert.AreEqual("Index",actualRedirectResult.RouteValues["Action"]);

        }
    }
}
