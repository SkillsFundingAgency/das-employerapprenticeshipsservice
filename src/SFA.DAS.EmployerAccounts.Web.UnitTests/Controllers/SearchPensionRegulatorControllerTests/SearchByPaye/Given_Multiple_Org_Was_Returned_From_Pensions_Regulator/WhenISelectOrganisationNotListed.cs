﻿using System.Collections.Generic;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByPaye.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISelectOrganisationNotListed
    {
        private SearchPensionRegulatorController _controller;    
       
        [SetUp]
        public void Setup()
        {                   
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();
          
            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>());
        }

        [Test]
        public void ThenTheSearchOrganisationPageIsDisplayed()
        {
            var viewModel = new SearchPensionRegulatorResultsViewModel
            {
                Results = new List<PensionRegulatorDetailsViewModel>
                {
                    new PensionRegulatorDetailsViewModel {ReferenceNumber = 1},
                    new PensionRegulatorDetailsViewModel {ReferenceNumber = 2}
                },
                SelectedOrganisation = 0
            };

            var response = _controller.SearchPensionRegulator(It.IsAny<string>(), viewModel).Result;
            var redirectResponse = (RedirectToRouteResult) response;

            Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}
