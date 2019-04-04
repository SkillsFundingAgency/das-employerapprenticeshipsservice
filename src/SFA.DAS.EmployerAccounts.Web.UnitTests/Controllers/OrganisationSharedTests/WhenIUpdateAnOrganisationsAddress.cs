using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    class WhenIUpdateAnOrganisationsAddress : OrganisationSharedControllerTestBase
    {
        public override void SetupOrchestrator()
        {
            base.Orchestrator.Setup(o => o.AddOrganisationAddress(It.IsAny<AddOrganisationAddressViewModel>()))
                .Returns(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new OrganisationDetailsViewModel()
                });

            base.Orchestrator.Setup(o => o.GetCookieData(It.IsAny<HttpContextBase>()))
                .Returns(
                    new EmployerAccountData
                    {
                        EmployerAccountOrganisationData = new EmployerAccountOrganisationData(),
                        EmployerAccountPayeRefData = new EmployerAccountPayeRefData()
                    });
        }

        [Test]
        public void ThenAddOrganisationAddressIsCalledOnTheOrchestrator()
        {
            base.SetupPopulatedRouteData();
            CallUpdateOrganisationAddressOnControllerAndReturnResult(new AddOrganisationAddressViewModel());

            base.Orchestrator.Verify(o => o.AddOrganisationAddress(It.IsAny<AddOrganisationAddressViewModel>()), Times.Once);
        }

        [Test]
        public void ThenIfTheOrchestratorResponseIsInvalidTheAddOrganisationViewIsReturned()
        {
            SetupOrchestratorResponseForABadRequest();

            var result = CallUpdateOrganisationAddressOnControllerAndReturnResult(new AddOrganisationAddressViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.AddOrganisationAddressViewName, result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<OrchestratorResponse<AddOrganisationAddressViewModel>>(result.Model);
        }

        [Test]
        public void ThenIfTheOrchestratorResponseIsInvalidTheModelDataIsCorrectlyConstructed()
        {
            SetupOrchestratorResponseForABadRequest();

            var requestModel = new AddOrganisationAddressViewModel();
            var result = CallUpdateOrganisationAddressOnControllerAndReturnResult(requestModel) as ViewResult;
            var actual = result.Model as OrchestratorResponse<AddOrganisationAddressViewModel>;

            Assert.AreEqual(requestModel, actual.Data);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.IsNotNull(actual.Exception);
            Assert.IsNotNull(actual.Data.Address.ErrorDictionary);
            Assert.AreEqual(1, actual.Data.Address.ErrorDictionary.Count);
            Assert.IsNotNull(actual.FlashMessage);
            Assert.AreEqual("a flash message", actual.FlashMessage.Message);
        }

        [Test]
        public void ThenIfAHashedAccountIdIsNotPresentInTheRouteARedirectToGatewayInformActionIsReturned()
        {
            base.SetupEmptyRouteData();
            var result = CallUpdateOrganisationAddressOnControllerAndReturnResult(new AddOrganisationAddressViewModel()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.GatewayInformViewName, result.RouteValues[ControllerConstants.ActionKeyName]);
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, result.RouteValues[ControllerConstants.ControllerKeyName]);
        }


        [Test]
        public void ThenIfAHashedAccountIdIsPresentInTheRouteTheConfirmOrganisationsDetailsViewIsReturned()
        {
            base.SetupPopulatedRouteData();
            var result = CallUpdateOrganisationAddressOnControllerAndReturnResult(new AddOrganisationAddressViewModel()) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.ConfirmOrganisationDetailsViewName, result.ViewName);
        }

        private void SetupOrchestratorResponseForABadRequest()
        {
            base.Orchestrator.Setup(o => o.AddOrganisationAddress(It.IsAny<AddOrganisationAddressViewModel>()))
                .Returns(new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Data = new OrganisationDetailsViewModel
                    {
                        ErrorDictionary = new Dictionary<string, string>
                        {
                            {"Error", "Description"}
                        }
                    },
                    Exception = new Exception("Exception"),
                    FlashMessage = new FlashMessageViewModel{ Message = "a flash message"}
                });
        }

        private ActionResult CallUpdateOrganisationAddressOnControllerAndReturnResult(AddOrganisationAddressViewModel viewModel)
        {
            return base.Controller.UpdateOrganisationAddress(viewModel);
        }
    }
}
