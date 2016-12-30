using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAgreementControllerTests
{
    public class WhenITryToFindACompany
    {
        private EmployerAgreementController _controller;
        private Mock<EmployerAgreementOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAgreementOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();


            _controller = new EmployerAgreementController(
                _owinWrapper.Object, _orchestrator.Object, _featureToggle.Object, _userWhiteList.Object);
        }

        [Test]
        public async Task ThenIShouldGetCompanyDetailsBackIfTheyExist()
        {
            //Arrange
            var viewModel = new FindOrganisationViewModel
            {
                HashedLegalEntityId = "1",
                CompanyName = "Test Corp",
                CompanyNumber = "0123456",
                DateOfIncorporation = DateTime.Now,
                RegisteredAddress = "1 Test Road, Test City, TE12 3ST",
                CompanyStatus = "active"
            };

            _orchestrator.Setup(x => x.FindLegalEntity(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = viewModel
                });

            var username = "user";
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(username);

            //Act
            var result = await _controller.FindLegalEntity(viewModel.HashedLegalEntityId, viewModel.CompanyNumber) as ViewResult;
            
            //Assert
            Assert.IsNotNull(result);

            _orchestrator.Verify(x => x.FindLegalEntity(viewModel.HashedLegalEntityId, viewModel.CompanyNumber, username), Times.Once);

            var model = result.Model as OrchestratorResponse<FindOrganisationViewModel>;
            Assert.IsNotNull(model?.Data);
        }
    }
}
