using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    class WhenIViewMyNewLegalEntityAgreement
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _owinWrapper = new Mock<IOwinWrapper>();
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();

            _controller = new EmployerAgreementController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public async Task ThenIShouldSeeTheLatestLegalAgreement()
        {
            //Assign
            const string entityName = "Test Corp";
            const string entityCode = "1234";
            const string entityAddress = "Test street";
            var entityIncorporated = DateTime.Now;
            const string accountId = "1";
            const string userId = "user";

            var expectedResponse = new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel(),
                Status = HttpStatusCode.OK
            };
           
            _orchestrator.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(expectedResponse);

            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(userId);
            
            //Act
            var result = await _controller.ViewEntityAgreement(accountId, entityName, entityCode, entityAddress, entityIncorporated) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse, result.Model);
            _orchestrator.Verify(x => x.Create(accountId, userId, entityName, entityCode, entityAddress, entityIncorporated), Times.Once);
        }
    }
}
