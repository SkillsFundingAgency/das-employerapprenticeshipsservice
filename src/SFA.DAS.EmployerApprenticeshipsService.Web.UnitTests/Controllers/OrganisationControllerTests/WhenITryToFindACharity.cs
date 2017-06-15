﻿using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenITryToFindACharity
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _mapper = new Mock<IMapper>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.GetCharityByRegistrationNumber(It.Is<string>(c => c == "12345"), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.OK
                });

            _orchestrator.Setup(x => x.GetCharityByRegistrationNumber(It.Is<string>(c => c == "666"), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.NotFound
                });

            _logger = new Mock<ILog>();

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                _mapper.Object,
                _logger.Object,
                _flashMessage.Object);
        }

        [Test]
        public async Task ThenTheCharityRegistrationNumberShouldBeUsedToFindTheCharity()
        {
            //Arrange
            var model = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "12345"
            };

            //Act
            var result = await _controller.AddOrganisation(model);

            //Assert
            _orchestrator.Verify(x => x.GetCharityByRegistrationNumber(It.Is<string>(s=> s == "12345"), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheConfirmationPageIfTheCharityExists()
        {
            //Arrange
            var model = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "12345"
            };

            //Act
            var result = await _controller.AddOrganisation(model) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("FindAddress", result.ViewName);
        }

        [Test]
        public async Task ThenIShouldBeUnableToProceedIfTheCharityDoesNotExist()
        {
            //Arrange
            var model = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "666"
            };

            //Act
            var result = await _controller.AddOrganisation(model) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AddOrganisation", result.ViewName);
        }


    }
}
