using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    public class WhenIAddAnOrganisation
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

            _orchestrator.Setup(x => x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.OK
                });

            _orchestrator.Setup(x =>
                    x.CreateAddOrganisationAddressViewModelFromOrganisationDetails(
                        It.IsAny<OrganisationDetailsViewModel>()))
                .Returns(new OrchestratorResponse<AddOrganisationAddressModel>());

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userWhiteList.Object,
                _mapper.Object);
        }

        [Test]
        public async Task ThenIMustSelectAType()
        {
            //Arrange
            var model = new AddLegalEntityViewModel();

            //Act
            var result = await _controller.AddOrganisation(model);

            //Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("", viewResult.ViewName);
        }

    }
}
