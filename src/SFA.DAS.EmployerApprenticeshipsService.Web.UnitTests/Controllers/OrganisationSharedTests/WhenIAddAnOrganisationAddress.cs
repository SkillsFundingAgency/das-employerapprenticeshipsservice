using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.OrganisationSharedTests
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
