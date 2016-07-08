using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.HomeControllerTests
{
    public class WhenILogout
    {
        private Mock<IOwinWrapper> _owinWrapper;
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _homeController = new HomeController(_owinWrapper.Object);
        }

        [Test]
        public void ThenTheOwinWrapperSignOutIsCalled()
        {
            //Act
            _homeController.SignOut();

            //Assert
            _owinWrapper.Verify(x=>x.SignOutUser());
        }
    }
}
