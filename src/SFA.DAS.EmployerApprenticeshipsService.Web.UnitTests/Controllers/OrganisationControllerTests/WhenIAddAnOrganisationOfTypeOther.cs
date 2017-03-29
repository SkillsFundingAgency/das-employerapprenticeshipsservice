using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenIAddAnOrganisationOfTypeOther
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<IMapper> _mapper;
        private OrchestratorResponse<OrganisationDetailsViewModel> _validationResponse;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _mapper = new Mock<IMapper>();

            _validationResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel(),
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()))
                .ReturnsAsync(_validationResponse);

            _mapper.Setup(x => x.Map<FindOrganisationAddressViewModel>(It.IsAny<OrganisationDetailsViewModel>()))
                    .Returns(new FindOrganisationAddressViewModel());

            _logger = new Mock<ILogger>();

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                _mapper.Object,
                _logger.Object);
        }

        [Test]
        public async Task ThenTheDetailsShouldBeValidated()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            await _controller.AddOtherOrganisationDetails(model);

            //Assert
            _orchestrator.Verify(x=> x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()), Times.Once);
        }

        [Test]
        public async Task ThenAnAddressViewModelShouldBeGeneratedIfValid()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            await _controller.AddOtherOrganisationDetails(model);

            //Assert
            _mapper.Verify(x => x.Map<FindOrganisationAddressViewModel>(_validationResponse.Data), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheAddressDetailsPageIfTheDetailsAreValid()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            var result = await _controller.AddOtherOrganisationDetails(model);

            //Assert
            var redirectResult = result as RedirectToRouteResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("FindAddress", redirectResult?.RouteValues["Action"]);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedBackIfTheDetailsAreInvalid()
        {
            //Arrange
            _orchestrator.Setup(x => x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.BadRequest
                });

            var model = new OrganisationDetailsViewModel();

            //Act
            var result = await _controller.AddOtherOrganisationDetails(model);

            //Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("AddOtherOrganisationDetails", viewResult?.ViewName);
        }


    }
}
