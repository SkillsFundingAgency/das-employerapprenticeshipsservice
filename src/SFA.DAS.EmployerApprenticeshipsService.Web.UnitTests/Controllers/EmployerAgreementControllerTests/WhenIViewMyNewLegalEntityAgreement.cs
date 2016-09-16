using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Controllers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    class WhenIViewMyNewLegalEntityAgreement
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();

            _controller = new EmployerAgreementController(_owinWrapper.Object, _orchestrator.Object);
        }

        [Test]
        public async Task ThenIShouldSeeTheLatestLegalAgreement()
        {
            //Assign
            const string entityName = "Test Corp";
            const string entityCode = "1234";
            const string entityAddress = "Test street";
            var entityIncorporated = DateTime.Now;
            const int accountId = 1;

            var expectedResponse = new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel(),
                Status = HttpStatusCode.OK
            };
           
            _orchestrator.Setup(x => x.Create(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(expectedResponse);
            
            //Act
            var result = await _controller.ViewEntityAgreement(accountId, entityName, entityCode, entityAddress, entityIncorporated) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse, result.Model);
            _orchestrator.Verify(x => x.Create(accountId, entityName, entityCode, entityAddress, entityIncorporated), Times.Once);
        }
    }
}
