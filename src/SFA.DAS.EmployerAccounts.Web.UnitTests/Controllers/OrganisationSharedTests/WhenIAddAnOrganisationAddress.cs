using System.Net;
using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    class WhenIAddAnOrganisationAddress : OrganisationSharedControllerTestBase
    {
        private ActionResult CallAddOrganisationAddressOnControllerAndReturnTheResult(AddOrganisationAddressViewModel viewModel)
        {
            return base.Controller.AddOrganisationAddress(viewModel);
        }

        [Test]
        public void AndAHashedAccountIdIsNotProvidedInTheRouteWillReturnTheConfirmOrganisationDetailsView()
        {
            base.SetupEmptyRouteData();

            var result = CallAddOrganisationAddressOnControllerAndReturnTheResult(new AddOrganisationAddressViewModel
            {
                OrganisationAddress =  "some address"
            }) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.ConfirmOrganisationDetailsViewName, result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<OrchestratorResponse<OrganisationDetailsViewModel>>(result.Model);
        }

        [Test]
        public void AndAHashedAccountIdIsProvidedInTheRouteWillReturnTheAddOrganisationDetailsView()
        {
            base.SetupPopulatedRouteData();

            var result = CallAddOrganisationAddressOnControllerAndReturnTheResult(new AddOrganisationAddressViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.AddOrganisationAddressViewName, result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<OrchestratorResponse<AddOrganisationAddressViewModel>>(result.Model);
        }

        [Test]
        public void AndAHashedAccountIdIsProvidedInTheRouteTheModelIsCorrectlyConstructed()
        {
            base.SetupPopulatedRouteData();

            var result = CallAddOrganisationAddressOnControllerAndReturnTheResult(new AddOrganisationAddressViewModel()) as ViewResult;

            var actualModel = (OrchestratorResponse<AddOrganisationAddressViewModel>)result.Model;
            Assert.IsInstanceOf<AddOrganisationAddressViewModel>(actualModel.Data);
            Assert.AreEqual(HttpStatusCode.OK, actualModel.Status);
        }
    }
}
