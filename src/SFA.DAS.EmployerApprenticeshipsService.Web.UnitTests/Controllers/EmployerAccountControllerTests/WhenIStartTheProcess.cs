using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    public class WhenIStartTheProcess
    {
        private EmployerAccountController _employerAccountController;

        [SetUp]
        public void Arrange()
        {
            _employerAccountController = new EmployerAccountController();
        }

        [Test]
        public void ThenIAmPresentedWithTheEligibilityPage()
        {
            //Act
            var actual = _employerAccountController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(string.Empty,actualViewResult.ViewName);
        }
    }
}
