﻿using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendOrganisation.Given_Multiple_Orgs_Were_Returned_From_Pensions_Regulator_Via_AORN
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
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData { OrganisationType = OrganisationType.PensionsRegulator, PensionsRegulatorReturnedMultipleResults = true },
                EmployerAccountPayeRefData = new EmployerAccountPayeRefData { AORN = "AORN" }
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
        public async Task ThenTheAORNPensionRegulatorChooseOrganisationPageIsDisplayed()
        {
            var response = _employerAccountController.AmendOrganisation();
            var redirectResponse = (RedirectToRouteResult)response;

            Assert.AreEqual(ControllerConstants.SearchUsingAornActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.SearchPensionRegulatorControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
