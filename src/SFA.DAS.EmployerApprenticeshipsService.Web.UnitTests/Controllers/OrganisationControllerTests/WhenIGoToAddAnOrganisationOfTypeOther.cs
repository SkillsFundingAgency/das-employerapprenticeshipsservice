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
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenIGoToAddAnOrganisationOfTypeOther
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<IMapper> _mapper;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _mapper = new Mock<IMapper>();

            _orchestrator.Setup(x => x.GetAddOtherOrganisationViewModel(It.IsAny<string>()))
                .Returns(new OrchestratorResponse<OrganisationDetailsViewModel>());

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
        public void ThenIGetTheAddOtherOrganisationView()
        {
            //Act
            var result = _controller.AddOtherOrganisationDetails("ABC123") as ViewResult;

            //Assert
            _orchestrator.Verify(x => x.GetAddOtherOrganisationViewModel(It.Is<string>(s => s == "ABC123")), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ViewName);
        }
    }
}
