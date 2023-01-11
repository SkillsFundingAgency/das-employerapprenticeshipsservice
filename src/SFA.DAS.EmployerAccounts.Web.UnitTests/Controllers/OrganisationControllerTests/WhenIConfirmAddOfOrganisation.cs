using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests
{
    /// <summary>
    /// AML-2459: Move to EmployerAccounts site tests
    /// </summary>
    public class WhenIConfirmAddOfOrganisation
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;      
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        private const string TestHashedAgreementId = "DEF456";

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();           
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _mapper = new Mock<IMapper>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntityViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            HashedAgreementId = TestHashedAgreementId
                        }
                    }
                });

            _logger = new Mock<ILog>();

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,           
                _userViewTestingService.Object,
                _mapper.Object,
                _logger.Object,
                _flashMessage.Object);
        }

        [Test]
        public async Task ThenIAmRedirectedToNextStepsViewIfSuccessful()
        {
            //Act
            var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, false) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("OrganisationAddedNextSteps", result.RouteValues["Action"]);
        }

        [Test]
        public async Task ThenIAmRedirectedToNextStepsNewSearchIfTheNewSearchFlagIsSet()
        {
            //Act
            var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, true) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("OrganisationAddedNextStepsSearch", result.RouteValues["Action"]);
        }

        [Test]
        public async Task ThenIAmSuppliedTheHashedAgreementIdForANewSearch()
        {
            //Act
            var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, true) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(TestHashedAgreementId, result.RouteValues["HashedAgreementId"]);
        }
    }
}
