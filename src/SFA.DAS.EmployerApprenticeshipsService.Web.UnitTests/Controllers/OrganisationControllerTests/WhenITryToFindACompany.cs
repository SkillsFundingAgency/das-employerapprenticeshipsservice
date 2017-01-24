using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenITryToFindACompany
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();
            _mapper = new Mock<IMapper>();


            _controller = new OrganisationController(
                _owinWrapper.Object, 
                _orchestrator.Object, 
                _featureToggle.Object, 
                _userWhiteList.Object,
                _mapper.Object);
        }

        [Test]
        public async Task ThenIShouldGetCompanyDetailsBackIfTheyExist()
        {
            //Arrange
            var viewModel = new OrganisationDetailsViewModel
            {
                HashedId = "1",
                Name = "Test Corp",
                ReferenceNumber = "0123456",
                DateOfInception = DateTime.Now,
                Address = "1 Test Road, Test City, TE12 3ST",
                Status = "active"
            };

            _orchestrator.Setup(x => x.GetLimitedCompanyByRegistrationNumber(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = viewModel
                });

            var username = "user";
            _owinWrapper.Setup(x => x.GetClaimValue(It.IsAny<string>())).Returns(username);

            var addModel = new AddLegalEntityViewModel
            {
                HashedAccountId = viewModel.HashedId,
                OrganisationType = OrganisationType.CompaniesHouse,
                CompaniesHouseNumber = viewModel.ReferenceNumber
            };

            //Act
            var result = await _controller.AddOrganisation(addModel) as ViewResult;

            //Assert
            Assert.IsNotNull(result);

            _orchestrator.Verify(x => x.GetLimitedCompanyByRegistrationNumber(viewModel.ReferenceNumber, viewModel.HashedId, username), Times.Once);

            var model = result.Model as OrchestratorResponse<OrganisationDetailsViewModel>;
            Assert.IsNotNull(model?.Data);
        }
    }
}
