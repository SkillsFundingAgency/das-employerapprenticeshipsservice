using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationControllerTests
{
    class WhenIEnterAPostcodeForAnAddress
    {
        private OrganisationController _controller;
        private Mock<OrganisationOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IUserWhiteList> _userWhiteList;
        private Mock<IMapper> _mapper;
        private SelectOrganisationAddressViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggle>();
            _userWhiteList = new Mock<IUserWhiteList>();
            _mapper = new Mock<IMapper>();

            _viewModel = new SelectOrganisationAddressViewModel();

            _orchestrator.Setup(x => x.GetAddressesFromPostcode(It.IsAny<FindOrganisationAddressViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<SelectOrganisationAddressViewModel>()
                {
                    Data = _viewModel,
                    Status = HttpStatusCode.OK
                });

            _mapper.Setup(x => x.Map<AddOrganisationAddressViewModel>(It.IsAny<FindOrganisationAddressViewModel>()))
                .Returns(new AddOrganisationAddressViewModel()
                {
                    Address = new AddressViewModel()
                });

            _controller = new OrganisationController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _featureToggle.Object,
                _userWhiteList.Object,
                _mapper.Object);
        }

        [Test]
        public async Task ThenIfASignleAddressIsFoundTheAddressViewShouldBePresented()
        {
            //Arange
            _viewModel.Addresses = new List<AddressViewModel>
            {
                new AddressViewModel()
            };

            //Act
            var viewResult = await _controller.SelectAddress(new FindOrganisationAddressViewModel()) as ViewResult;

            //Assert
            Assert.AreEqual("AddOrganisationAddress", viewResult?.ViewName);
        }

        [Test]
        public async Task ThenIfAMultiAddressesAreFoundTheSelectAddressViewShouldBePresented()
        {
            //Arange
            _viewModel.Addresses = new List<AddressViewModel>
            {
                new AddressViewModel(),
                new AddressViewModel()
            };

            //Act
            var viewResult = await _controller.SelectAddress(new FindOrganisationAddressViewModel()) as ViewResult;

            //Assert
            Assert.IsEmpty(viewResult?.ViewName); //Empty view name will go to the select address View
        }
    }
}
