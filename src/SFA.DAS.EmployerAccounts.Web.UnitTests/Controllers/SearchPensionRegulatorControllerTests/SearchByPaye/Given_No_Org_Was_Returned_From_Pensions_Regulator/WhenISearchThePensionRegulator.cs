﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_No_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISearchThePensionRegulator
    {
        private SearchPensionRegulatorController _controller;    
       
        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();

            orchestrator
                .Setup(x => x.SearchPensionRegulator(It.IsAny<string>()))
                .ReturnsAsync(
                    new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                    {
                        Data = new SearchPensionRegulatorResultsViewModel
                        {
                            Results = new List<PensionRegulatorDetailsViewModel>()
                        }
                    });

            orchestrator.Setup(x => x.GetCookieData())
                .Returns(
                    new EmployerAccountData
                    {
                        EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                        {
                            PayeReference = "PayeRef"
                        }
                    });

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>());
        }

        [Test]
        public async Task ThenTheSearchOrganisationPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulator(It.IsAny<string>());
            var redirectResponse = (RedirectToRouteResult)response;

            Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
