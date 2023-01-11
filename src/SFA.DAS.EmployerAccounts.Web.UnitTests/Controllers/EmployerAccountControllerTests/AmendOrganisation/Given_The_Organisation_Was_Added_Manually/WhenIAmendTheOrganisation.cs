using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendOrganisation.Given_The_Organisation_Was_Added_Manually
{
    [TestFixture]
    public class WhenIAmendTheOrganisation
    {
        private EmployerAccountController _employerAccountController;

        [SetUp]
        public void Setup()
        {
            var orchestrator = new Mock<EmployerAccountOrchestrator>();
            orchestrator.Setup(x => x.GetCookieData()).Returns(new EmployerAccountData
            {
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData { OrganisationType = OrganisationType.Charities }
            });

            _employerAccountController = new EmployerAccountController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ILog>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
        }

        [Test]
        public async Task ThenTheSearchOrganisationPageIsDisplayed()
        {
            var response =  _employerAccountController.AmendOrganisation();
            var redirectResponse = (RedirectToRouteResult)response;

            Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
