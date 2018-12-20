using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    public class WhenIAddAnOrganisationOfTypeOther : OrganisationSharedControllerTestBase
    {
        private OrchestratorResponse<OrganisationDetailsViewModel> _validationResponse;
        
        [SetUp]
        public override void Arrange()
        {
            _validationResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel(),
                Status = HttpStatusCode.OK
            };

            base.Arrange();
        }

        public override void SetupOrchestrator()
        {
            base.Orchestrator.Setup(x => x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()))
                .ReturnsAsync(_validationResponse);
        }

        public override void SetupMapper()
        {
            base.Mapper.Setup(x => x.Map<FindOrganisationAddressViewModel>(It.IsAny<OrganisationDetailsViewModel>()))
                .Returns(new FindOrganisationAddressViewModel());
        }

        [Test]
        public async Task ThenTheDetailsShouldBeValidated()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            await CallAddOtherOrganisationDetailsMethodOnControllerAndReturnResult(model);

            //Assert
            base.Orchestrator.Verify(x=> x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()), Times.Once);
        }

        [Test]
        public async Task ThenAnAddressViewModelShouldBeGeneratedIfValid()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            await CallAddOtherOrganisationDetailsMethodOnControllerAndReturnResult(model);

            //Assert
            base.Mapper.Verify(x => x.Map<FindOrganisationAddressViewModel>(_validationResponse.Data), Times.Once);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheAddressDetailsPageIfTheDetailsAreValid()
        {
            //Arrange
            var model = new OrganisationDetailsViewModel();

            //Act
            var result = await CallAddOtherOrganisationDetailsMethodOnControllerAndReturnResult(model);

            //Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(ControllerConstants.FindAddressViewName, viewResult.ViewName);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedBackIfTheDetailsAreInvalid()
        {
            //Arrange
            base.Orchestrator.Setup(x => x.ValidateLegalEntityName(It.IsAny<OrganisationDetailsViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.BadRequest
                });

            var model = new OrganisationDetailsViewModel();

            //Act
            var result = await CallAddOtherOrganisationDetailsMethodOnControllerAndReturnResult(model);

            //Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(ControllerConstants.AddOtherOrganisationDetailsViewName, viewResult?.ViewName);
        }

        private async Task<ActionResult> CallAddOtherOrganisationDetailsMethodOnControllerAndReturnResult(OrganisationDetailsViewModel model)
        {
            var result = await base.Controller.AddOtherOrganisationDetails(model);
            return result;
        }
    }
}
