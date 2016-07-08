using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{

    public class WhenILoginInAUser
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();

            _homeController = new HomeController(_owinWrapper.Object,null);
        }

        [Test]
        public void ThenTheOwinWrapperIsCalled()
        {
            //Act
            _homeController.SignInUser(new SignInUserModel());

            //Assert
            _owinWrapper.Verify(x => x.IssueLoginCookie(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void ThenThePartialLoginCookieIsRemoved()
        {
            //Act
            _homeController.SignInUser(new SignInUserModel());

            //Assert
            _owinWrapper.Verify(x => x.RemovePartialLoginCookie());
        }

        [Test]
        public void ThenTheOwinWrapperIsCalledWithThePostParameters()
        {
            //Arrange
            var firstName = "test";
            var lastName = "tester";
            var id = Guid.NewGuid().ToString();

            //Act
            _homeController.SignInUser(new SignInUserModel
            {
                FirstName = firstName,
                LastName = lastName,
                UserId = id
            });

            //Assert
            _owinWrapper.Verify(x => x.IssueLoginCookie(id, $"{firstName} {lastName}"));
        }

        [Test]
        public void ThenTheOWinWrapperSignInUserMethodIsCalledWithParams()
        {
            //Arrange
            var firstName = "test";
            var lastName = "tester";
            var id = Guid.NewGuid().ToString();

            //Act
            _homeController.SignInUser(new SignInUserModel
            {
                FirstName = firstName,
                LastName = lastName,
                UserId = id
            });

            //Assert
            _owinWrapper.Verify(x => x.SignInUser(id, $"{firstName} {lastName}", $"{firstName}.{lastName}@local.test"));
        }
    }
}
