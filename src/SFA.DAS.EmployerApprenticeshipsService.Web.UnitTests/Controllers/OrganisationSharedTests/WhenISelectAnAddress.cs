using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationSharedTests
{
    class WhenISelectAnAddress : OrganisationSharedControllerTestBase
    {
        public override void SetupMapper()
        {
            base.Mapper.Setup(o => o.Map<AddOrganisationAddressViewModel>(It.IsAny<FindOrganisationAddressViewModel>()))
                .Returns(new AddOrganisationAddressViewModel());
        }

        public override void SetupOrchestrator()
        {
            base.Orchestrator.Setup(o => o.GetAddressesFromPostcode(It.IsAny<FindOrganisationAddressViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<SelectOrganisationAddressViewModel>
                {
                    Data = new SelectOrganisationAddressViewModel
                    {
                        Addresses = new List<AddressViewModel>
                        {
                            new AddressViewModel()
                        }
                    }
                });
        }

        [Test]
        public async Task AndTheOrchestratorResponseContainsNoAddressesTheViewIsReturned()
        {
            base.Orchestrator.Setup(o => o.GetAddressesFromPostcode(It.IsAny<FindOrganisationAddressViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<SelectOrganisationAddressViewModel>()
                {
                    Data = new SelectOrganisationAddressViewModel()
                });

            var result = await CallSelectAddressOnControllerReturningResponse();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<OrchestratorResponse<SelectOrganisationAddressViewModel>>(((ViewResult) result).Model);
        }

        private async Task<ActionResult> CallSelectAddressOnControllerReturningResponse()
        {
            return await base.Controller.SelectAddress(new FindOrganisationAddressViewModel());
        }

        [Test]
        public async Task AndTheOrchestratorResponseContainsAListOfAddressesTheAddOrganisationAddressViewIsReturned()
        {
            var result = await CallSelectAddressOnControllerReturningResponse();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<OrchestratorResponse<AddOrganisationAddressViewModel>>(((ViewResult) result).Model);
        }

        [Test]
        public async Task AndTheOrchestratorResponseContainsAListOfAddressesTheRequestIsMappedToAnAddOrganisationAddressViewModel()
        {
            var result = await CallSelectAddressOnControllerReturningResponse();

            base.Mapper.Verify(
                o => o.Map<AddOrganisationAddressViewModel>(It.IsAny<FindOrganisationAddressViewModel>()), Times.Once);
        }

        [Test]
        public async Task AndTheOrchestratorResponseContainsAListOfAddressesTheModelIsConstructedCorrectly()
        {
            var result = await CallSelectAddressOnControllerReturningResponse() as ViewResult;
            var actualModel =(OrchestratorResponse<AddOrganisationAddressViewModel>)result.Model;

            Assert.IsNotNull(actualModel.Data);
            Assert.AreEqual(HttpStatusCode.OK, actualModel.Status);
        }
    }
}
