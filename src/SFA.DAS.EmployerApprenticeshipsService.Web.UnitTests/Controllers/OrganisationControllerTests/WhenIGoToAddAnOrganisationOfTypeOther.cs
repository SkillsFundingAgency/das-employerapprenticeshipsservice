﻿using System.Threading.Tasks;
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
    public class WhenIGoToAddAnOrganisationOfTypeOther
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

            _orchestrator.Setup(x => x.GetAddOtherOrganisationViewModel(It.IsAny<string>()))
                .Returns(new OrchestratorResponse<OrganisationDetailsViewModel>());

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userWhiteList.Object,
                _mapper.Object);
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
