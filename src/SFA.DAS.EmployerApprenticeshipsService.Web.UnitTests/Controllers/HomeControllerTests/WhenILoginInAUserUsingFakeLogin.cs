using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.HomeControllerTests
{

    public class WhenILoginInAUserUsingFakeLogin
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _homeController = new HomeController(
                _owinWrapper.Object,null, _configuration.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public void ThenTheOwinWrapperIsNotCalledIfTheUserIsNotSelected()
        {
            //Act
            _homeController.SignInUser(null, new SignInUserViewModel {AvailableUsers = new List<SignInUserModel>()});

            //Assert
            _owinWrapper.Verify(x => x.IssueLoginCookie(It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public void ThenThePartialLoginCookieIsNotRemovedIfTheUserIsNull()
        {
            //Act
            _homeController.SignInUser(null, new SignInUserViewModel { AvailableUsers = new List<SignInUserModel>() });

            //Assert
            _owinWrapper.Verify(x => x.RemovePartialLoginCookie(),Times.Never);
        }

        [Test]
        public void ThenTheOwinWrapperIsCalledWithThePostParameters()
        {
            //Arrange
            var firstName = "test";
            var lastName = "tester";
            var id = Guid.NewGuid().ToString();
            var model = GetModel(firstName, lastName, id);
            //Act
            _homeController.SignInUser(id, model);
            
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
            var model = GetModel(firstName, lastName, id);

            //Act
            _homeController.SignInUser(id, model);

            //Assert
            _owinWrapper.Verify(x => x.SignInUser(id, $"{firstName} {lastName}", $"{firstName}.{lastName}@test.local"));
        }

        private static SignInUserViewModel GetModel(string firstName, string lastName, string id)
        {
            return new SignInUserViewModel
            {
                AvailableUsers = new List<SignInUserModel>
                {
                    new SignInUserModel
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        UserId = id,
                       UserSelected = id
                    }
                }
            };
        }
    }
}
