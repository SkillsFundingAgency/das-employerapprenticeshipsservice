﻿using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AmendPaye.Given_The_GG_Route_Was_Used
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
                EmployerAccountPayeRefData = new EmployerAccountPayeRefData { AORN = "" }
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
        public async Task ThenTheGatewayInformPageIsDisplayed()
        {
            var response = _employerAccountController.AmendPaye();
            var redirectResponse = (RedirectToRouteResult) response;

            Assert.AreEqual(ControllerConstants.GatewayInformActionName, redirectResponse.RouteValues["action"].ToString());       
        }
    }
}
