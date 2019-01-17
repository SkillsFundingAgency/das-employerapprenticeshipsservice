using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    class WhenIEnterAPostcodeForAnAddress : OrganisationSharedControllerTestBase
    {
        private SelectOrganisationAddressViewModel _viewModel;

        [SetUp]
        public override void Arrange()
        {
            _viewModel = new SelectOrganisationAddressViewModel();
            base.Arrange();
        }

        public override void SetupOrchestrator()
        {
            base.Orchestrator.Setup(x => x.GetAddressesFromPostcode(It.IsAny<FindOrganisationAddressViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<SelectOrganisationAddressViewModel>()
                {
                    Data = _viewModel,
                    Status = HttpStatusCode.OK
                });
        }

        public override void SetupMapper()
        {
            base.Mapper.Setup(x => x.Map<AddOrganisationAddressViewModel>(It.IsAny<FindOrganisationAddressViewModel>()))
                .Returns(new AddOrganisationAddressViewModel()
                {
                    Address = new AddressViewModel()
                });

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
            var viewResult = await CallSelectAddressOnControllerAndReturnViewResult();

            //Assert
            Assert.AreEqual(ControllerConstants.AddOrganisationAddressViewName, viewResult?.ViewName);
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
            var viewResult = await CallSelectAddressOnControllerAndReturnViewResult();

            //Assert
            Assert.IsEmpty(viewResult?.ViewName); //Empty view name will go to the select address View
        }

        private async Task<ViewResult> CallSelectAddressOnControllerAndReturnViewResult()
        {
            var viewResult = await base.Controller.SelectAddress(new FindOrganisationAddressViewModel()) as ViewResult;
            return viewResult;
        }
    }
}
