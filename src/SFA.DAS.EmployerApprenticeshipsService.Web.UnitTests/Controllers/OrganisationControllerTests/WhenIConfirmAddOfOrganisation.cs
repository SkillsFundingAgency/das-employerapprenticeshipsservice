using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.EmployerAgreement;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenIConfirmAddOfOrganisation
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

            _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntity>()))
                .ReturnsAsync(new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView()
                    }
                });

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userWhiteList.Object,
                _mapper.Object);
        }

        [Test]
        public async Task ThenIAmRedirectedToEmployerAgreementViewIfSuccessful()
        {
            //Act
            var result = await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, "") as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("EmployerAgreement", result.RouteValues["Controller"]);
            Assert.AreEqual("Index", result.RouteValues["Action"]);
        }
    }
}
