using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAccountPayeOrchestrator
{
    public class WhenIRemoveAPayeScheme
    {
        private Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator> _employerAccountPayeOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private EmployerAccountPayeController _controller;

        [SetUp]
        public void Arrange()
        {
            _employerAccountPayeOrchestrator = new Mock<Web.Orchestrators.EmployerAccountPayeOrchestrator>();
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>())).ReturnsAsync(new OrchestratorResponse<bool>());
            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("sub")).Returns("123abc");

            _controller = new EmployerAccountPayeController(_owinWrapper.Object,_employerAccountPayeOrchestrator.Object);
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalledIfYouConfirmToRemoveTheScheme()
        {
            //Act
            var actual = await _controller.RemovePaye(new RemovePayeScheme());

            //Assert
            _employerAccountPayeOrchestrator.Verify(x=>x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>()), Times.Once);
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual("Index",actualRedirect.RouteValues["Action"]);
            Assert.AreEqual("EmployerAccountPaye", actualRedirect.RouteValues["Controller"]);
        }

        [Test]
        public async Task ThenTheConfirmRemoveSchemeViewIsReturnedIfThereIsAnError()
        {
            //Arrange
            _employerAccountPayeOrchestrator.Setup(x => x.RemoveSchemeFromAccount(It.IsAny<RemovePayeScheme>())).ReturnsAsync(new OrchestratorResponse<bool> {Status = HttpStatusCode.BadRequest});

            //Act
            var actual = await _controller.RemovePaye(new RemovePayeScheme());

            //Assert
            Assert.IsNotNull(actual);
            var actualView = actual as ViewResult;
            Assert.IsNotNull(actualView);
            Assert.AreEqual("GenericError",actualView.ViewName);
        }
    }
}
