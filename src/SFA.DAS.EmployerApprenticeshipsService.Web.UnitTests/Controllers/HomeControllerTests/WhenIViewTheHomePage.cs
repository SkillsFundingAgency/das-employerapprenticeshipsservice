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
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenIViewTheHomePage
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;
        private Mock<HomeOrchestrator> _homeOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _homeOrchestrator = new Mock<HomeOrchestrator>();
            _homeOrchestrator.Setup(x => x.GetUsers()).ReturnsAsync(new SignInUserViewModel());

            _homeController = new HomeController(_owinWrapper.Object, _homeOrchestrator.Object);
        }

        [Test]
        public async Task ThenTheUsersAreReturnedFromTheOrchestrator()
        {
            //Act
            await _homeController.FakeUserSignIn();

            //Assert
            _homeOrchestrator.Verify(x=>x.GetUsers());
        }

        [Test]
        public async Task ThenTheModelIsPassedToTheView()
        {
            //Act
            var actual = await _homeController.FakeUserSignIn();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            var actualModel = actualViewResult.Model as SignInUserViewModel;
            Assert.IsNotNull(actualModel);
        }
    }
}
