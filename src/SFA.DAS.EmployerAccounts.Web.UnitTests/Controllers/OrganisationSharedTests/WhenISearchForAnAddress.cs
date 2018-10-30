using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    public class WhenISearchForAnAddress : OrganisationSharedControllerTestBase
    {
        [Test]
        public void AndAHashedAccountIdIsProvidedWillReturnTheView()
        {
            base.SetupPopulatedRouteData();

            var result = CallFindAddressOnControllerWith(new FindOrganisationAddressViewModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void AndAHashedAccountIdIsProvidedWillNotCreateCreateTheCookieData()
        {
            base.SetupPopulatedRouteData();

            var result = CallFindAddressOnControllerWith(new FindOrganisationAddressViewModel());

            base.Orchestrator.Verify(o => o.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()), Times.Never);
        }

        [Test]
        public void AndNoHashedAccountIdIsProvidedReturnsRedirectToAction()
        {
            base.SetupEmptyRouteData();
            SetupOrchestratorForCookie();

            var result = CallFindAddressOnControllerWith(CreatePopulatedViewModel()) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(ControllerConstants.GatewayInformViewName,
                result.RouteValues[ControllerConstants.ActionKeyName]);
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName,
                result.RouteValues[ControllerConstants.ControllerKeyName]);
        }

        [Test]
        public void AndNoHashedAccountIdIsProvidedCookieDataIsCreated()
        {
            base.SetupEmptyRouteData();
            SetupOrchestratorForCookie();

            CallFindAddressOnControllerWith(CreatePopulatedViewModel());

            base.Orchestrator.Verify(o => o.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()), Times.Once);
        }

        private void SetupOrchestratorForCookie()
        {
            base.Orchestrator.Setup(o => o.GetCookieData(It.IsAny<HttpContextBase>()))
                .Returns(new EmployerAccountData());
        }

        private static FindOrganisationAddressViewModel CreatePopulatedViewModel()
        {
            return new FindOrganisationAddressViewModel
            {
                OrganisationAddress = "some address"
            };
        }

        private ActionResult CallFindAddressOnControllerWith(FindOrganisationAddressViewModel viewModel)
        {
            var result = base.Controller.FindAddress(viewModel);
            return result;
        }
    }
}
