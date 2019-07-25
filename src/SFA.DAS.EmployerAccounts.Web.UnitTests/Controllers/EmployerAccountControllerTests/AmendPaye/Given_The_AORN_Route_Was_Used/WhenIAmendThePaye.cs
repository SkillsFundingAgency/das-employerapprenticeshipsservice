using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendPaye.Given_The_AORN_Route_Was_Used
{
    [TestFixture]
    class WhenIAmendThePaye
    {
        private EmployerAccountController _employerAccountController;

        [SetUp]
        public void Setup()
        {
            var orchestrator = new Mock<EmployerAccountOrchestrator>();
            orchestrator.Setup(x => x.GetCookieData()).Returns(new EmployerAccountData
            {
                EmployerAccountPayeRefData = new EmployerAccountPayeRefData { AORN = "AnAORNValue" }
            });

            _employerAccountController = new EmployerAccountController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ILog>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
                Mock.Of<IAuthorizationService>());
        }

        [Test]
        public async Task ThenTheEnterYourPAYESchemeDetailsPageIsDisplayed()
        {
            var response = _employerAccountController.AmendPaye();
            var redirectResponse = (RedirectToRouteResult) response;

            Assert.AreEqual(ControllerConstants.WaysToAddPayeSchemeActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.EmployerAccountPayeControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
